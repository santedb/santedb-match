﻿/*
 * Copyright (C) 2021 - 2025, SanteSuite Inc. and the SanteSuite Contributors (See NOTICE.md for full copyright notices)
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
 * Date: 2023-6-21
 */
using Phonix;
using SanteDB.Core;
using SanteDB.Core.Services;
using SanteDB.Matcher.Configuration;
using SanteDB.Matcher.Util;
using System;
using System.Linq;
using System.Text.RegularExpressions;

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
        public static bool Approx(this String me, String other)
        {
            var config = ApplicationServiceContext.Current.GetService<IConfigurationManager>().GetSection<ApproximateMatchingConfigurationSection>();
            var isApprox = false;

            if (config != null)
            {
                foreach (var m in config.ApproxSearchOptions)
                {
                    if (m is ApproxPhoneticOption phonetic && m.Enabled)
                    {
                        var minSpec = phonetic.MinSimilarity;
                        if (!phonetic.MinSimilaritySpecified)
                        {
                            minSpec = 1.0f;
                        }

                        switch (phonetic.Algorithm)
                        {
                            case ApproxPhoneticOption.PhoneticAlgorithmType.Auto:
                            case ApproxPhoneticOption.PhoneticAlgorithmType.DoubleMetaphone:
                                isApprox &= me.DoubleMetaphone().Levenshtein(other.DoubleMetaphone()) >= minSpec;
                                break;
                            case ApproxPhoneticOption.PhoneticAlgorithmType.Metaphone:
                                isApprox = me.Metaphone().Levenshtein(other.Metaphone()) >= minSpec;
                                break;
                            case ApproxPhoneticOption.PhoneticAlgorithmType.Soundex:
                                isApprox = me.Soundex().Levenshtein(other.Soundex()) >= minSpec;
                                break;
                        }
                    }
                    else if (m is ApproxDifferenceOption difference && m.Enabled)
                    {
                        isApprox &= me.Levenshtein(other) < difference.MaxDifference;
                    }
                    else if (m is ApproxPatternOption pattern && m.Enabled)
                    {
                        var regex = new Regex(other.Replace("?", ".?").Replace("*", ".*?"), pattern.IgnoreCase ? RegexOptions.IgnoreCase : RegexOptions.None);
                        isApprox &= regex.IsMatch(me);
                    }
                }
            }

            return isApprox;
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
            return me.SoundsLike(other, "metaphone");
        }

        /// <summary>
        /// Returns true if <paramref name="other"/> sounds like <paramref name="me"/> using <paramref name="algorithmName"/>
        /// </summary>
        public static bool SoundsLike(this String me, String other, String algorithmName)
        {
            switch (algorithmName)
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
        /// Determines of other is an alias of me and the strength of the alias
        /// </summary>
        public static double Alias(this String me, String other)
        {
            return (ApplicationServiceContext.Current.GetService<IAliasProvider>()?.GetAlias(me)?.FirstOrDefault(o => o.Alias.Equals(other, StringComparison.CurrentCultureIgnoreCase)).Relevance).GetValueOrDefault();
        }
    }
}
