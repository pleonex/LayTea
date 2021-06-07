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
    using NUnit.Framework;
    using SceneGate.Games.ProfessorLayton.Texts;
    using Yarhl.Media.Text;

    [TestFixture]
    public class PoTableReplacerTests
    {
        [Test]
        public void Guards()
        {
            Assert.That(() => new PoTableReplacer().Convert(null), Throws.ArgumentNullException);
            Assert.That(() => new PoTableReplacer().Initialize(null), Throws.ArgumentNullException);
        }

        [Test]
        public void DoesNotThrowIfNotInitialized()
        {
            Po po = new Po();
            Assert.That(() => new PoTableReplacer().Convert(po), Throws.Nothing);
        }

        [Test]
        public void ConvertForward()
        {
            var replacer = new Replacer();
            replacer.Add("a", "b");

            var converterParams = new PoTableReplacerParams {
                Replacer = replacer,
                TransformForward = true,
            };

            Po expected = new Po();
            expected.Add(new PoEntry { Original = "abc", Translated = "cba" });

            var converter = new PoTableReplacer();
            converter.Initialize(converterParams);
            var actual = converter.Convert(expected);

            Assert.That(actual, Is.SameAs(expected));
            Assert.That(actual.Entries, Has.Count.EqualTo(1));
            Assert.That(actual.Entries[0].Original, Is.EqualTo("bbc"));
            Assert.That(actual.Entries[0].Translated, Is.EqualTo("cbb"));
        }

        [Test]
        public void ConvertBackward()
        {
            var replacer = new Replacer();
            replacer.Add("a", "b");

            var converterParams = new PoTableReplacerParams {
                Replacer = replacer,
                TransformForward = false,
            };

            Po expected = new Po();
            expected.Add(new PoEntry { Original = "abc", Translated = "cba" });

            var converter = new PoTableReplacer();
            converter.Initialize(converterParams);
            var actual = converter.Convert(expected);

            Assert.That(actual, Is.SameAs(expected));
            Assert.That(actual.Entries, Has.Count.EqualTo(1));
            Assert.That(actual.Entries[0].Original, Is.EqualTo("aac"));
            Assert.That(actual.Entries[0].Translated, Is.EqualTo("caa"));
        }
    }
}
