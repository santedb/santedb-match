# SanteDB Matcher

This project adds an implementation of a fuzzy matching service to the SanteDB iCDR 
core instance in which it is installed. It provides:

* Services for identifying duplicate records 
* Services for classifying an inbound record against the iCDR's data source
* Query Filters for fuzzy searching using the HDSI query syntax

## Match Process

There are two matchers that are provided in the matching project:

* **DeterministicMatcher** which can detect matches based on HDSI query criteria directly from the database. These matches are not scored, rather they have a probability of 0.0 or 1.0 only.
* **ProbabalisticMatcher** which uses a three stage algorithm to determine matches given a series of rules. The steps for this matcher are as follows:
   1. **Blocking:** In the blocking phase HDSI queries are run against the underlying database and are selected. This is done to reduce the number of records which need to be classified.
   2. **Scoring:** In the scoring stage, a series of transforms and measurements are done on the blocked records (from step 1), and each record is given a score. The score indicates
      the confidence that the record is a match.
   3. **Classification:** In the final stage, the weighted sum of all scores in step 2 are classified as: Match, Probable Match, or Non Match and returned to the caller
    
### Query Filters

In SanteDB, query filters are commonly used in deterministic ways (such as =, <=, ~, etc). SanteDB
1.2 introduces the concept of extended query filters which allow consumers to indicate a transform 
to be performed on data prior to execution. For example, if you wanted to match a Patient's last name
using Soundex instead of "LIKE" you could use:

```
?name.component.value=:(soundex)SMITH
```

Which will match SMITH, SMITHE, SMYTHE, etc.

Query filters are in the format:

**property=:(extension** *|parameters* **)** *operator* *value*

| Filter | Description | Example |
|-|-|-|
| ``` :(date_diff\|otherdate)timespan ``` | Calculates the difference between two dates | ```?dateOfBirth=:(date_diff\|2018-01-01)<1w``` Matches all patients born within one week of 2018-01-01  |
| ```:(substr\|start[,end])otherString``` | Performs a partial match on a sub-portion of the string | ```?identifier.value=:(substr\|0,6)304-304-394``` matches all patients who have an identifier starting with 304-30 |
| ```:(levenshtein\|otherString)distance``` | Calculates the levenshtein difference between the property and input | ```?name.component.value=:(levenshtein\|Jenny)<1``` matches all patients who's name is only one character different than Jenny (Jenn, Jennye but not Jennie) |
| ```:(metaphone[\|specificity])otherString``` | Matches the field based on the metaphone code | ```?name.component.value=:(metaphone)Smith``` matches any name whose metaphone code matches SMITH|
| ```:(dmetaphone)otherString```| Matches the field based on the double-metaphone code | ```?name.component.value=:(dmetaphone)Smith``` matches any name whose double metaphone code matches SMITH|
| ```:(soundex)otherString```| Matches the field based on the SOUNDEX code | ```?name.component.value=:(soundex)Smith``` matches any name whose soundex code matches SMITH|
| ```:(soundslike\|otherString)```| Matches the field based on the currently configured phonetic algorithm handler (lets server decide the algorithm)||
| ```:(phonetic_diff\|otherString[,algorithm])distance``` | Matches a field based on phonetic difference to another code using metaphone (default) or using soundex or dmetaphone| ```?name.component.value=:(phonetic_diff\|SMITH)<2``` Matches any name where the metaphone code is only 1 character different|
| ```:(alias\|otherString)relevance``` | Matches records which are an alias | ```name.component.value=:(alias\|Will)>0``` will match Will, William, and Bill |

## Configuring The Matcher

Both matchers use the same configuration file, with the exception that deterministic matching only uses the blocking section of configuration.

