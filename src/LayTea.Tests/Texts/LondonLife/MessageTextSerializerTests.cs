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
    using System;
    using NUnit.Framework;
    using SceneGate.Games.ProfessorLayton.Texts.LondonLife;

    [TestFixture]
    public class MessageTextSerializerTests
    {
        private readonly MessageTextSerializer serializer = new MessageTextSerializer();

        [Test]
        public void EmptyMessageReturnsEmpty()
        {
            var message = new Message();
            AssertSerialization(message, string.Empty);
        }

        [Test]
        public void SerializeSeveralTimesDoesNotAffect()
        {
            var message = new Message();
            message.Add("hello");
            AssertSerialization(message, "hello");
            AssertSerialization(message, "hello");
        }

        [Test]
        public void SerializeRawText()
        {
            var message = new Message();
            message.Add("hello");
            message.Add(" world!");
            AssertSerialization(message, "hello world!");
        }

        [Test]
        public void SerializeRawTextWithSpecialChars()
        {
            var message = new Message();
            message.Add("{hello\\ world}\ntest!");
            AssertSerialization(message, "{{hello\\\\ world}}" + Environment.NewLine + "test!");
        }

        [Test]
        public void SerializeFunction()
        {
            var message = new Message();
            message.Add(new MessageFunction(0xF2, "color", 1));
            message.Add(new MessageFunction(0xFB, "save_load", null));
            AssertSerialization(message, "{color(1)}{save_load()}");
        }

        [Test]
        public void SerializeQuestion()
        {
            var message = new Message();
            message.QuestionOptions = new QuestionOptions {
                DefaultIndex = 1,
                PreSelectedIndex = 2,
            };
            message.QuestionOptions.Options.Add((1, "Answer 1"));
            message.QuestionOptions.Options.Add((0, "Answer 2"));
            message.QuestionOptions.Options.Add((2, "Answer 3"));

            string text = "{options(default:1,selected:2)}" + Environment.NewLine +
                "- 1: Answer 1" + Environment.NewLine +
                "- 0: Answer 2" + Environment.NewLine +
                "- 2: Answer 3" + Environment.NewLine;
            AssertSerialization(message, text);
        }

        [Test]
        public void SerializeComplexMessage()
        {
            var message = new Message();
            message.Add("{hello\\ world}");
            message.Add(new MessageFunction(0xF2, "color", 1));
            message.Add("\ntest");
            message.QuestionOptions = new QuestionOptions {
                DefaultIndex = 0,
                PreSelectedIndex = 0,
            };
            message.QuestionOptions.Options.Add((0, "Answer 1"));

            string text = "{{hello\\\\ world}}{color(1)}" + Environment.NewLine +
                "test" +
                "{options(default:0,selected:0)}" + Environment.NewLine +
                "- 0: Answer 1" + Environment.NewLine;
            AssertSerialization(message, text);
        }

        [Test]
        public void SerializeStopIfBoxCommand()
        {
            var message = new Message();
            message.Add("box1");
            message.Add(new MessageFunction(0xF1, "next_box", null));
            message.Add("box2");

            AssertSerialization(message, "box1", "box2");
        }

        private void AssertSerialization(Message message, params string[] text)
        {
            Assert.That(serializer.Serialize(message), Is.EqualTo(text));
            Assert.That(() => serializer.Deserialize(text), Throws.InstanceOf<NotImplementedException>());
        }
    }
}
