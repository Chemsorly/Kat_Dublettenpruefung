using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;

namespace Dublettenprüfung
{
    /// <summary>
    /// provides a class to search for duplicates in a directory tree. allows different kind of comparisons
    /// </summary>
    public class Dublettenprüfung : IDublettenprüfung
    {
        /// <summary>
        /// gets fired when a unit of work has finished
        /// </summary>
        public event EventHandler<String> ProgressReport;

        /// <summary>
        /// checks a list of duplicate candidatews for equality by md5 hashing the files and then checking for comparison
        /// </summary>
        /// <param name="kandidaten">list of duplicate candidates</param>
        /// <returns>a list of verified duplicates</returns>
        public IEnumerable<IDublette> Prüfe_Kandidaten(IEnumerable<IDublette> kandidaten)
        {
            List<IDublette> hashDub = new List<IDublette>();
            int counter = 0;
            foreach (var kandidat in kandidaten)
            {
                var hashDict = Dublette.GetFilehashes(kandidat.Dateipfade);

                //get all duplicates
                var duplicates = hashDict.Where(t => hashDict.Any(u => t.Key != u.Key && t.Value.SequenceEqual(u.Value)));

                //check for groups
                var uniquehashes = duplicates.Select(t => t.Value).Distinct(new ByteArrayComparer());
                foreach(var uniquehash in uniquehashes)
                    hashDub.Add(new Dublette(duplicates.Where(t => t.Value.SequenceEqual(uniquehash)).Select(t => t.Key)));

                counter++;
                this.ProgressReport?.Invoke(this, $"Duplicate check finished (candidate {counter} of {kandidaten.Count()})");
            }
            return hashDub;
        }

        /// <summary>
        /// checks all files in a directory tree for size and name equality
        /// </summary>
        /// <param name="pfad">root folder of directory tree</param>
        /// <returns>a list of possible duplicates</returns>
        public IEnumerable<IDublette> Sammle_Kandidaten(string pfad)
        {
            return Sammle_Kandidaten(pfad, Vergleichsmodi.Größe_und_Name);
        }

        /// <summary>
        /// checks all files in a directory tree for size (and optionally name) equality
        /// </summary>
        /// <param name="pfad">root folder of directory tree</param>
        /// <param name="modus">comparison type</param>
        /// <returns>a list of possible duplicates</returns>
        public IEnumerable<IDublette> Sammle_Kandidaten(string pfad, Vergleichsmodi modus)
        {
            var files = Directory.GetFiles(pfad, "*", System.IO.SearchOption.AllDirectories).Select(t => new FileInfo(t)).ToList();
            List<Dublette> dubletten = new List<Dublette>();

            int counter = 0;
            files.ForEach(file =>
            {
                IEnumerable<FileInfo> targetFiles;
                switch (modus)
                {
                    case Vergleichsmodi.Größe_und_Name:
                        targetFiles = files.Where(t => t.Name == file.Name && t.Length == file.Length);
                        break;
                    case Vergleichsmodi.Größe:
                        targetFiles = files.Where(t => t.Length == file.Length);
                        break;
                    default:
                        throw new NotImplementedException();
                }
                //check if any target files were found AND first target file does not belong to any set (if it does, all target files do)
                if (targetFiles.Any() && !dubletten.SelectMany(t => t.Fileinfos).Any(t => t.FullName == targetFiles.FirstOrDefault().FullName))
                    dubletten.Add(new Dublette(targetFiles.Select(t => t.FullName)));

                counter++;
                this.ProgressReport?.Invoke(this,$"Candidate check finished (file {counter} of {files.Count})");
            });
            return dubletten;
        }
    }

    /// <summary>
    /// custom comparer to compare byte[] in function calls
    /// </summary>
    class ByteArrayComparer : IEqualityComparer<byte[]>
    {
        public bool Equals(byte[] x, byte[] y)
        {
            return x.SequenceEqual(y);
        }

        public int GetHashCode(byte[] obj)
        {
            return base.GetHashCode();
        }
    }

    /// <summary>
    /// implementation of IDublette to provide FileInfo[] of the file paths
    /// </summary>
    class Dublette : IDublette
    {
        public IEnumerable<string> Dateipfade { get; }

        public IEnumerable<FileInfo> Fileinfos { get; } = new List<FileInfo>();

        public Dublette(IEnumerable<string> pDateipfade)
        {
            Dateipfade = pDateipfade;
            Fileinfos = pDateipfade.Select(t => new FileInfo(t));
        }        

        /// <summary>
        /// creates a Dictionary of all files containing the MD5 hashes of the files
        /// </summary>
        /// <param name="pDateipfade">files to hash</param>
        /// <returns>Dictonary(Filename,MD5 hash)</returns>
        internal static Dictionary<String,byte[]> GetFilehashes(IEnumerable<String> pDateipfade)
        {
            var md5 = System.Security.Cryptography.MD5.Create();
            Dictionary<String, byte[]> hashDict = new Dictionary<String, byte[]>();
            foreach (var dateipfad in pDateipfade)
                hashDict.Add(dateipfad, md5.ComputeHash(File.ReadAllBytes(dateipfad)));
            return hashDict;
        }
    }
}
