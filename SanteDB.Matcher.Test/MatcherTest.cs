using System;
using System.IO;
using System.Linq;
using MARC.HI.EHRS.SVC.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SanteDB.Core;
using SanteDB.Core.Model.Constants;
using SanteDB.Core.Model.DataTypes;
using SanteDB.Core.Model.EntityLoader;
using SanteDB.Core.Model.Roles;
using SanteDB.Core.Services;
using SanteDB.Matcher.Matchers;

namespace SanteDB.Matcher.Test
{
    [TestClass]
    public class MatcherTest
    {

        /// <summary>
        /// Initialize the test class
        /// </summary>
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            AppDomain.CurrentDomain.SetData(
               "DataDirectory",
               Path.Combine(context.TestDeploymentDir, string.Empty));

            EntitySource.Current = new EntitySource(new PersistenceServiceEntitySource());

            // Register the AuditAdoPersistenceService
            ApplicationContext.Current.AddServiceProvider(typeof(MARC.HI.EHRS.SVC.Core.Configuration.LocalConfigurationManager));
            ApplicationContext.Current.AddServiceProvider(typeof(DummyMatchConfigurationProvider)); // Sec repo service is for get user name implementation
            ApplicationContext.Current.AddServiceProvider(typeof(DummyConceptRepositoryService));
            ApplicationContext.Current.AddServiceProvider(typeof(DummyPatientDataPersistenceService));
            ApplicationContext.Current.AddServiceProvider(typeof(ProbabalisticRecordMatchingService));

            // Start the daemon services
            if (!ApplicationContext.Current.IsRunning)
            {
                //adoPersistenceService.Start();
                ApplicationContext.Current.Start();
                ApplicationServiceContext.Current = ApplicationContext.Current;
            }
        }

        /// <summary>
        /// Test that matching patients are blocked
        /// </summary>
        [TestMethod]
        public void ShouldBlockMatchingPatients()
        {
            var matchService = ApplicationContext.Current.GetService<IRecordMatchingService>();

            // We will block patients, there are no patients that match this record (last name doesn't sound like)
            var patient = new Patient()
            {
                DateOfBirth = DateTime.Parse("1986-10-01"),
                GenderConceptKey = Guid.Parse("094941e9-a3db-48b5-862c-bc289bd7f86c"),
                Identifiers = new System.Collections.Generic.List<Core.Model.DataTypes.EntityIdentifier>()
                {
                    new Core.Model.DataTypes.EntityIdentifier(new AssigningAuthority("HIN", "Health Insurance", "1.2.3.4.56"), "993642-49382938-1986S")
                },
                Names = new System.Collections.Generic.List<Core.Model.Entities.EntityName>()
                {
                    new Core.Model.Entities.EntityName(NameUseKeys.OfficialRecord, "Smit", "Chloey")
                }
            };
            patient = patient.LoadConcepts();
            Assert.AreEqual(2, matchService.Block<Patient>(patient, "test.dob_and_gender_no_class").Count());
        }
    }
}
