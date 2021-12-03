## Object Comparison
The object comparison library (available as a nuget package - BorsukSoftware.Testing.Comparison.Core) is a library designed make it easier to perform comparative testing. This is where we are (or aren't) expecting to see changes in the results of a complex system and it's necessary to have visibility of those changes before the changes can be approved.

The library is structured as a base framework with plugins to perform the actual comparisons so if a user wishes to extend the range of comparisons which are performed, then they are able to by writing a new plugin rather than having to modify the framework.

Note that we do not recommend the use of this library where the requirements are more akin to unit tests. For that, there are other libraries better suited to a pass-fail approach such as FluentAssertions.

#### Suggested use cases
We put this together to simplify the process of seeing the impact of code, config or process changes upon the results of a complex workflow. In our use-case, we used the BorsukSoftware.ObjectFlattener.* framework to flatten our complex result types to a comparable form before feeding them through this library. In the case of the objects being equal, the output was empty, in the case of there being differences, the output was transformed into a Json doc where the differences were grouped by calculation unit so that they could be inspected subsequently by a human for approval (or further processed in the automated testing process etc.).

It is expected that the library would be integrated into a user's own workflow to do the comparisons of their own data structures, especially around the appropriate grouping of items for comparison (see examples for, well, examples).

To see examples of how to use the library, see the 'Examples' project.

#### Supported features out of the box
There are plugins to compare:

* byte / sbyte
* int (16|32|64)
* uint (16|32|64)
* general (signed|unsigned) integers (treats items as their long equivalents)
* BigInteger
* decimal
* float
* double
* strings
* reference equality

As well as function / filter based plugins to make it easier to configure to your own requirements.

Where appropriate, the default plugins allow a user to specify a tolerance so that small numerical noise can be ignored as well as being able to treat a missing value as the default value for the type (strings / numbers)

#### FAQs

###### I prefer unit tests, why would I use this?
Unit tests are great where there is always a known outcome, the range of inputs is easy to know and changes are rare. We use comparative tests to handle cases where we're making a change to an entire process (whether configuration, a maths library which is used, the nature of the process etc.) and we want to know 2 items:

1. Is there an impact to the output
2. What is the actual impact.

To give a concrete example, assuming one was making a change to the way some analytics calculated an output within a derivatives workflow, one would naturally write unit tests in the underlying library to confirm that the behaviour was as expected. However, the range of possible inputs / market data etc. which one might actually have in production would typically be vastly greater than the range of inputs coverable in the unit tests, not to mention that one may not be aware of the full set of possible impacts (e.g. a developer may only be aware of the conventions of developed markets and so only test them, whereas their analytics are being used to EMG markets as well). This library could be used to  generate a report showing the differences at the high level system level post that change. The person responsible for signing off that change would then be able to view the differences and hopefully verify that no unexpected changes had slipped in. 

A further example would be if there were to be a desire to change the configuration of a system, it'd be rare that there would be zero impact. It would frequently be very helpful to the persons responsible for both making and signing off the change to have a report which could express the differences which would occur under those circumstances. 

This doesn't remove the need for unit tests, we love unit tests and use them heavily ourselves. This is a subsequent, additional step which we find very useful in both our development processes and our config management processes.

###### How do tolerances work?
There are absolute and relative tolerances. If any selected mode would be treated as a pass, then the value will be marked as being equal. If you need a case whereby multiple criteria have to be matched for it to be considered a match, then you'll have to create a custom comparison plugin.

###### Why are there separate plugins for int16 vs. int32 etc.
Because in a lot of cases, even if they're the same number, the change in data format could be considered to be a failure state. There are also plugins which can compare all ints or uints (2 separate plugins) which ignore the data type and do the comparison as long / ulong objects.

###### How do I compare dictionaries?
There are 2 choices here:
1. Split the dictionary object up (using a custom plugin for BorsukSoftware.ObjectFlattener.*) into individual components
2. Create a custom plugin and payload object to internalise the comparison.

Which method is best depends on your use case.

###### Why isn't there a plugin to cover my specific use case?
Presumably because we haven't encountered it yet. At this point, you're best off creating a plugin locally. If you think that it could be used more generally, then please do let us know and we'll see what we can do.

###### I have a bug fix / performance improvement / new feature idea, what do I do?
Either raise an issue in this repository or contact us.

