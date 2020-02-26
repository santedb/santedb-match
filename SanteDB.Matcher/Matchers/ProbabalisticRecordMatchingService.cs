/*
 * Copyright 2015-2019 Mohawk College of Applied Arts and Technology
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
 * User: JustinFyfe
 * Date: 2019-1-22
 */
using SanteDB.Core;
using SanteDB.Core.Model;
using SanteDB.Core.Model.Query;
using SanteDB.Core.Model.Roles;
using SanteDB.Core.Services;
using SanteDB.Matcher.Configuration;
using SanteDB.Matcher.Exceptions;
using SanteDB.Matcher.Transforms;
using System;
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
    public class ProbabalisticRecordMatchingService : BaseRecordMatchingService
    {

        /// <summary>
        /// Probabalistic matching service
        /// </summary>
        public override string ServiceName => "SanteMatch Probabalistic Matching Service";

        // Transforms loaded by this matcher
        static Dictionary<String, IDataTransformer> s_transforms = new Dictionary<string, IDataTransformer>();

        /// <summary>
        /// Initializes the transform filters 
        /// </summary>
        static ProbabalisticRecordMatchingService()
        {
            foreach (var t in typeof(BaseRecordMatchingService).GetTypeInfo().Assembly.ExportedTypes.Where(t => typeof(IDataTransformer).GetTypeInfo().IsAssignableFrom(t.GetTypeInfo()) && !t.GetTypeInfo().IsAbstract))
            {
                var idt = Activator.CreateInstance(t) as IDataTransformer;
                s_transforms.Add(idt.Name, idt);
            }
        }

        /// <summary>
        /// Classify the records using the specified configuration
        /// </summary>
        public override IEnumerable<IRecordMatchResult<T>> Classify<T>(T input, IEnumerable<T> blocks, string configurationName)
        {
            try
            {

                if (EqualityComparer<T>.Default.Equals(default(T), input)) throw new ArgumentNullException(nameof(input), "Input classifier is required");

                // Get configuration if specified
                var config = ApplicationServiceContext.Current.GetService<IRecordMatchingConfigurationService>().GetConfiguration(configurationName);
                config = (config as MatchConfigurationCollection)?.Configurations.FirstOrDefault(o => o.Target.Any(t => typeof(T).GetTypeInfo().IsAssignableFrom(t.ResourceType.GetTypeInfo()))) ?? config;
                if (config == null || !(config is MatchConfiguration))
                    throw new InvalidOperationException($"Configuration {config?.GetType().Name ?? "null"} is not compatible with this provider");

                var strongConfig = config as MatchConfiguration;
                if (!strongConfig.Target.Any(t => t.ResourceType.GetTypeInfo().IsAssignableFrom(typeof(T).GetTypeInfo())))
                    throw new InvalidOperationException($"Configuration {strongConfig.Name} doesn't appear to contain any reference to {typeof(T).FullName}");

                return blocks.Select(b => this.ClassifyInternal(input, b, strongConfig.Classification, strongConfig.MatchThreshold, strongConfig.NonMatchThreshold));
            }
            catch (Exception e)
            {
                this.m_tracer.TraceError("Error classifying {0} with configuration {1} : {2}", input, configurationName, e.Message);
                throw new MatchingException($"Error classifying {input} with configuration {configurationName}", e);
            }
        }

        /// <summary>
        /// Classifies the individual record against the input and score it based on similarity rules
        /// </summary>
        /// <typeparam name="T">The type of record being classified</typeparam>
        /// <param name="input">The input being classified</param>
        /// <param name="block">The block which is being classified</param>
        /// <param name="vectors">The match vectors to classify on</param>
        /// <returns>The match classification</returns>
        private IRecordMatchResult<T> ClassifyInternal<T>(T input, T block, List<MatchVector> vectors, double matchThreshold, double nonMatchThreshold) where T : IdentifiedData
        {
            try
            {
                var vectorResult = vectors.AsParallel().AsOrdered().WithDegreeOfParallelism(2).Select(v =>
                {
                    this.m_tracer.TraceVerbose("Initializing vector {0}", v);
                    // Initialize the weights and such for the vector
                    v.Initialize();
                    var vectorScore = 0.0d; // The score that this vector receives 

                    var selectorExpression = QueryExpressionParser.BuildPropertySelector<T>(v.Property) as LambdaExpression;
                    object aValue = selectorExpression.Compile().DynamicInvoke(input),
                        bValue = selectorExpression.Compile().DynamicInvoke(block);
                    var defaultInstance = selectorExpression.ReturnType.GetTypeInfo().DeclaredConstructors.Any(c => c.GetParameters().Length == 0) ?
                        Activator.CreateInstance(selectorExpression.ReturnType) :
                        null;

                    bool skip = aValue == null ||
                        bValue == null ||
                        (aValue as IdentifiedData)?.SemanticEquals(defaultInstance) == true ||
                        (bValue as IdentifiedData)?.SemanticEquals(defaultInstance) == true;
                    // Either value null?
                    if (skip)
                    {
                        switch (v.WhenNull)
                        {
                            case MatchVectorNullBehavior.Disqualify:
                                vectorScore = -Int32.MaxValue;
                                break;
                            case MatchVectorNullBehavior.Ignore:
                                vectorScore = 0.0;
                                break;
                            case MatchVectorNullBehavior.Match:
                                vectorScore = v.MatchWeight;
                                break;
                            case MatchVectorNullBehavior.NonMatch:
                                vectorScore = v.NonMatchWeight;
                                break;
                        }
                        this.m_tracer.TraceVerbose("Match vector ({0}) was determined null and assigned score of {1}", v, vectorScore);
                    }
                    else
                    {
                        vectorScore = this.ExecuteAssertion(v.Assertion, aValue, bValue) ? v.MatchWeight : v.NonMatchWeight;

                        // Is there a measure applied?
                        if (v.Measure != null)
                            vectorScore *= (double)this.ExecuteTransform(v.Measure, ref aValue, ref bValue);
                        this.m_tracer.TraceVerbose("Match vector ({0}) was scored against input as {1}", v, vectorScore);

                    }

                    return new VectorResult(v.Property, v.M, v.MatchWeight, vectorScore, !skip, aValue, bValue);
                }).ToList();

                var score = vectorResult.Sum(v => v.Score);
                var retVal = new MatchResult<T>(block, score, score > matchThreshold ? RecordMatchClassification.Match : score <= nonMatchThreshold ? RecordMatchClassification.NonMatch : RecordMatchClassification.Probable);
                retVal.Vectors.AddRange(vectorResult);
                return retVal;
            }
            catch (Exception e)
            {
                this.m_tracer.TraceError("Error classifying result set (mt={0}, nmt={1}) - {2}", matchThreshold, nonMatchThreshold, e.Message);
                throw new MatchingException($"Error classifying result set", e);
            }
        }

        /// <summary>
        /// Execute an assertion
        /// </summary>
        /// <param name="assertion">The assertion to execute</param>
        /// <param name="aValue">The value of the A value</param>
        /// <param name="bValue">The value of the B value</param>
        /// <returns>True if the assertion passes, false if not</returns>
        private bool ExecuteAssertion(MatchVectorAssertion assertion, object aValue, object bValue)
        {
            try
            {
                // Apply all transforms first
                object a = aValue, b = bValue;
                object scope = null;
                foreach (var xform in assertion.Transforms)
                    scope = this.ExecuteTransform(xform, ref a, ref b) ?? scope;

                var retVal = true;
                switch (assertion.Operator)
                {
                    case BinaryOperatorType.AndAlso: // There are sub-assertions 
                        {
                            foreach (var asrt in assertion.Assertions)
                                retVal &= this.ExecuteAssertion(asrt, a, b);
                            break;
                        }
                    case BinaryOperatorType.OrElse:
                        {
                            foreach (var asrt in assertion.Assertions)
                                retVal |= this.ExecuteAssertion(asrt, a, b);
                            break;
                        }
                    case BinaryOperatorType.Equal:
                        if (assertion.ValueSpecified)
                            retVal = scope.Equals(assertion.Value);
                        else
                            retVal = a.Equals(b);
                        break;
                    case BinaryOperatorType.NotEqual:
                        if (assertion.ValueSpecified)
                            retVal = !scope.Equals(assertion.Value);
                        else
                            retVal = !a.Equals(b);
                        break;
                    case BinaryOperatorType.GreaterThan:
                        retVal = (double)scope > assertion.Value;
                        break;
                    case BinaryOperatorType.GreaterThanOrEqual:
                        retVal = (double)scope >= assertion.Value;
                        break;
                    case BinaryOperatorType.LessThan:
                        retVal = (double)scope < assertion.Value;
                        break;
                    case BinaryOperatorType.LessThanOrEqual:
                        retVal = (double)scope <= assertion.Value;
                        break;
                    default:
                        throw new InvalidOperationException($"Operator type {assertion.Operator} is not supported in this context");
                }

#if DEBUG
                this.m_tracer.TraceVerbose("ASSERT - {0} {1} {2} => {3}", aValue, assertion.Operator, bValue, retVal);
#endif
                return retVal;
            }
            catch (Exception e)
            {
                this.m_tracer.TraceError("Error executing assertion {0} - {1}", e);
                throw new MatchingException($"Error executing assertion {assertion}", e);
            }
        }

        /// <summary>
        /// Executes a transform
        /// </summary>
        /// <param name="transform">The transform to run</param>
        /// <param name="aValue">The A value to pass to the transform</param>
        /// <param name="bValue">The B value to pass to the transform</param>
        /// <returns>The result of the transform to be assigned back to the scope (or to the values in the case of unary transforms)</returns>
        private object ExecuteTransform(MatchTransform transform, ref object aValue, ref object bValue)
        {
            try
            {
                IDataTransformer transformer = null;
                if (s_transforms.TryGetValue(transform.Name, out transformer))
                {
                    if (transformer is IUnaryDataTransformer)
                    {
#if DEBUG
                        this.m_tracer.TraceVerbose("Will apply unary transform {0} on {1}", transform, aValue);
#endif
                        aValue = (transformer as IUnaryDataTransformer).Apply(aValue, transform.Parameters.ToArray());
#if DEBUG
                        this.m_tracer.TraceVerbose("Will apply unary transform {0} on {1}", transform, bValue);
#endif
                        bValue = (transformer as IUnaryDataTransformer).Apply(bValue, transform.Parameters.ToArray());
                        return null; // No result for unary transforms
                    }
                    else if (transformer is IBinaryDataTransformer)
                    {
#if DEBUG
                        this.m_tracer.TraceVerbose("Will apply binary transform {0} on {1} and {2}", transform, aValue, bValue);
#endif
                        return (transformer as IBinaryDataTransformer).Apply(aValue, bValue, transform.Parameters.ToArray());
                    }
                    else
                        throw new InvalidOperationException("Invalid type of transform");
                }
                else
                    throw new KeyNotFoundException($"Transform {transform.Name} is not registered");
            }
            catch(Exception e)
            {
                this.m_tracer.TraceError("Error executing {0} on {1} and {2} - {3}", transform, aValue, bValue, e.Message);
                throw new MatchingException($"Error executing {transform} on {aValue} and {bValue}", e);
            }
        }

        /// <summary>
        /// Block and match records based on their match result
        /// </summary>
        public override IEnumerable<IRecordMatchResult<T>> Match<T>(T input, string configurationName)
        {
            return this.Classify(input, base.Block(input, configurationName), configurationName);
        }
    }
}
