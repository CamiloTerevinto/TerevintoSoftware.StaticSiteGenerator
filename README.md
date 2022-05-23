# TerevintoSoftware.StaticSiteGenerator

This project aims to provide a way for c# developers to benefit from very cheap/free hosting through static files, 
while allowing them to use familiar ASP.NET MVC concepts (Views, Partials, Layouts, etc).
This also allows you to have an initial static site that you can then update to a dynamic website backed by ASP.NET Core without starting from scratch.

Since this project converts Views into HTML files, it is not possible to use Models.

## Packages

This project is divided in two packages:

| Package | Purpose |
| ------- | ------- |
| [TerevintoSoftware.StaticSiteGenerator][1] | Contains the main logic of the project, depends on ASP.NET Core. |
| [TerevintoSoftware.StaticSiteGenerator.Tool][2] | Contains a .NET Tool that can be invoked to perform the generation. |

## Sample usage

1. Install the tool: `dotnet tool install TerevintoSoftware.StaticSiteGenerator.Tool`
2. Run the static site generation: `ssg --project-path [project-path] --output-path [output-path] --assembly-path [assembly-path]`

## How to build

* Install Visual Studio 2022 (.NET 6 required), if needed.
* Install git, if needed.
* Clone this repository.
* Build from Visual Studio or through `dotnet build`.

## Bug reports and feature requests

Please use the [issue tracker](https://github.com/CamiloTerevinto/TerevintoSoftware.StaticSiteGenerator/issues) and ensure your question/feedback was not previously reported.

[1]: https://www.nuget.org/packages/TerevintoSoftware.StaticSiteGenerator/
[2]: https://www.nuget.org/packages/TerevintoSoftware.StaticSiteGenerator.Tool/
