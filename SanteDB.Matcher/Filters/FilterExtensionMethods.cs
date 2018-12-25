/*
 * Copyright 2015-2018 Mohawk College of Applied Arts and Technology
 *
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
 * User: justin
 * Date: 2018-12-25
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Phonix;
using SanteDB.Core;
using SanteDB.Core.Interfaces;
using SanteDB.Core.Services;
using SanteDB.Matcher.Util;

namespace SanteDB.Matcher.Filters
{
    /// <summary>
    /// Provides filtering extension methods
    /// </summary>
    /// <remarks>
    /// This class exists to support the query extension filters
    /// </remarks>
    public static class FilterExtensionMethods
    {

        /// <summary>
        /// Represents a metaphone coding
        /// </summary>
        public static string Metaphone(this String me)
        {
            return new Metaphone(4).BuildKey(me);
        }

        /// <summary>
        /// Represents metaphone encoding with specificity
        /// </summary>
        public static string Metaphone(this String me, int maxLength)
        {
            return new Metaphone(maxLength).BuildKey(me);
        }

        /// <summary>
        /// Returns a double metaphone code
        /// </summary>
        public static string DoubleMetaphone(this String me)
        {
            return new DoubleMetaphone().BuildKey(me);
        }

        /// <summary>
        /// Returns a soundex code
        /// </summary>
        public static string Soundex(this String me)
        {
            return new Soundex(true).BuildKey(me);
        }

        /// <summary>
        /// Returns true if <paramref name="me"/> sounds like <paramref name="other"/>
        /// </summary>
        public static bool SoundsLike(this String me, String other)
        {
            var alg = ApplicationServiceContext.Current.GetService<IPhoneticAlgorithmHandler>();
            return alg?.GenerateCode(me) == alg?.GenerateCode(other);
        }

        /// <summary>
        /// Returns true if <paramref name="other"/> sounds like <paramref name="me"/> using <paramref name="algorithmName"/>
        /// </summary>
        public static bool SoundsLike(this String me, String other, String algorithmName)
        {
            switch(algorithmName)
            {
                case "metaphone":
                    return new Metaphone(4).IsSimilar(new string[] { me, other });
                case "dmetaphone":
                    return new DoubleMetaphone().IsSimilar(new string[] { me, other });
                case "soundex":
                    return new Soundex().IsSimilar(new string[] { me, other });
                default:
                    throw new NotSupportedException($"Algorithm {algorithmName} is not supported");
            }
        }

        /// <summary>
        /// Returns true if <paramref name="me"/> sounds like <paramref name="other"/>
        /// </summary>
        public static double PhoneticDifference(this String me, String other)
        {
            String meCode = me.Metaphone(4),
                otherCode = other.Metaphone(4);
            return meCode.Levenshtein(otherCode);
        }

        /// <summary>
        /// Returns true if <paramref name="other"/> sounds like <paramref name="me"/> using <paramref name="algorithmName"/>
        /// </summary>
        public static double PhoneticDifference(this String me, String other, String algorithmName)
        {
            String meCode = null, otherCode = null;
            switch (algorithmName)
            {
                case "metaphone":
                    meCode = new Metaphone(4).BuildKey(me);
                    otherCode = new Metaphone(4).BuildKey(other);
                    break;
                case "dmetaphone":
                    meCode = new DoubleMetaphone().BuildKey(me);
                    otherCode = new DoubleMetaphone().BuildKey(other);
                    break;
                case "soundex":
                    meCode = new Metaphone(4).BuildKey(me);
                    otherCode = new Metaphone(4).BuildKey(other);
                    break;
                default:
                    throw new NotSupportedException($"Algorithm {algorithmName} is not supported");
            }

            return meCode.Levenshtein(otherCode);
        }
        /// <summary>
        /// Calculate the difference between two dates
        /// </summary>
        public static TimeSpan Difference(this DateTime source, DateTime target)
        {
            return source.Subtract(target).Duration();
        }

        /// <summary>
        /// Calculate the difference between two dates
        /// </summary>
        public static TimeSpan Difference(this DateTimeOffset source, DateTimeOffset target)
        {
            return source.Subtract(target).Duration();
        }

        /// <summary>
        /// Gets the first characters
        /// </summary>
        public static String Substr(this String me, int start)
        {
            if (start > me.Length)
                start = me.Length - 1;
            return me.Substring(start); 
        }

        /// <summary>
        /// Gets the first characters
        /// </summary>
        public static String Substr(this String me, int start, int length)
        {
            if (start + length> me.Length)
                length = me.Length - start;
            if (start > me.Length)
                start = me.Length - 1;
            return me.Substring(start, length);
        }

        /// <summary>
        /// Determines of other is an alias of me and the strength of the alias
        /// </summary>
        public static double Alias(this String me, String other)
        {
            return (ApplicationServiceContext.Current.GetService<IAliasProvider>()?.GetAlias(me)?.FirstOrDefault(o => o.Alias.Equals(other, StringComparison.CurrentCultureIgnoreCase)).Relevance).GetValueOrDefault();
        }
    }
}
