# Static Site Generator
[![Coverage Status](https://coveralls.io/repos/github/CamiloTerevinto/TerevintoSoftware.StaticSiteGenerator/badge.svg?branch=main)](https://coveralls.io/github/CamiloTerevinto/TerevintoSoftware.StaticSiteGenerator?branch=main) [![Nuget version](https://img.shields.io/nuget/v/TerevintoSoftware.StaticSiteGenerator.Tool)](https://www.nuget.org/packages/TerevintoSoftware.StaticSiteGenerator.Tool/)

This project aims to provide a way for c# developers to benefit from very cheap/free hosting through static files, 
while allowing them to use familiar ASP.NET MVC concepts (Views, Partials, Layouts, etc).
This also allows you to have an initial static site that you can then update to a dynamic website backed by ASP.NET Core without starting from scratch.

Since this project converts Views into HTML files, it is not possible (at least yet) to use Models.

## Packages

This project is divided in two packages:

| Package | Purpose |
| ------- | ------- |
| [TerevintoSoftware.StaticSiteGenerator][1] | Contains the main logic of the project, depends on ASP.NET Core. |
| [TerevintoSoftware.StaticSiteGenerator.Tool][2] | Contains a .NET Tool that can be invoked to perform the generation. |

## Sample usage

SSG.NET has its own website that lists the different features here: https://www.staticsitegenerator.net/.

There is also a sample GitHub repository that shows how to use the tool for a Blog: https://github.com/CamiloTerevinto/StaticBlogTemplate.

## How to build

* Install Visual Studio 2022 (.NET 6 required), if needed. The ASP.NET Core workload is required to build the project.
* Install git, if needed.
* Clone this repository.
* Build from Visual Studio or through `dotnet build`.

### Running tests

Once the solution is compiled, tests can be run either from Visual Studio's Test Explorer window, or through `dotnet test`.

## License

This project is licensed under the [MIT license](/LICENSE).

## Bug reports and feature requests

Please use the [issue tracker](3) and ensure your question/feedback was not previously reported.

[1]: https://www.nuget.org/packages/TerevintoSoftware.StaticSiteGenerator/
[2]: https://www.nuget.org/packages/TerevintoSoftware.StaticSiteGenerator.Tool/
[3]: https://github.com/CamiloTerevinto/TerevintoSoftware.StaticSiteGenerator/issues
