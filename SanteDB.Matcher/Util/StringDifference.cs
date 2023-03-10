/*
 * Copyright (C) 2021 - 2023, SanteSuite Inc. and the SanteSuite Contributors (See NOTICE.md for full copyright notices)
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
 * Date: 2023-3-10
 */
using System;
using System.Linq;

namespace SanteDB.Matcher.Util
{
    /// <summary>
    /// Represents a utility for string differences
    /// </summary>
    public static class StringDifference
    {

        /// <summary>
        /// Calculate the levenshtein distance between strings
        /// </summary>
        public static double Levenshtein(this String source, String target)
        {

            if (string.IsNullOrEmpty(source))
            {
                return string.IsNullOrEmpty(target) ? 0 : target.Length;
            }

            if (string.IsNullOrEmpty(target))
            {
                return string.IsNullOrEmpty(source) ? 0 : source.Length;
            }

            source = source.ToLowerInvariant().Trim();
            target = target.ToLowerInvariant().Trim();

            var sourceLength = source.Length;
            var targetLength = target.Length;

            var distance = new int[sourceLength + 1, targetLength + 1];

            for (var i = 0; i <= sourceLength; distance[i, 0] = i++) ;

            for (var j = 0; j <= targetLength; distance[0, j] = j++) ;

            for (var i = 1; i <= sourceLength; i++)
            {
                for (var j = 1; j <= targetLength; j++)
                {
                    var cost = (target[j - 1] == source[i - 1]) ? 0 : 1;
                    distance[i, j] = Math.Min(Math.Min(distance[i - 1, j] + 1, distance[i, j - 1] + 1), distance[i - 1, j - 1] + cost);
                }
            }

            return distance[sourceLength, targetLength];

        }

        /// <summary>
        /// Calculates the sorensen dice coefficient between two strings
        /// </summary>
        public static double SorensenDice(this String source, String target)
        {
            // First we want to turn the source and target into bigrams
            string[] sourceBigram = Enumerable.Range(0, source.Length - 1).Select(o => source.Substring(o, 2)).Distinct().ToArray(),
                targetBigram = Enumerable.Range(0, target.Length - 1).Select(o => target.Substring(o, 2)).Distinct().ToArray();

            // Insertsect the bigram sets
            var intersect = sourceBigram.Intersect(targetBigram);
            return (2.0 * intersect.Count()) / (sourceBigram.Length + targetBigram.Length);


        }

        /// <summary>
        /// Returns the Jaro-Winkler proximity of two strings 
        /// </summary>
        public static double JaroWinkler(this String source, String target)
        {
            int lLen1 = source.Length;
            int lLen2 = target.Length;
            if (lLen1 == 0)
            {
                return lLen2 == 0 ? 1.0 : 0.0;
            }

            int lSearchRange = Math.Max(0, Math.Max(lLen1, lLen2) / 2 - 1);

            // default initialized to false
            bool[] lMatched1 = new bool[lLen1];
            bool[] lMatched2 = new bool[lLen2];

            int lNumCommon = 0;
            for (int i = 0; i < lLen1; ++i)
            {
                int lStart = Math.Max(0, i - lSearchRange);
                int lEnd = Math.Min(i + lSearchRange + 1, lLen2);
                for (int j = lStart; j < lEnd; ++j)
                {
                    if (lMatched2[j])
                    {
                        continue;
                    }

                    if (source[i] != target[j])
                    {
                        continue;
                    }

                    lMatched1[i] = true;
                    lMatched2[j] = true;
                    ++lNumCommon;
                    break;
                }
            }
            if (lNumCommon == 0)
            {
                return 0.0;
            }

            int lNumHalfTransposed = 0;
            int k = 0;
            for (int i = 0; i < lLen1; ++i)
            {
                if (!lMatched1[i])
                {
                    continue;
                }

                while (!lMatched2[k])
                {
                    ++k;
                }

                if (source[i] != target[k])
                {
                    ++lNumHalfTransposed;
                }

                ++k;
            }
            // System.Diagnostics.Debug.WriteLine("numHalfTransposed=" + numHalfTransposed);
            int lNumTransposed = lNumHalfTransposed / 2;

            // System.Diagnostics.Debug.WriteLine("numCommon=" + numCommon + " numTransposed=" + numTransposed);
            double lNumCommonD = lNumCommon;
            double lWeight = (lNumCommonD / lLen1
                             + lNumCommonD / lLen2
                             + (lNumCommon - lNumTransposed) / lNumCommonD) / 3.0;

            if (lWeight <= 0.7d)
            {
                return lWeight;
            }

            int lMax = Math.Min(4, Math.Min(target.Length, source.Length));
            int lPos = 0;
            while (lPos < lMax && source[lPos] == target[lPos])
            {
                ++lPos;
            }

            if (lPos == 0)
            {
                return lWeight;
            }

            return lWeight + 0.1 * lPos * (1.0 - lWeight);
        }

        /// <summary>
        /// Returns the similarty to 
        /// </summary>
        public static double SimilarityTo(this String source, String target)
        {

            return source.JaroWinkler(target);
        }

        /// <summary>
        /// Returns the similarty to two string using levenshtein
        /// </summary>
        public static double LevenshteinSimilarityTo(this String source, String target)
        {

            return 1.0 - ((float)source.Levenshtein(target) / Enumerable.Max(new double[] { (double)target.Length, (double)source.Length }));
        }

    }
}
