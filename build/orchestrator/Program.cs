using BuildSystem;
using Cake.Common.Build;
using Cake.Core;
using Cake.Frosting;
using Cake.Frosting.PleOps.Recipe;
using Cake.Frosting.PleOps.Recipe.Dotnet;

return new CakeHost()
    .AddAssembly(typeof(PleOpsBuildContext).Assembly)
    .UseContext<BuildContext>()
    .UseLifetime<BuildLifetime>()
    .Run(args);

public sealed class BuildLifetime : FrostingLifetime<BuildContext>
{
    public override void Setup(BuildContext context, ISetupContext info)
    {
        // We can't force code coverage locally as it requires game files
        context.DotNetContext.CoverageTarget = context.BuildSystem().IsLocalBuild ? 0 : 100;

        context.ReadArguments();

        context.DotNetContext.PreviewNuGetFeed = "https://pkgs.dev.azure.com/SceneGate/SceneGate/_packaging/SceneGate-Preview/nuget/v3/index.json";
        context.DotNetContext.ApplicationProjects.Add(new ProjectPublicationInfo(
            "./src/LayTea", [ "win-x64", "linux-x64", "osx-x64" ], "net8.0"));

        context.Print();
    }

    public override void Teardown(BuildContext context, ITeardownContext info)
    {
        // Save the info from the existing artifacts for the next execution (e.g. deploy job)
        context.DeliveriesContext.Save();
    }
}

[TaskName("Default")]
[IsDependentOn(typeof(Cake.Frosting.PleOps.Recipe.Common.SetGitVersionTask))]
[IsDependentOn(typeof(Cake.Frosting.PleOps.Recipe.Common.CleanArtifactsTask))]
[IsDependentOn(typeof(Cake.Frosting.PleOps.Recipe.Dotnet.DotnetTasks.BuildProjectTask))]
public sealed class DefaultTask : FrostingTask
{
}

[TaskName("Bundle")]
[IsDependentOn(typeof(Cake.Frosting.PleOps.Recipe.Common.SetGitVersionTask))]
[IsDependentOn(typeof(Cake.Frosting.PleOps.Recipe.GitHub.ExportReleaseNotesTask))]
[IsDependentOn(typeof(Cake.Frosting.PleOps.Recipe.Dotnet.DotnetTasks.BundleProjectTask))]
[IsDependentOn(typeof(Cake.Frosting.PleOps.Recipe.DocFx.BuildTask))]
public sealed class BundleTask : FrostingTask
{
}

[TaskName("Deploy")]
[IsDependentOn(typeof(Cake.Frosting.PleOps.Recipe.Common.SetGitVersionTask))]
[IsDependentOn(typeof(Cake.Frosting.PleOps.Recipe.Dotnet.DotnetTasks.DeployProjectTask))]
[IsDependentOn(typeof(Cake.Frosting.PleOps.Recipe.GitHub.UploadReleaseBinariesTask))]
public sealed class DeployTask : FrostingTask
{
}
