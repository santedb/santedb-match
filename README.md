# SanteDB Matcher

This project adds an implementation of a fuzzy matching service to the SanteDB iCDR 
core instance in which it is installed. It provides:

* Services for identifying duplicate records 
* Services for classifying an inbound record against the iCDR's data source
* Query Filters for fuzzy searching using the HDSI query syntax

## Match Process

There are two matchers that are provided in the matching project:

* **DeterministicMatcher** which can detect matches based on HDSI query criteria directly from the database. These matches are not scored, rather they have a probability of 0.0 or 1.0 only.
* **Weighted/Scoring Matcher** which uses a three stage algorithm to determine matches given a series of rules. The steps for this matcher are as follows:
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

Up to date documentation can be found on the [SanteDB Wiki](https://app.gitbook.com/@fyfe-software-inc/s/ops/santempi/matching-engine)
