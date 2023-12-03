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
    public class Acl2PaletteCollectionTests
    {
        private Acl2PaletteCollection converter;

        [SetUp]
        public void Setup()
        {
            converter = new Acl2PaletteCollection();
        }

        [Test]
        public void Guards()
        {
            Assert.That(() => converter.Convert(null), Throws.ArgumentNullException);
        }

        [Test]
        public void ThrowIfInvalidStamp()
        {
            byte[] binary = new byte[] {
                0x41, 0x43, 0x4C, 0x30, 0x01, 0x00, 0x00, 0x00,
                0x04, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00,
                0xDF, 0x7B, 0x26, 0x5D,
            };
            var stream = DataStreamFactory.FromArray(binary, 0, binary.Length);
            using var binaryFormat = new BinaryFormat(stream);

            Assert.That(() => converter.Convert(binaryFormat), Throws.InstanceOf<FormatException>());
        }

        [Test]
        public void ReadTwoPalettes()
        {
            byte[] binary = new byte[] {
                0x41, 0x43, 0x4C, 0x20, 0x02, 0x00, 0x00, 0x00,
                0x04, 0x00, 0x0A, 0x00, 0x02, 0x00, 0x00, 0x00,
                0xDF, 0x7B, 0x26, 0x5D, 0x01, 0x00, 0x00, 0x00,
                0x5F, 0x2F,
            };
            var stream = DataStreamFactory.FromArray(binary, 0, binary.Length);
            using var binaryFormat = new BinaryFormat(stream);

            var collection = converter.Convert(binaryFormat);
            Assert.That(collection.Palettes, Has.Count.EqualTo(2));
            Assert.That(collection.Palettes[0].Colors, Has.Count.EqualTo(2));
            Assert.That(collection.Palettes[0].Colors[0].Red, Is.EqualTo(248));
            Assert.That(collection.Palettes[0].Colors[0].Green, Is.EqualTo(240));
            Assert.That(collection.Palettes[0].Colors[0].Blue, Is.EqualTo(240));
            Assert.That(collection.Palettes[1].Colors, Has.Count.EqualTo(1));
        }
    }
}
