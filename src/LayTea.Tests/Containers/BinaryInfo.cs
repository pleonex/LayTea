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
namespace SceneGate.Games.ProfessorLayton.Tests.Containers
{
    using System;
    using System.IO;
    using System.Security.Cryptography;
    using NUnit.Framework;
    using YamlDotNet.Serialization;
    using YamlDotNet.Serialization.NamingConventions;
    using Yarhl.IO;

    public class BinaryInfo
    {
        public long Offset { get; set; }

        public long Length { get; set; }

        public string Sha256 { get; set; }

        public static BinaryInfo FromYaml(string path)
        {
            string yaml = File.ReadAllText(path);
            return new DeserializerBuilder()
                .WithNamingConvention(UnderscoredNamingConvention.Instance)
                .Build()
                .Deserialize<BinaryInfo>(yaml);
        }

        public void AssertIsEqual(IBinary decompressed)
        {
            Assert.That(decompressed.Stream.Offset, Is.EqualTo(Offset));
            Assert.That(decompressed.Stream.Length, Is.EqualTo(Length));

            using var sha256 = SHA256.Create();
            decompressed.Stream.Position = 0;
            sha256.ComputeHash(decompressed.Stream);
            string actualHash = BitConverter.ToString(sha256.Hash)
                .Replace("-", string.Empty)
                .ToLowerInvariant();
            Assert.That(actualHash, Is.EqualTo(Sha256));
        }
    }
}