```
<?xml version="1.0" encoding="utf-8"?>
<MatchConfiguration xmlns="http://santedb.org/matcher"
                    id="sample.patient"
                    uThreshold="0.5"
                    mThreshold="0.9">
  <!-- 
    @element target
    @cardinality 1..*
    @summary Identifies the resource(s) that this match configuration is tuned for
    @attribute {string} resource The name of the resource type that is being targeted
  -->
  <target resource="">
    <!--
      @element trigger
      @summary Identifies the trigger(s) on the target resource which initiate automatic duplicate detection
    -->
    <trigger></trigger>
  </target>
  <!-- 
    @element blocking
    @cardinality 0..*
    @summary Controls the blocking criteria for the matching operation
    @attribute {enum} op The operation (or|and) which dictate how multiple blocking elements are handled (either union or intersect)
  -->
  <blocking op="">
    <!-- 
      @element hdsiExpression 
      @cardinality 1..*
      @summary One or more query filters which should be applied to the underlying datasource to filter potential matches
    -->
    <hdsiExpression></hdsiExpression>
  </blocking>
  <!-- 
    @element classification
    @summary Controls the classification/scoring vectors for probabilistic matching
  -->
  <classification>
    <!-- 
      @element vector
      @cardinality 0..*
      @summary Identifies a matching vector which should be used for probabilistic matching
      @attribute {decimal} m The probability that a record where this vector matches is an actual match
      @attribute {decimal} u The uncertainty measure, where this vector will match by pure coincidence
      @attribute {enum} whenNull When the source record's property is null, how should this vector be treated
      @attribute {boolean} required Indicates that this vector MUST fully match or else the entire match is considered false
      @attribute {enum} weightMode For string based vectors, indicates whether outcome of this measure should be the full weight or based on the difference/similarity of the string functions
      @attribute {string} property The name or path to the property this vector is measuring
      @description
      The m and u values can be calculated as follows:
        (u) Take the probability that a person has the same week DOB and is not a match u=(1/52 = 0.019)
        (m) Estimate the probability that pairs will definately match (perfect data is 1.0) this is an estimation. A 95% chance would be represented as m=0.95
        
       The algorithm will then calculate:
       u(match) = 0.019
       m(match) = 0.95
       u(non-match) = 0.981
       m(non-match) = 0.05
       
       f(match) = m(match)/u(match) = 50  << Frequency ratio of matches based on week of birth
       f(non-match) = m(non-match)/u(non-match) = 0.0509
       
       weight for matches: ln(f(match))/ln(2) = 5.64385
       weight for non-matches: ln(f(non-match))/ln(2) = -4.2961
       
       Please note that you can specify either u and m or matchWeight and nonMatchWeight but not both sets
    -->
    <vector m="" u="" whenNull="" required="">
      <!--
        @element assert 
        @cardinality 1..1
        @summary The logical assertion which is applied to the vector which must evaluate to true
        @attribute {enum} op The operation to perform on the measure
        @attribute {any} value The value to compare the output (example: if transform with levenshtein) default is $$other
        @description
        Valid values for @op are:
          eq = Equals (A must exactly equal B after transforming)
          lt = Less than
          lte = Less than equal
          gt = Greater than
          gte = Greater than equal
          ne = Not equal
          and = All sub rules must evaluate to TRUE
          or = One sub rule must evaluate to TRUE
      -->
      <assert op="" value="">
        <!--
          @element assert 
          @cardinality 0..*
          @summary Sub-assertions to apply
        -->
        <assert />
        <!-- 
          @element transform
          @cardinality 0..*
          @summary Represents one or more transformations to apply to the input value before evaulating the assertion
          @attribute {string} name The id of the transform which is to be applied
        -->
        <transform name="">
          <!--
            @element args
            @cardinality 0..1
            @summary Indicates parameters that should be passed to the transform
            -->
          <args>
            <!--
              @element int|double|string|boolean
              @cardinality 1..*
              @summary The actual parameter value to add
            -->
            <add></add>
          </args>
        </transform>
      </assert>
      <!--
        @element partialWeight
        @cardinality 0..1
        @summary When present, indicates how the input and output vectors should be partially weighted
        @attribute {enum} name Indicates the method of measuring or scoring the output
        @description
        Valid methods of measuring are:
          date_diff - Measure based on timespan difference (dates) of A and B. Threshold is 
          phonetic_diff - Measure based on the difference of the metaphone phonetic code of A and B
          levenshtein - Measure based on % of characters which are different between each string
      -->
      <partialWeight name="">
        <!--
            @element args
            @cardinality 0..1
            @summary Indicates parameters that should be passed to the measure
            -->
        <args>
          <!--
              @element int|double|string|boolean
              @cardinality 1..*
              @summary The actual parameter value to add
            -->
          <add></add>
        </args>
      </partialWeight>
    </vector>
  </classification>
</MatchConfiguration>
```