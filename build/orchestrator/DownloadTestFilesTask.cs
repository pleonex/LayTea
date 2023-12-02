namespace BuildSystem;

using System.Text.Json;
using Cake.Common.IO;
using Cake.Common.Net;
using Cake.Core.Diagnostics;
using Cake.Frosting;

[TaskName("Download-TestFiles")]
[TaskDescription("Download the test resource files")]
[IsDependeeOf(typeof(Cake.Frosting.PleOps.Recipe.Dotnet.TestTask))]
public class DownloadTestFilesTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        string resourcesPath = Path.Combine(context.TemporaryPath, "test_resources");
        Environment.SetEnvironmentVariable("SCENEGATE_TEST_DIR", resourcesPath);

        if (Directory.Exists("resources")) {
            context.Log.Information("Test files already exists, skipping download.");
            return;
        }

        if (string.IsNullOrEmpty(context.TestResourceUri)) {
            context.Log.Information("Test resource uri is not present, skipping download.");
            return;
        }

        var jsonInfoPath = context.DownloadFile(context.TestResourceUri);
        string jsonInfoText = File.ReadAllText(jsonInfoPath.FullPath);

        var jsonOptions = new JsonSerializerOptions(JsonSerializerOptions.Default) {
            AllowTrailingCommas = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };
        IEnumerable<TestResource>? resources = JsonSerializer
            .Deserialize<IEnumerable<TestResource>>(jsonInfoText, jsonOptions);
        if (resources is null) {
            throw new Exception("Failed to read json info file");
        }

        foreach (TestResource resource in resources) {
            var compressedResources = context.DownloadFile(resource.Uri);
            context.Unzip(compressedResources, Path.Combine(resourcesPath, resource.Path));
        }
    }

    private sealed record TestResource(string Uri, string Path);
}
