# IP Locator
A code-kata exercise in refactoring code for unit testing.

## Unit Tests Test a Unit
To understand a unit test, we must first understand a unit. The unit we test
is a single process or algorithm within our codebase. This unit might have 
dependencies or a known context from where it's called. But these are 
irrelevant for the unit test. 

A unit test _should_ include situations we might never get to in our code.
And it should not rely on dependencies to give it the results it needs.

### Ignore The Context of your Unit
In this codebase, when we test `IpApiService`, we should not care that
`IpLocatorService` validates the IP address. We should still test the unit
with invalid data and know that it produces the expected result. This is 
especially true if we have exception handling to handle these situations even 
if they can "never happen".

When a situation never occurs, we are inclined to forget what a unit does when
it encounters this situation. With the functionality documented in unit tests, 
we always know what will happen in this situation and, more importantly, when
our understanding has changed. If a future development then bypasses the
checks that ensure that this situation never occurs, we are aware of the side
effects and how they may differ from our assumptions.

### Fake all your Dependencies
Inside our unit, if we are making calls to a dependency (eg `HttpClient`), our
tests shouldn't rely on getting the correct result from our dependencies. 
Instead we should consider using mocks or stubs to produce expected results.

This allows us to isolate failures to specific units. Consider that 
`IpLocatorService` makes requests to `IpApiService`. If we use the units in
`IpApiService` in our tests for `IpLocatorService` and changes to 
`IpApiService` cause it to fail its unit tests, these changes may also cause
`IpLocatorService` to fail its unit tests. If there is a much longer chain of 
dependencies, you may have multiple unit tests failing and it will become
difficult to locate the source of the fail.

A fake should implement the same interface as the class it is faking, but
simply provide an expected hardcoded output for an expected input. They should
not perform logic to derive the result because this will introduce another 
"unit" that needs to be tested.

### Tests That Can't Fulfil These Requirements Need to be Refactored
The most common reason to require refactoring is that a dependency can't be
mocked. There are some important examples of this in the codebase. 

There are some cases, however, where it's difficult to ignore the context
from the unit. There is an example of this in the codebase as well, which
will be discussed. 

In general, we are refactoring code that does not follow SOLID principles.
Problems with single responsibility make it hard to separate context and 
problems with dependency inversion make it difficult to separate 
dependencies. 

## Recommended Improvements to the Codebase
### Use `IHttpClientFactory` when Making Http Requests
`HttpClient`s are particularly difficult to inject and test. In the case here, 
`IpApiService` creates and disposes of a HttpClient on every request. 

But this means we can't test the `IpApiService` without actually making a http
request that returns our expected result. 

Here, we have to call ip-api to test our service for correct responses. To get
an invalid result we have to make a call to another api to get an incorrect
response (in this case, we use google.com).

But if the service is down or the values of the request change (which is often
the case for IPs), then our unit tests fail. Our unit tests should run offline
and with no external dependencies. 

### Using Interfaces
When testing `IpLocatorService`, we have to instantiate `IpApiService` to inject 
into it. We then use `IpApiService` as if it would be used in the actual running 
program. 

But what if `IpApiService` is not working properly? Then our `IpLocatorService` 
tests will also fail. Our unit test for `IpLocatorServiceTests` are not testing 
only the `IpLocatorService` unit. 

Consider using an interface for injecting `IpApiService` into `IpLocatorService`.
This will allow you to stub an `IIpApiService` to give you the results you
expect from a perfectly working `IpApiService`.

### Use Helper Classes instead of Private Static Methods
`IpLocatorService.EvaluateIp`, amongst other things, validates IP addresses 
against a list of reserved IP masks. It is impossible to test this validation
in isolation. 

For particularly complicated processes, it is better to move this code into a
separate helper class, like an `IpValidatorService` so that the unit we are
testing is easier to understand.

## Further Exploration
When all this is done, you should now have 100% code coverage except for
`IpLocatorService.Start`. 

- Is this testable as is? 
- How would you update it? 