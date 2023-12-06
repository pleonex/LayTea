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
            var tableArg = new Option<string>("--table", "optional path to the table file");

            var exportInputArg = new Option<string>("--input", "the game file") { IsRequired = true };
            var exportFormatArg = new Option<LondonLifeTextFormat>("--format", "the format of the input file") { IsRequired = true };
            var exportOutputArg = new Option<string>("--output", "the output folder for the text files") { IsRequired = true };
            var export = new Command("export", "Export the text to PO") {
                exportInputArg,
                tableArg,
                exportFormatArg,
                exportOutputArg,
            };
            export.SetHandler(TextCommands.Export, exportInputArg, tableArg, exportFormatArg, exportOutputArg);

            var importInputArg = new Option<string>("--input", "the input folder with the text files") { IsRequired = true };
            var importDarcArg = new Option<string>("--original-darc", "the original ll_common.darc file if the output format is CommonDarc");
            var importOutputArg = new Option<string>("--output", "the new game file") { IsRequired = true };
            var importFormatArg = new Option<LondonLifeTextFormat>("--format", "the format of the output file") { IsRequired = true };

            var import = new Command("import", "Import the text from PO") {
                importInputArg,
                tableArg,
                importDarcArg,
                importOutputArg,
                importFormatArg,
            };
            import.SetHandler(TextCommands.Import, importInputArg, tableArg, importDarcArg, importOutputArg, importFormatArg);

            return new Command("text", "Export/Import text files") {
                export,
                import,
            };
        }

        private static Command CreateGraphicCommand()
        {
            var townInArg = new Option<string>("--town", "the ll_town.darc file") { IsRequired = true };
            var townOutArg = new Option<string>("--output", "the output directory") { IsRequired = true };
            var exportTown = new Command("export-town", "Export the graphics from ll_town.darc") {
                townInArg,
                townOutArg,
            };
            exportTown.SetHandler(GraphicCommands.ExportTown, townInArg, townOutArg);

            var saveInArg = new Option<string>("--save", "the ll_save.darc file") { IsRequired = true };
            var saveOutArg = new Option<string>("--output", "the output directory") { IsRequired = true };
            var exportSave = new Command("export-save", "Export the graphics from ll_save.darc") {
                saveInArg,
                saveOutArg,
            };
            exportSave.SetHandler(GraphicCommands.ExportSave, saveInArg, saveOutArg);

            var kihiraInArg = new Option<string>("--kihira", "the kihira.archive file") { IsRequired = true };
            var kihiraOutArg = new Option<string>("--output", "the output directory") { IsRequired = true };
            var exportKihira = new Command("export-kihira", "Export the graphics from kihira.archive") {
                kihiraInArg,
                kihiraOutArg,
            };
            exportKihira.SetHandler(GraphicCommands.ExportKihira, kihiraInArg, kihiraOutArg);

            return new Command("graphic", "Export/Import graphic files") {
                exportTown,
                exportSave,
                exportKihira,
            };
        }
    }
}
