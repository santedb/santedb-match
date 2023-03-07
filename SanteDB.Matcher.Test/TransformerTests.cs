/*
 * Copyright (C) 2021 - 2022, SanteSuite Inc. and the SanteSuite Contributors (See NOTICE.md for full copyright notices)
 * Copyright (C) 2019 - 2021, Fyfe Software Inc. and the SanteSuite Contributors
 * Portions Copyright (C) 2015-2018 Mohawk College of Applied Arts and Technology
 * 
 * Licensed under the Apache License, Version 2.0 (the "License"); you 
 * may not use this file except in compliance with the License. You may 
 * obtain a copy of the License at 
 * 
 * http://www.apache.org/licenses/LICENSE-2.0 
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the 
 * License for the specific language governing permissions and limitations under 
 * the License.
 * 
 * User: fyfej
 * Date: 2022-5-30
 */
using NUnit.Framework;
using SanteDB.Matcher.Transforms;
using SanteDB.Matcher.Transforms.Date;
using SanteDB.Matcher.Transforms.Text;
using System.Diagnostics.CodeAnalysis;

namespace SanteDB.Matcher.Test
{
    [ExcludeFromCodeCoverage]
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
        /// Tests the jaro-winkler transfrm
        /// </summary>
        [Test]
        public void TestJaroWinklerTransform()
        {
            var instance = TransformerFactory.Current.CreateTransformer("jaro_winkler");
            Assert.IsAssignableFrom<JaroWinklerTransform>(instance);
            Assert.IsInstanceOf<IBinaryDataTransformer>(instance);

            if (instance is IBinaryDataTransformer binary)
            {
                var distance = binary.Apply("Oscar", "Ofsar");
                Assert.LessOrEqual(0.1d, (double)distance);
                distance = binary.Apply("Kimeu", "Cherop");
            }
        }

        /// <summary>
        /// Tests the sorensen dice
        /// </summary>
        [Test]
        public void TestSorensenDiceTransform()
        {
            var instance = TransformerFactory.Current.CreateTransformer("sorensen_dice");
            Assert.IsAssignableFrom<SorensenDiceTransform>(instance);
            Assert.IsInstanceOf<IBinaryDataTransformer>(instance);

            // Apply
            if (instance is IBinaryDataTransformer binary)
            {
                var distance = binary.Apply("Night", "Nacht");
                Assert.AreEqual(0.25d, distance);
                distance = binary.Apply("Testing", "Testong");
                Assert.AreEqual(2.0 / 3.0, distance);
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
