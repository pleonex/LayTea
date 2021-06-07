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
    using System.IO;
    using NUnit.Framework;
    using SceneGate.Games.ProfessorLayton.Containers;
    using SceneGate.Games.ProfessorLayton.Texts.LondonLife;
    using Yarhl.FileSystem;
    using Yarhl.Media.Text;

    [TestFixture]
    public class MessageCollection2PoContainerTests
    {
        [Test]
        public void NullBinaryThrows()
        {
            var converter = new MessageCollection2PoContainer();
            Assert.That(() => converter.Convert(null), Throws.ArgumentNullException);
        }

        [Test]
        public void MissingInitializerDoesNotThrow()
        {
            var messages = new MessageCollection();
            var converter = new MessageCollection2PoContainer();
            Assert.That(() => converter.Convert(messages), Throws.Nothing);
        }

        [Test]
        public void ConvertFileSucceeds()
        {
            string commonPath = Path.Combine(TestDataBase.RootFromOutputPath, "containers", "ll_common.darc");
            TestDataBase.IgnoreIfFileDoesNotExist(commonPath);

            using Node node = NodeFactory.FromFile(commonPath);
            Node msgNode = null;
            try {
                msgNode = node.TransformWith<BinaryDarc2Container>()
                    .Children[2]
                    .TransformWith<DencDecompression>()
                    .TransformWith<Binary2MessageCollection>();
            } catch {
                Assert.Ignore("Failed to obtain messages");
            }

            Assert.That(() => msgNode.TransformWith<MessageCollection2PoContainer, LondonLifeRegion>(LondonLifeRegion.Usa), Throws.Nothing);
            foreach (var child in msgNode.Children) {
                Assert.That(child.Format, Is.InstanceOf<Po>(), child.Name);
            }
        }
    }
}
