# Testing of Ycode.AspNetCore.Mvc.GroupVersioning

The tests in this test project implement the scenarios that are derived from the user stories below.

## Features

* Group Versioning
* API Versioning users can easily introduce API Group Versioning
* Support for Web API implementation manners where API Versioning works

### Feature: Group Versioning

As an API provider,
I want to manage versions for each endpoint independently
so that I don't need to touch the code that does not have behaviour changes.

As an API provider,
I want to provide global versions that are associated with endpoints of a specific version each that are compatible with each other endpoints
so that API user can access endpoints of the right version only with a single global version.

[See scenarios for these user stories.](./doc/scenarios-group-versioning.md)

### Feature: API Versioning users can easily introduce API Group Versioning

As an API provider who currently uses ASP.NET API Versioning in a ASP.NET Web API application,
I want my application to work all the same way even using ASP.NET API Group Versioning
so that I can introduce or try API Group Versioning in a part of my API application
and that I can expand API Group Versioning step by step.

[See scenarios for this user story.](./doc/scenarios-easy-to-introduce.md)

### Feature: Support for Web API implementation manners where API Versioning works

As an API provider who currently uses ASP.NET API Versioning in a ASP.NET Web API application,
I want to introduce API Group Versioning with small amount of changes
so that I can introduce API Group Versioning with a low cost and a low risk.

[See scenarios for this user story.](./doc/scenarios-web-api-impl-manners.md)
