using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace Dublettenprüfung.Tests
{
    [TestClass]
    public class Test_Dublettenprüfung
    {
        [TestMethod]
        [DeploymentItem("testfiles", "testfiles")]
        public void Prüfe_Kandidaten()
        {
            var dublettenprüfung = new Dublettenprüfung();
            var kandidaten = dublettenprüfung.Sammle_Kandidaten("testfiles");
            var duplicate = dublettenprüfung.Prüfe_Kandidaten(kandidaten);

            Assert.IsTrue(duplicate.Count() == 5);
        }

        [TestMethod]
        [DeploymentItem("testfiles", "testfiles")]
        public void Sammle_Kandidaten()
        {
            var dublettenprüfung = new Dublettenprüfung();
            var kandidaten = dublettenprüfung.Sammle_Kandidaten("testfiles");

            Assert.IsTrue(kandidaten.Count() == 5);

            //only check size
            kandidaten = dublettenprüfung.Sammle_Kandidaten("testfiles", Vergleichsmodi.Größe);
            Assert.IsTrue(kandidaten.Count() == 2);
        }
    }
}
