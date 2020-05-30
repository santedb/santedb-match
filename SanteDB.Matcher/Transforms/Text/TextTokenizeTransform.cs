using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SanteDB.Matcher.Transforms.Text
{
    /// <summary>
    /// Takes a name and tokenizes it
    /// </summary>
    public class TextTokenizeTransform : IUnaryDataTransformer
    {
        /// <summary>
        /// Get the name of the transformer
        /// </summary>
        public string Name => "tokenize";

        /// <summary>
        /// Apply the tokenizer
        /// </summary>
        public object Apply(object input, params object[] parms)
        {
            if (parms.Length != 1)
                throw new ArgumentException("Require token delimiter parameter");

            // Processed tokens for the name
            List<String> tokens = new List<string>() { (String)input };
            foreach (char delim in (String)parms[0])
            {
                tokens = tokens.SelectMany(o => o.Split(delim)).ToList();
            }
            return tokens;
        }
    }
}
