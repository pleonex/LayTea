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
    using NUnit.Framework;
    using SceneGate.Games.ProfessorLayton.Texts.LondonLife;

    [TestFixture]
    public class MessageFunctionTests
    {
        [Test]
        public void ConstructorSetProperties()
        {
            var func = new MessageFunction(0xFA, "test", 4);
            Assert.That(func.Id, Is.EqualTo(0xFA));
            Assert.That(func.Mnemonic, Is.EqualTo("test"));
            Assert.That(func.Argument, Is.EqualTo(4));
        }

        [Test]
        public void CreateFromId()
        {
            var func = MessageFunction.FromId(0xF3, 104);
            Assert.That(func.Id, Is.EqualTo(0xF3));
            Assert.That(func.Mnemonic, Is.EqualTo("wait"));
            Assert.That(func.Argument, Is.EqualTo(104));
        }

        [Test]
        public void CreateFromMnemonic()
        {
            var func = MessageFunction.FromMnemonic("wait", 104);
            Assert.That(func.Id, Is.EqualTo(0xF3));
            Assert.That(func.Mnemonic, Is.EqualTo("wait"));
            Assert.That(func.Argument, Is.EqualTo(104));
        }
    }
}
