using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.Frosting;
using Spectre.Console;

namespace SceneGate.Games.ProfessorLayton.Build
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            PrintHeader();
            if (args.Length == 0) {
                AnsiConsole.MarkupLine("[red]Missing game argument[/]");
                PrintUsage();
                return 3;
            }

            var host = new CakeHost();

            string game = args[0];
            if (game == "londonlife") {
                host.UseContext<LondonLife.Context>()
                    .UseLifetime<LondonLife.LifeTime>();
            } else {
                AnsiConsole.MarkupLine($"[red]Missing or unsupported game: {game}[/]");
                PrintUsage();
                return 2;
            }

            return host.Run(args.Skip(1));
        }

        private static void PrintHeader()
        {
            AnsiConsole.Render(new FigletText("LayTea").Color(Color.Blue));
            AnsiConsole.MarkupLine("[italic blue]Exporter and importer for Professor Layton game files[/]");

            var version = typeof(Program).Assembly.GetName().Version;
            AnsiConsole.MarkupLine($"[italic blue]Version: {version}[/]");
        }

        private static void PrintUsage()
        {
            AnsiConsole.MarkupLine("[yellow]USAGE:[/] LayTeaBuild <game:londonlife> <cake arguments>");
        }
    }
}
