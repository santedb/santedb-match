using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SanteDB.Matcher.Model;
using SanteDB.Matcher.Transforms;
using SanteDB.Matcher.Transforms.Date;

namespace SanteDB.Matcher.Test
{
    [TestClass]
    public class TransformerTests
    {

        /// <summary>
        /// The transform tool should load transformers
        /// </summary>
        [TestMethod]
        public void ShouldCreateTransformer()
        {
            var instance = TransformerFactory.Current.CreateTransformer("date_extract");
            Assert.IsInstanceOfType(instance, typeof(DateExtractTransform));
        }

    }
}
