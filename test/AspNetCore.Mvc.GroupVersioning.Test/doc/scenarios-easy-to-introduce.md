## Scenarios

### Feature: API Versioning users can easily introduce API Group Versioning

#### Scenario: No Group Versioning
```
Given I have a ASP.NET Web API application
And I use ASP.NET API Group Versioning
And no ApiVersion attributes has group version
When API user request to an endpoint with a defined version
Then the request is routed to the same endpoint as in the case without ASP.NET API Group Versioning.
```
NOTE:
* Sample1 project implements this case, and tested in CommonTest.


#### Scenario: Request with group version to endpoint
```
Given I have a ASP.NET Web API application
And I use ASP.NET API Group Versioning
And all the ApiVersion attributes for an endpoint contain group version
When API user request to the endpoint with a group version defined among the ApiVersion attributes
Then the request are routed to the action where the group version is defined.
```
NOTE:
* All the same regardless of complete or partial Group Versioning
* Sample2 project implements complete Group Versioning, and tested in ImplicitOrExplicitGroupVersioningTest.
* No sample project implements partial Group Versioning, and not tested yet.

#### Scenario: Request with major.minor version to endpoint
```
Given I have a ASP.NET Web API application
And I use ASP.NET API Group Versioning
And all the ApiVersion attributes for an endpoint contain group version
When API user request to the endpoint with a major.minor version defined among the ApiVersion attributes
Then the request are routed to the action where the major.minor version is defined.
```
NOTE:
* All the same regardless of complete or partial Group Versioning
* Sample2 project implements complete Group Versioning, and tested in CommonTest.
* No sample project implements partial Group Versioning, and not tested yet.

#### Scenario: Request with group and major.minor version to endpoint
```
Given I have a ASP.NET Web API application
And I use ASP.NET API Group Versioning
And all the ApiVersion attributes for an endpoint contain group version
When API user request to the endpoint with a set of group version and major.minor version defined among the ApiVersion attributes
Then the request are routed to the action where the group version is defined.
```
NOTE:
* All the same regardless of complete or partial Group Versioning
* Sample2 project implements complete Group Versioning, and tested in ImplicitOrExplicitGroupVersioningTest.
* No sample project implements partial Group Versioning, and not tested yet.

#### Scenario: Request with undefined group version to endpoint with complete Group Versioning
```
Given I have a ASP.NET Web API application
And I use ASP.NET API Group Versioning
And I use Implicit Group Versioning option
And all the ApiVersion attributes for an endpoint contain group version (Group Version A and B in time order)
When API user request to the endpoint with a group version NOT defined among the ApiVersion attributes, which date is after Group Version A and before Group Version B
Then the request are routed to the action where Group Version A is defined.
```
NOTE:
* Sample4 project implements this case, but not tested yet.
* It should be tested in ImplicitOrExplicitGroupVersioningTest.

#### Scenario: Request with undefined group version to endpoint with partial Group Versioning
```
Given I have a ASP.NET Web API application
And I use ASP.NET API Group Versioning
And I use Implicit Group Versioning option
And some of the ApiVersion attributes for an endpoint contain group version, and others don't (Group Version A and B in time order)
When API user request to the endpoint with a group version NOT defined among the ApiVersion attributes, which date is after Group Version A and before Group Version B
Then the request not are routed and API user receives a routing error message.
```
NOTE:
* No sample project implements this case, but not tested yet.
* It should be tested in ImplicitOrExplicitGroupVersioningTest.
