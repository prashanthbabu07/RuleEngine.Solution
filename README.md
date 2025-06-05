# Rule Engine

Let's work through and write in simple english on how should a rule engine should work.

## Overview

The core idea is to have a system that can evaluate rules based simple conditional evaluator.

<Some property> <Some operator> <Some value>

<Some property> has a defined data type. e.g. "duration", "resolution", "string", "number", etc.
<Some operator> has a defined set of operators that can be used to compare the property with the value. e.g. "equals", "in", "greater than", "less than", etc.
<Some value> can be set depending on <Some operator> i.e. "in" operator can have list of values, "equals" can have a single value, "greater than" can have a single value, etc.

We need support for complex rules that can be built using logical operators like "AND", "OR", and "NOT".

e.g. "property1 equals value1 AND property2 greater than value2 OR NOT property3 in [value3, value4]"

We need the ability to add new properties, operators, and values dynamically.
i.e. that system should be able to support for adding new properties, operators without changing existing implementation of the rule engine.

Since we can have complex data type i.e. unstructured like media files (videos, images etc.). We need to have these types of data transformed into something
digestible by the rule engine. 

The media content will be pre-processed and extract relevant metadata than can be used in the rules. 
For example, a video file can have properties like "duration", "resolution", "codec", etc.

[media] -> [pre-processor] -> [metadata] -> [rule engine]

We need the ability to include ML models as <Some operator> in the rules.
For example, a rule can be defined as "property  "


