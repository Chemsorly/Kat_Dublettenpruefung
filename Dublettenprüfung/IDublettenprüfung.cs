using System;
using System.Collections.Generic;
using System.Text;

namespace Dublettenprüfung
{
    /// <summary>
    /// target interface contract with evend based progress reporting
    /// </summary>
    interface IDublettenprüfung
    {
        IEnumerable<IDublette> Sammle_Kandidaten(string pfad);
        IEnumerable<IDublette> Sammle_Kandidaten(string pfad, Vergleichsmodi modus);
        IEnumerable<IDublette> Prüfe_Kandidaten(IEnumerable<IDublette> kandidaten);
        event EventHandler<String> ProgressReport;
    }

    /// <summary>
    /// target interface contract
    /// </summary>
    public interface IDublette
    {
        IEnumerable<string> Dateipfade { get; }
    }

    /// <summary>
    /// types of comparisons
    /// </summary>
    public enum Vergleichsmodi
    {
        Größe_und_Name,
        Größe
    }
}
