using System;
using System.Linq;

namespace Dublettenprüfung
{
    class Program
    {
        /// <summary>
        /// example program
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            String testPath = @"D:\Desktop\dublettentest";

            Dublettenprüfung prüfung = new Dublettenprüfung();
            prüfung.ProgressReport += delegate (object sender, string e) { Console.WriteLine(e); };

            var kandidaten = prüfung.Sammle_Kandidaten(testPath);
            foreach (var duplette in kandidaten)
            {
                Console.WriteLine($"Candidate found:");
                foreach (var file in duplette.Dateipfade)
                    Console.WriteLine(file);
                Console.WriteLine();
            }


            var dupletten = prüfung.Prüfe_Kandidaten(kandidaten);
            foreach (var duplette in dupletten)
            {
                Console.WriteLine($"Duplicate found:");
                foreach (var file in duplette.Dateipfade)
                    Console.WriteLine(file);
                Console.WriteLine();
            }

            Console.WriteLine($"Total duplicates found: {dupletten.Count()}");
            Console.ReadKey();
        }
    }
}
