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
namespace SceneGate.Games.ProfessorLayton.Tool
{
    using System;
    using System.IO;
    using System.Linq;
    using SceneGate.Games.ProfessorLayton.Containers;
    using SceneGate.Games.ProfessorLayton.Graphics;
    using Texim.Compressions.Nitro;
    using Texim.Formats;
    using Texim.Images;
    using Texim.Palettes;
    using Yarhl.FileSystem;
    using Yarhl.IO;

    /// <summary>
    /// Commands related to graphic files.
    /// </summary>
    public static class GraphicCommands
    {
        /// <summary>
        /// Export all the images from the town container.
        /// </summary>
        /// <param name="town">The path to the town file.</param>
        /// <param name="output">The output directory.</param>
        public static void ExportTown(string town, string output)
        {
            using var townNode = NodeFactory.FromFile(town, FileOpenMode.Read)
                .TransformWith<BinaryDarc2Container>();
            var nodes = townNode.Children;

            for (int i = 0; i < 53; i++) {
                // The game uses the palette + 1 at night
                // No idea which pixels uses the map 106 + i, but it looks like a collision map
                // No idea which palette either, so we use 3341 that has 16 palettes.
                string basePath = Path.Combine(output, "maps", $"map_{i}");
                ExportBackgroundA(nodes[3341], nodes[398 + i], nodes[106 + i], basePath + "_x.png");
                ExportBackgroundA(nodes[3339 + (i * 2)], nodes[398 + i], nodes[i * 2], basePath + "_0.png");
                ExportBackgroundA(nodes[3339 + (i * 2)], nodes[398 + i], nodes[(i * 2) + 1], basePath + "_1.png");
            }

            var menuPath = Path.Combine(output, "menu_bg.png");
            ExportBackgroundA(nodes[4523], nodes[451], nodes[159], menuPath);

            for (int i = 0; i < 10; i++) {
                string imagePath = Path.Combine(output, "minimaps", $"minimap{i}.png");
                ExportBackgroundA(nodes[4525 + i], nodes[452 + i], nodes[160 + i], imagePath);
            }

            for (int i = 0; i < 114; i++) {
                string imagePath = Path.Combine(output, "rooms", $"room_{i}.png");
                try {
                    ExportBackgroundA(nodes[4256 + i], nodes[284 + i], nodes[170 + i], imagePath);
                } catch {
                    // Some images crash probably due to invalid width param.
                    // They are not very important so skip.
                }
            }
        }

        /// <summary>
        /// Export all the images from the save container.
        /// </summary>
        /// <param name="save">The save container.</param>
        /// <param name="output">The output directory.</param>
        public static void ExportSave(string save, string output)
        {
            using var saveNode = NodeFactory.FromFile(save, FileOpenMode.Read)
                .TransformWith<BinaryDarc2Container>();
            var nodes = saveNode.Children;

            var bg0Path = Path.Combine(output, "bg0.png");
            ExportBackgroundA(nodes[1678], nodes[2], nodes[0], bg0Path);

            var bg1Path = Path.Combine(output, "bg1.png");
            ExportBackgroundA(nodes[1679], nodes[3], nodes[1], bg1Path);
        }

        /// <summary>
        /// Export all the images from the kihira container.
        /// </summary>
        /// <param name="kihira">The kihira container.</param>
        /// <param name="output">The output directory.</param>
        public static void ExportKihira(string kihira, string output)
        {
            using var kihiraNode = NodeFactory.FromFile(kihira, FileOpenMode.Read)
                .TransformWith<BinaryArchive2Container>();

            Node lastPalette = null;
            Node lastPixels = null;
            var nodes = kihiraNode.Children.Where(c => c.Stream.Length > 0);
            foreach (var node in nodes) {
                node.TransformWith<NitroLzxDecompression>();
                string id = new DataReader(node.Stream).ReadString(4);

                if (id == "NCCL") {
                    lastPalette = node;
                } else if (id == "NCCG") {
                    lastPixels = node;
                } else if (id == "NCSC") {
                    ExportBackgroundNWithName(lastPalette, lastPixels, node, output);
                }
            }
        }

        private static void ExportBackgroundA(Node palette, Node pixels, Node map, string output)
        {
            if (palette.Format is not PaletteCollection) {
                palette.TransformWith<DencDecompression>()
                    .TransformWith<Acl2PaletteCollection>();
            }

            if (pixels.Format is not NodeContainerFormat) {
                pixels.TransformWith<DencDecompression>()
                    .TransformWith<Acbg2IndexedImageContainer>();

                if (pixels.Children.Count != 1) {
                    throw new FormatException("Invalid number of images for a background");
                }
            }

            if (map.Format is not ScreenMap) {
                map.TransformWith<DencDecompression>()
                    .TransformWith<BinaryAsc2ScreenMap>();
            }

            // We don't use TransformTo because we may transform the same node
            // several times with different palettes / maps.
            var mapParams = new MapDecompressionParams {
                Map = map.GetFormatAs<ScreenMap>(),
            };
            var mapDecompression = new MapDecompression(mapParams);
            var indexedImage = mapDecompression.Convert(pixels.Children[0].GetFormatAs<IndexedImage>());

            var paletteParams = new IndexedImageBitmapParams {
                Palettes = palette.GetFormatAs<PaletteCollection>(),
            };
            var bitmapConverter = new IndexedImage2Bitmap(paletteParams);
            using var bitmap = bitmapConverter.Convert(indexedImage);
            bitmap.Stream.WriteTo(output);
        }

        private static void ExportBackgroundNWithName(Node palette, Node pixels, Node map, string output)
        {
            Ncsc ncsc = map.TransformWith<BinaryNcsc2ScreenMap>().GetFormatAs<Ncsc>();

            string path = ncsc.ImageName
                .Replace("\\", "/")
                .Replace("..", "parent")
                .Replace(".ncg", string.Empty);
            path = Path.Combine(output, path);

            int index = 0;
            string finalName = path + ".png";
            while (File.Exists(finalName)) {
                index++;
                finalName = path + $"_{index}.png";
            }

            ExportBackgroundN(palette, pixels, map, finalName);
        }

        private static void ExportBackgroundN(Node palette, Node pixels, Node map, string output)
        {
            if (palette.Format is not PaletteCollection) {
                palette.TransformWith<BinaryNccl2PaletteCollection>();
            }

            if (pixels.Format is not IndexedImage) {
                pixels.TransformWith<BinaryNccg2IndexedImage>();
            }

            if (map.Format is not Ncsc) {
                map.TransformWith<BinaryNcsc2ScreenMap>();
            }

            // We don't use TransformTo because we may transform the same node
            // several times with different palettes / maps.
            var mapParams = new MapDecompressionParams {
                Map = map.GetFormatAs<Ncsc>(),
            };
            var mapDecompression = new MapDecompression(mapParams);
            var indexedImage = mapDecompression.Convert(pixels.GetFormatAs<IndexedImage>());

            var paletteParams = new IndexedImageBitmapParams {
                Palettes = palette.GetFormatAs<PaletteCollection>(),
            };
            var bitmapConverter = new IndexedImage2Bitmap(paletteParams);
            using var bitmap = bitmapConverter.Convert(indexedImage);
            bitmap.Stream.WriteTo(output);
        }
    }
}
