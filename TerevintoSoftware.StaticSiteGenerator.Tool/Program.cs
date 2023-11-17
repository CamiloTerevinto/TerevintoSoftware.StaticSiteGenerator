using Spectre.Console.Cli;
using TerevintoSoftware.StaticSiteGenerator.Tool;

var app = new CommandApp();

app.Configure(configurator =>
{
    configurator
        .SetApplicationName("ssg")
        .SetApplicationVersion("2.0.0");

    configurator.AddCommand<GenerateCommand>("generate")
        .WithDescription(
            "Generates a static site from an ASP.NET Core MVC project. Please look at the GitHub project for more information: "
            + "https://github.com/CamiloTerevinto/TerevintoSoftware.StaticSiteGenerator"
        );
});

return app.Run(args);