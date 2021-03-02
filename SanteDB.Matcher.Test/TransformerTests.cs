using System;
using NUnit.Framework;
using SanteDB.Matcher.Model;
using SanteDB.Matcher.Transforms;
using SanteDB.Matcher.Transforms.Date;
using SanteDB.Matcher.Transforms.Text;

namespace SanteDB.Matcher.Test
{
    [TestFixture(Category = "Matching")]
    public class TransformerTests
    {

        /// <summary>
        /// The transform tool should load transformers
        /// </summary>
        [Test]
        public void ShouldCreateTransformer()
        {
            var instance = TransformerFactory.Current.CreateTransformer("date_extract");
            Assert.IsAssignableFrom<DateExtractTransform>(instance);
        }

        /// <summary>
        /// Tests the sorensen dice
        /// </summary>
        [Test]
        public void TestSorensenDiceTransform()
        {
             var instance = TransformerFactory.Current.CreateTransformer("sorensen_dice");
            Assert.IsAssignableFrom<SorensenDiceTransform>(instance);
            Assert.IsAssignableFrom<IBinaryDataTransformer>(instance);

            // Apply
            if (instance is IBinaryDataTransformer binary) {
                var distance = binary.Apply("Night", "Nacht");
                Assert.AreEqual(0.25d, distance);
                distance = binary.Apply("Testing", "Testong");
                Assert.AreEqual(2.0/3.0, distance);
                distance = binary.Apply("This is a long string", "This is shorter");
                Assert.AreEqual(0.41379310344827586d, distance);
                distance = binary.Apply("Bahamas", "Bahamas, The");
                Assert.AreEqual(0.70588235294117652d, distance);
                distance = binary.Apply("Zambia", "Gambia");
                Assert.AreEqual(0.8d, distance);
                distance = binary.Apply("Myanmar/Burma", "Burma");
                Assert.AreEqual(0.53333333333333333, distance);

                distance = binary.Apply("this is not a correct string", "this is a correct string");
                Assert.AreEqual(0.93333333333333333, distance);
            }
        }
    }
}
