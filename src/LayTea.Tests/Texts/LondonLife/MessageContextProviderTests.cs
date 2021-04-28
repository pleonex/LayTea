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
namespace SceneGate.Games.ProfessorLayton.Tests.Texts.LondonLife
{
    using System.Linq;
    using NUnit.Framework;
    using SceneGate.Games.ProfessorLayton.Texts.LondonLife;

    [TestFixture]
    public class MessageContextProviderTests
    {
        [Test]
        public void UnsupportedRegionsThrows()
        {
            Assert.That(() => new MessageContextProvider(LondonLifeRegion.Australia), Throws.InvalidOperationException);
            Assert.That(() => new MessageContextProvider(LondonLifeRegion.Japan), Throws.InvalidOperationException);
        }

        [Test]
        public void SupportedRegionLoadInfo()
        {
            var context = new MessageContextProvider(LondonLifeRegion.Usa);
            Assert.That(context.Sections, Has.Count.GreaterThan(0));
            Assert.That(context.Sections.Any(s => s.Name == "Characters"), Is.True);
        }

        [Test]
        public void GetNameIndexInRange()
        {
            var context = new MessageContextProvider(LondonLifeRegion.Usa);
            Assert.That(context.GetNameIndex(6), Is.GreaterThan(0));
        }

        [Test]
        public void GetNameIndexOutOfRange()
        {
            var context = new MessageContextProvider(LondonLifeRegion.Usa);
            Assert.That(context.GetNameIndex(int.MaxValue), Is.EqualTo(-1));
        }
    }
}
