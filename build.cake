#load "nuget:?package=PleOps.Cake&version=0.4.1"

Task("Define-Project")
    .Description("Fill specific project information")
    .Does<BuildInfo>(info =>
{
    info.AddLibraryProjects("LayTea");
    info.AddApplicationProjects("LayTea.Tool");
    info.AddTestProjects("LayTea.Tests");

    info.PreviewNuGetFeed = "https://nuget.pkg.github.com/pleonex/index.json";
    info.StableNuGetFeed = "https://nuget.pkg.github.com/pleonex/index.json";

    // We can't force code coverage as it requires game files
    info.CoverageTarget = 0;
});

Task("Default")
    .IsDependentOn("Stage-Artifacts");

string target = Argument("target", "Default");
RunTarget(target);
