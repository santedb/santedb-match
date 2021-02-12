using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SanteDB.Core.Model.Roles;
using SanteDB.Matcher.Configuration;
using SanteDB.Matcher.Definition;
using SanteDB.Matcher.Model;

namespace SanteDB.Matcher.Test
{
    [TestClass]
    public class MatchConfigurationTest
    {
        /// <summary>
        /// Should load configuration
        /// </summary>
        [TestMethod]
        public void ShouldLoadConfiguration()
        {
            var loaded = MatchConfiguration.Load(typeof(MatchConfigurationTest).Assembly.GetManifestResourceStream("SanteDB.Matcher.Test.Resources.DateOfBirthGenderIdClassified.xml"));
            Assert.IsNotNull(loaded);
            Assert.AreEqual(2, loaded.Scoring.Count);
            Assert.IsNotNull(loaded.Scoring.First().Assertion);
            Assert.AreEqual(1, loaded.Blocking.Count);
            Assert.AreEqual(3, loaded.Blocking.First().Filter.Count);
            Assert.AreEqual(typeof(Patient), loaded.Target.First().ResourceType);
            loaded.Scoring.First().Initialize();
            Assert.AreNotEqual(0.0d, loaded.Scoring.First().MatchWeight);
        }

        
    }
}
