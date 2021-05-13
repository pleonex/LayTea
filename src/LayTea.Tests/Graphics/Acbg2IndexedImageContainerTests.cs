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
namespace SceneGate.Games.ProfessorLayton.Tests.Graphics
{
    using System;
    using System.Collections;
    using System.IO;
    using System.Security.Cryptography;
    using NUnit.Framework;
    using SceneGate.Games.ProfessorLayton.Containers;
    using SceneGate.Games.ProfessorLayton.Graphics;
    using Texim.Formats;
    using Texim.Palettes;
    using Yarhl.FileSystem;
    using Yarhl.IO;

    [TestFixture]
    public class Acbg2IndexedImageContainerTests
    {
        private Acbg2IndexedImageContainer converter;

        public static IEnumerable GetImages()
        {
            var savePath = Path.Combine(TestDataBase.RootFromOutputPath, "containers", "ll_save.darc");
            Node save = NodeFactory.FromFile(savePath, FileOpenMode.Read)
                .TransformWith<BinaryDarc2Container>();

            yield return new TestCaseData(
                save.Children[2].TransformWith<DencDecompression>(),
                GetPalette(save.Children[1678]),
                "ce512013456584f46778dc347ad104bbfae5ebe3fd814cb74fe062aca4c4a3fb");

            yield return new TestCaseData(
                save.Children[3].TransformWith<DencDecompression>(),
                GetPalette(save.Children[1679]),
                "f660ad3bfa42de668be656d29ee7484ed069db1f87e5ccada28dffaed9285840");
        }

        [SetUp]
        public void Setup()
        {
            converter = new Acbg2IndexedImageContainer();
        }

        [Test]
        public void Guards()
        {
            Assert.That(() => converter.Convert(null), Throws.ArgumentNullException);
        }

        [Test]
        public void ThrowIfInvalidFlag()
        {
            byte[] binary = new byte[] {
                0x41, 0x43, 0x42, 0x20, 0x01, 0x00, 0x00, 0x00,
                0x04, 0x00, 0x00, 0x00, 0x02, 0x08, 0x00, 0x84,
            };
            var stream = DataStreamFactory.FromArray(binary, 0, binary.Length);
            using var binaryFormat = new BinaryFormat(stream);

            Assert.That(() => converter.Convert(binaryFormat), Throws.InstanceOf<FormatException>());
        }

        [Test]
        public void ThrowIfInvalidStamp()
        {
            byte[] binary = new byte[] {
                0x41, 0x43, 0x42, 0x30,
            };
            var stream = DataStreamFactory.FromArray(binary, 0, binary.Length);
            using var binaryFormat = new BinaryFormat(stream);

            Assert.That(() => converter.Convert(binaryFormat), Throws.InstanceOf<FormatException>());
        }

        [TestCaseSource(nameof(GetImages))]
        public void DeserializeFile(Node pixels, PaletteCollection palette, string expectedHash)
        {
            var bitmapParams = new IndexedImageBitmapParams { Palettes = palette };
            var bitmap = pixels.TransformWith<Acbg2IndexedImageContainer>()
                .Children[0]
                .TransformWith<IndexedImage2Bitmap, IndexedImageBitmapParams>(bitmapParams);

            using var sha256 = SHA256.Create();
            bitmap.Stream.Position = 0;
            sha256.ComputeHash(bitmap.Stream);
            string actualHash = BitConverter.ToString(sha256.Hash)
                .Replace("-", string.Empty)
                .ToLowerInvariant();

            Assert.That(actualHash, Is.EqualTo(expectedHash));
        }

        private static PaletteCollection GetPalette(Node node)
        {
            return node.TransformWith<DencDecompression>()
                .TransformWith<Acl2PaletteCollection>()
                .GetFormatAs<PaletteCollection>();
        }
    }
}
