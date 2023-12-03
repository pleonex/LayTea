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
    using NUnit.Framework;
    using SceneGate.Games.ProfessorLayton.Graphics;
    using Yarhl.IO;

    [TestFixture]
    public class BinaryAsc2ScreenMapTests
    {
        private BinaryAsc2ScreenMap converter;

        [SetUp]
        public void Setup()
        {
            converter = new BinaryAsc2ScreenMap();
        }

        [Test]
        public void Guards()
        {
            Assert.That(() => converter.Convert(null), Throws.ArgumentNullException);
        }

        [Test]
        public void DeserializeMinimumFile()
        {
            byte[] binary = new byte[] {
                0x41, 0x53, 0x43, 0x20, 0x08, 0x00, 0x08, 0x00,
                0x01, 0x00, 0x10, 0x00, 0x02, 0x00, 0x00, 0x00,
                0x80, 0xF0,
            };
            var stream = DataStreamFactory.FromArray(binary, 0, binary.Length);
            using var binaryFormat = new BinaryFormat(stream);

            var map = converter.Convert(binaryFormat);
            Assert.That(map.Width, Is.EqualTo(8));
            Assert.That(map.Height, Is.EqualTo(8));
            Assert.That(map.Maps, Has.Length.EqualTo(1));
            Assert.That(map.Maps[0].HorizontalFlip, Is.False);
            Assert.That(map.Maps[0].VerticalFlip, Is.False);
            Assert.That(map.Maps[0].PaletteIndex, Is.EqualTo(15));
            Assert.That(map.Maps[0].TileIndex, Is.EqualTo(0x80));
        }

        [Test]
        public void ThrowIfInvalidStamp()
        {
            byte[] binary = new byte[] {
                0x41, 0x53, 0x43, 0x30, 0x08, 0x00, 0x08, 0x00,
                0x01, 0x00, 0x10, 0x00, 0x02, 0x00, 0x00, 0x00,
                0x00, 0x00,
            };
            var stream = DataStreamFactory.FromArray(binary, 0, binary.Length);
            using var binaryFormat = new BinaryFormat(stream);

            Assert.That(() => converter.Convert(binaryFormat), Throws.InstanceOf<FormatException>());
        }

        [Test]
        public void ThrowIfMultipleMaps()
        {
            byte[] binary = new byte[] {
                0x41, 0x53, 0x43, 0x20, 0x08, 0x00, 0x08, 0x00,
                0x02, 0x00, 0x10, 0x00, 0x02, 0x00, 0x00, 0x00,
                0x00, 0x00,
            };
            var stream = DataStreamFactory.FromArray(binary, 0, binary.Length);
            using var binaryFormat = new BinaryFormat(stream);

            Assert.That(() => converter.Convert(binaryFormat), Throws.InstanceOf<FormatException>());
        }
    }
}
