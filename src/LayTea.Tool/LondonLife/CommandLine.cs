// Copyright (c) 2021 SceneGate

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
namespace SceneGate.Games.ProfessorLayton.Tool.LondonLife
{
    using System.CommandLine;
    using System.CommandLine.Invocation;

    /// <summary>
    /// Command-line interface for Professor Layton London Life game.
    /// </summary>
    public static class CommandLine
    {
        /// <summary>
        /// Create the CLI command for the game.
        /// </summary>
        /// <returns>The CLI command.</returns>
        public static Command CreateCommand()
        {
            return new Command("londonlife", "Professor Layton London Life (US only)") {
                CreateTextCommand(),
                CreateGraphicCommand(),
            };
        }

        private static Command CreateTextCommand()
        {
            var export = new Command("export", "Export the text to PO") {
                new Option<string>("--input", "the game file", ArgumentArity.ExactlyOne),
                new Option<string>("--table", "optional path to the table file", ArgumentArity.ZeroOrOne),
                new Option<LondonLifeTextFormat>("--format", "the format of the input file", ArgumentArity.ExactlyOne),
                new Option<string>("--output", "the output folder for the text files", ArgumentArity.ExactlyOne),
            };
            export.Handler = CommandHandler.Create<string, string, LondonLifeTextFormat, string>(TextCommands.Export);

            var import = new Command("import", "Import the text from PO") {
                new Option<string>("--input", "the input folder with the text files", ArgumentArity.ExactlyOne),
                new Option<string>("--table", "optional path to the table file", ArgumentArity.ZeroOrOne),
                new Option<string>("--original-darc", "the original ll_common.darc file if the output format is CommonDarc", ArgumentArity.ZeroOrOne),
                new Option<string>("--output", "the new game file", ArgumentArity.ExactlyOne),
                new Option<LondonLifeTextFormat>("--format", "the format of the output file", ArgumentArity.ExactlyOne),
            };
            import.Handler = CommandHandler.Create<string, string, string, string, LondonLifeTextFormat>(TextCommands.Import);

            return new Command("text", "Export/Import text files") {
                export,
                import,
            };
        }

        private static Command CreateGraphicCommand()
        {
            var exportTown = new Command("export-town", "Export the graphics from ll_town.darc") {
                new Option<string>("--town", "the ll_town.darc file", ArgumentArity.ExactlyOne),
                new Option<string>("--output", "the output directory", ArgumentArity.ExactlyOne),
            };
            exportTown.Handler = CommandHandler.Create<string, string>(GraphicCommands.ExportTown);

            var exportSave = new Command("export-save", "Export the graphics from ll_save.darc") {
                new Option<string>("--save", "the ll_save.darc file", ArgumentArity.ExactlyOne),
                new Option<string>("--output", "the output directory", ArgumentArity.ExactlyOne),
            };
            exportSave.Handler = CommandHandler.Create<string, string>(GraphicCommands.ExportSave);

            return new Command("graphic", "Export/Import graphic files") {
                exportTown,
                exportSave,
            };
        }
    }
}
