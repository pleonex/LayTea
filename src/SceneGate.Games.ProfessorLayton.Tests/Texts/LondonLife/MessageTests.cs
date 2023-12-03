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
    public class MessageTests
    {
        [Test]
        public void AddText()
        {
            var msg = new Message();
            msg.Add("hello world!");

            Assert.That(msg.Content, Has.Count.EqualTo(1));
            Assert.That(msg.Content[0], Is.TypeOf<MessageRawText>());
            Assert.That(((MessageRawText)msg.Content[0]).Text, Is.EqualTo("hello world!"));
        }

        [Test]
        public void AddFunction()
        {
            var msg = new Message();
            msg.Add(MessageFunction.FromId(0xF3, null));

            Assert.That(msg.Content, Has.Count.EqualTo(1));
            Assert.That(msg.Content[0], Is.TypeOf<MessageFunction>());
            Assert.That(((MessageFunction)msg.Content[0]).Id, Is.EqualTo(0xF3));
        }
    }
}
