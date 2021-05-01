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
namespace SceneGate.Games.ProfessorLayton.Tests.Texts
{
    using System;
    using NUnit.Framework;
    using SceneGate.Games.ProfessorLayton.Texts;
    using Yarhl.IO;
    using Yarhl.Media.Text;

    [TestFixture]
    public class ReplacerDeserializerTests
    {
        private ReplacerDeserializer deserializer;

        [SetUp]
        public void Setup()
        {
            deserializer = new ReplacerDeserializer();
        }

        [Test]
        public void Guards()
        {
            Assert.That(() => deserializer.Convert(null), Throws.ArgumentNullException);
        }

        [Test]
        public void ReadAllLines()
        {
            using var data = GetData("a=1\nb=2");
            var expected = new Replacer();
            expected.Add("a", "1");
            expected.Add("b", "2");

            var actual = deserializer.Convert(data);

            AssertReplacer(expected, actual);
        }

        [Test]
        public void TrimLines()
        {
            using var data = GetData(" a=1\n\tb=2  \n \n\nc=3");
            var expected = new Replacer();
            expected.Add("a", "1");
            expected.Add("b", "2");
            expected.Add("c", "3");

            var actual = deserializer.Convert(data);

            AssertReplacer(expected, actual);
        }

        [Test]
        public void IgnoreComments()
        {
            using var data = GetData("# comment\na=1\n #comment\nb=2");
            var expected = new Replacer();
            expected.Add("a", "1");
            expected.Add("b", "2");

            var actual = deserializer.Convert(data);

            AssertReplacer(expected, actual);
        }

        [Test]
        public void ThrowIfInvalidLine()
        {
            using var data = GetData("a=1\nb= 2");
            Assert.That(() => deserializer.Convert(data), Throws.InstanceOf<FormatException>());

            using var data2 = GetData("a=1\nb=xx");
            Assert.That(() => deserializer.Convert(data2), Throws.InstanceOf<FormatException>());

            using var data3 = GetData("aa=1\nb=x");
            Assert.That(() => deserializer.Convert(data2), Throws.InstanceOf<FormatException>());
        }

        private static BinaryFormat GetData(string text)
        {
            var binary = new BinaryFormat();
            new TextDataWriter(binary.Stream).Write(text);
            return binary;
        }

        private static void AssertReplacer(Replacer expected, Replacer actual)
        {
            Assert.That(actual.Map, Has.Count.EqualTo(expected.Map.Count));
            for (int i = 0; i < actual.Map.Count; i++) {
                Assert.That(actual.Map[i].Original, Is.EqualTo(expected.Map[i].Original));
                Assert.That(actual.Map[i].Modified, Is.EqualTo(expected.Map[i].Modified));
            }
        }
    }
}
