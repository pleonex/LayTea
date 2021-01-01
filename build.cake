#load "nuget:?package=PleOps.Cake&version=0.4.1-preview.3&prerelease"

Task("Define-Project")
    .Description("Fill specific project information")
    .Does<BuildInfo>(info =>
{
    info.AddLibraryProjects("LayTea");
    info.AddApplicationProjects("LayTea.Tool");
    info.AddTestProjects("LayTea.Tests");

    info.PreviewNuGetFeed = "https://nuget.pkg.github.com/pleonex/index.json";
    info.StableNuGetFeed = "https://nuget.pkg.github.com/pleonex/index.json";
});

Task("Default")
    .IsDependentOn("Stage-Artifacts");

string target = Argument("target", "Default");
RunTarget(target);
