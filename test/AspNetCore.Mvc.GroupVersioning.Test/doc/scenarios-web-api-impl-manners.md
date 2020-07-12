## Scenarios

### Feature: Support for Web API implementation manners where API Versioning works

#### Scenario: API Group Versioning works in a variety styles of ASP.NET Web API where API Versioning works
```
Given I have a ASP.NET Web API application
And I use ASP.NET API Group Versioning
And I use the following attributes and classes for the code of an endpoint
When API user requests to the endpoint
Then the routing of the endpoint succeeds or fails in the way defined in 'Result' column below
```

| Sample    | ApiVersion Attr | MapToApiVersion Attr | Controller Attr | Controller Base Class | Result                                                       |
| --------- |:---------------:|:--------------------:| --------------- | --------------------- | ------------------------------------------------------------ |
| Pattern11 | ✓               | ✕                    | none            | none                  | routing fails with an error because multiple endpoints match |
| Pattern14 | ✓               | ✕                    | none            | ControllerBase        | routing fails with an error because multiple endpoints match |
| Pattern12 | ✓               | ✕                    | ApiController   | none                  | routing succeeds                                             |
| Pattern15 | ✓               | ✕                    | ApiController   | ControllerBase        | routing succeeds                                             |
| Pattern13 | ✓               | ✕                    | Controller      | none                  | routing fails with an error because multiple endpoints match |
| Pattern16 | ✓               | ✕                    | Controller      | ControllerBase        | routing fails with an error because multiple endpoints match |
| Pattern1  | ✓               | ✓                    | none            | none                  | routing fails with an error because multiple endpoints match |
| Pattern4  | ✓               | ✓                    | none            | ControllerBase        | routing fails with an error because multiple endpoints match |
| Pattern2  | ✓               | ✓                    | ApiController   | none                  | routing succeeds                                             |
| Pattern5  | ✓               | ✓                    | ApiController   | ControllerBase        | routing succeeds                                             |
| Pattern3  | ✓               | ✓                    | Controller      | none                  | routing fails with an error because multiple endpoints match |
| Pattern6  | ✓               | ✓                    | Controller      | ControllerBase        | routing fails with an error because multiple endpoints match |

Legend:

✓: use, ✕: do not use

NOTE:
* It is assumed that Controller class behaves the same as ControllerBase class in terms of the controller-action routing because Controller class inherits ControllerBase class.
* Sample3 implements these cases without group version, and tested in WebApiImplementationVarietyTest.cs.
* Sample5 implements these cases with group version, and tested in WebApiImplementationVarietyTest.cs.
