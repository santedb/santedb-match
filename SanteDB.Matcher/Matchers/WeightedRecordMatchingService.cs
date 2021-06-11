/*
 * Copyright (C) 2019 - 2021, Fyfe Software Inc. and the SanteSuite Contributors (See NOTICE.md)
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
 * Date: 2021-2-9
 */
using SanteDB.Core;
using SanteDB.Core.Model;
using SanteDB.Core.Model.Query;
using SanteDB.Core.Model.Roles;
using SanteDB.Core.Services;
using SanteDB.Matcher.Configuration;
using SanteDB.Matcher.Definition;
using SanteDB.Matcher.Exceptions;
using SanteDB.Matcher.Transforms;
using SanteDB.Matcher.Util;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SanteDB.Matcher.Matchers
{
    /// <summary>
    /// Represents a probabalistic record matching service
    /// </summary>
    [ServiceProvider("SanteMatch Probabalistic Match Service")]
    public class WeightedRecordMatchingService : BaseRecordMatchingService
    {
        
        /// <summary>
        /// Probabalistic matching service
        /// </summary>
        public override string ServiceName => "SanteMatch Probabalistic Matching Service";

        // Configurations
        static ConcurrentDictionary<String, MatchConfiguration> s_configurations = new ConcurrentDictionary<string, MatchConfiguration>();

        /// <summary>
        /// Classify the records using the specified configuration
        /// </summary>
        public override IEnumerable<IRecordMatchResult<T>> Classify<T>(T input, IEnumerable<T> blocks, string configurationName)
        {
            try
            {

                if (EqualityComparer<T>.Default.Equals(default(T), input)) throw new ArgumentNullException(nameof(input), "Input classifier is required");
                var strongConfig = this.GetConfiguration<T>(configurationName);
                if (!strongConfig.Target.Any(t => t.ResourceType.IsAssignableFrom(typeof(T))))
                    throw new InvalidOperationException($"Configuration {strongConfig.Name} doesn't appear to contain any reference to {typeof(T).FullName}");

                return blocks.AsParallel().Select(b => this.ClassifyInternal(input, b, strongConfig.Scoring, strongConfig.ClassificationMethod, strongConfig.MatchThreshold, strongConfig.NonMatchThreshold)).ToList();
            }
            catch (Exception e)
            {
                this.m_tracer.TraceError("Error classifying {0} with configuration {1} : {2}", input, configurationName, e.Message);
                throw new MatchingException($"Error classifying {input} with configuration {configurationName}", e);
            }
        }

        /// <summary>
        /// Get the specified configuration
        /// </summary>
        private MatchConfiguration GetConfiguration<T>(string configurationName) where T : IdentifiedData
        {
#if DEBUG
            var config = ApplicationServiceContext.Current.GetService<IRecordMatchingConfigurationService>().GetConfiguration(configurationName);
            MatchConfiguration retVal = (config as MatchConfigurationCollection)?.Configurations.FirstOrDefault(o => o.Target.Any(t => typeof(T).IsAssignableFrom(t.ResourceType))) ?? config as MatchConfiguration;
#else
            if (!s_configurations.TryGetValue(configurationName, out MatchConfiguration retVal))
            {
                var config = ApplicationServiceContext.Current.GetService<IRecordMatchingConfigurationService>().GetConfiguration(configurationName);
                retVal = (config as MatchConfigurationCollection)?.Configurations.FirstOrDefault(o => o.Target.Any(t => typeof(T).IsAssignableFrom(t.ResourceType))) ?? config as MatchConfiguration;
                if (retVal == null)
                    throw new InvalidOperationException($"Configuration {config?.GetType().Name ?? "null"} is not compatible with this provider");
                s_configurations.TryAdd(configurationName, retVal);
            }
#endif
            return retVal;
        }

        /// <summary>
        /// Classifies the individual record against the input and score it based on similarity rules
        /// </summary>
        /// <typeparam name="T">The type of record being classified</typeparam>
        /// <param name="input">The input being classified</param>
        /// <param name="block">The block which is being classified</param>
        /// <param name="attributes">The match attributes to classify on</param>
        /// <returns>The match classification</returns>
        private IRecordMatchResult<T> ClassifyInternal<T>(T input, T block, List<MatchAttribute> attributes, ThresholdEvaluationType evaluationType, double matchThreshold, double nonMatchThreshold) where T : IdentifiedData
        {
            try
            {

                var attributeResult = attributes.Select(v =>
                {
                    this.m_tracer.TraceVerbose("Initializing attribute {0}", v);
                    // Initialize the weights and such for the attribute
                    v.Initialize();
                    var attributeScores = v.GetPropertySelectors<T>().Select(selector =>
                    {
                        Func<T, dynamic> selectorExpression = (Func<T, dynamic>)selector.Value;
                        object aValue = selectorExpression(input),
                            bValue = selectorExpression(block);
                        var defaultInstance = selectorExpression.Method.ReturnType.GetConstructors().Any(c => c.GetParameters().Length == 0) ?
                            Activator.CreateInstance(selectorExpression.Method.ReturnType) :
                            null;
                        return AssertionUtil.ExecuteAssertion(selector.Key, v.Assertion, v, aValue, bValue);
                    });
                    var bestScore = attributeScores.OrderByDescending(o => o.CalculatedScore).FirstOrDefault();

                    if (bestScore == null || !bestScore.CalculatedScore.HasValue)
                        return null;
                    else
                        return new MatchVector(v, v.Id ?? bestScore.PropertyName, v.M, v.MatchWeight, bestScore.CalculatedScore.Value, bestScore.Evaluated, bestScore.A, bestScore.B);
                }).OfType<MatchVector>().ToList();

                // Throw out attributes which are dependent however the dependent attribute was unsuccessful
                // So if for example: If the scoring for CITY is only counted when STATE is successful, but STATE was 
                // unsuccessful, we want to exclude CITY.
                attributeResult.RemoveAll(o => o.Attribute.When.Any(w => attributeResult.First(r => r.Attribute.Id == w.AttributeRef).Score < 0)); // Remove all failed attributes
                var score = attributeResult.Sum(v => v.Score);

                // The attribute scores which are produced will be from SUM(NonMatchWeight) .. SUM(MatchWeight)
                double maxScore = attributeResult.Sum(o => o.Attribute.MatchWeight),
                    minScore = attributeResult.Sum(o => o.Attribute.NonMatchWeight);
                // This forms a number line between -MIN .. MAX , our probability is the distance that our score 
                // is on that line, for example: -30.392 .. 30.392 
                // Then the strength is 0.5 of a score of 0 , and 1.0 for a score of 30.392
                var strength = (score + -minScore) / (maxScore + -minScore);
                if (Double.IsNaN(strength))
                    strength = 0;
                RecordMatchClassification classification = RecordMatchClassification.NonMatch;
                if (evaluationType == ThresholdEvaluationType.AbsoluteScore)
                    classification = score > matchThreshold ? RecordMatchClassification.Match : score <= nonMatchThreshold ? RecordMatchClassification.NonMatch : RecordMatchClassification.Probable;
                else 
                    classification = strength > matchThreshold ? RecordMatchClassification.Match : strength <= nonMatchThreshold ? RecordMatchClassification.NonMatch : RecordMatchClassification.Probable;
                var retVal = new MatchResult<T>(block, score, strength, classification, RecordMatchMethod.Weighted, attributeResult);


                return retVal;
            }
            catch (Exception e)
            {
                this.m_tracer.TraceError("Error classifying result set (mt={0}, nmt={1}) - {2}", matchThreshold, nonMatchThreshold, e.Message);
                throw new MatchingException($"Error classifying result set", e);
            }
        }

        /// <summary>
        /// Block and match records based on their match result
        /// </summary>
        public override IEnumerable<IRecordMatchResult<T>> Match<T>(T input, string configurationName, IEnumerable<Guid> ignoreList)
        {
            var result = this.Classify(input, base.Block(input, configurationName, ignoreList), configurationName);
            return result;
        }

    }
}
