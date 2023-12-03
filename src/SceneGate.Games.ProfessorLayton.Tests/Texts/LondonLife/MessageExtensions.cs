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

    public static class MessageExtensions
    {
        public static void AssertIsEquivalent(this Message msg1, Message msg2)
        {
            Assert.That(msg1.Content, Has.Count.EqualTo(msg2.Content.Count), "Content count");
            for (int i = 0; i < msg1.Content.Count; i++) {
                if (msg1.Content[i] is MessageRawText text1 && msg2.Content[i] is MessageRawText text2) {
                    Assert.That(text1.Text, Is.EqualTo(text2.Text), $"text[{i}]");
                } else if (msg1.Content[i] is MessageFunction fnc1 && msg2.Content[i] is MessageFunction fnc2) {
                    fnc1.AssertIsEquivalent(fnc2);
                } else {
                    Assert.Fail($"Different types for {i}");
                }
            }

            if (msg1.QuestionOptions != null && msg2.QuestionOptions != null) {
                msg1.QuestionOptions.AssertIsEquivalent(msg2.QuestionOptions);
            } else if (msg1.QuestionOptions != null || msg2.QuestionOptions != null) {
                Assert.Fail("One option is null");
            }
        }

        public static void AssertIsEquivalent(this MessageFunction fnc1, MessageFunction fnc2)
        {
            Assert.That(fnc1.Id, Is.EqualTo(fnc2.Id), "ID");
            Assert.That(fnc1.Mnemonic, Is.EqualTo(fnc2.Mnemonic), "Mnemonic");
            Assert.That(fnc1.Argument, Is.EqualTo(fnc2.Argument), "Argument");
        }

        public static void AssertIsEquivalent(this QuestionOptions question1, QuestionOptions question2)
        {
            Assert.That(question1.DefaultIndex, Is.EqualTo(question2.DefaultIndex), "DefaultIndex");
            Assert.That(question1.PreSelectedIndex, Is.EqualTo(question2.PreSelectedIndex), "PreSelectedIndex");

            Assert.That(question1.Options, Has.Count.EqualTo(question2.Options.Count), "Options count");
            for (int i = 0; i < question1.Options.Count; i++) {
                Assert.That(question1.Options[i].Item1, Is.EqualTo(question2.Options[i].Item1), $"Item1 at {i}");
                Assert.That(question1.Options[i].Item2, Is.EqualTo(question2.Options[i].Item2), $"Item2 at {i}");
            }
        }
    }
}
