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
namespace SceneGate.Games.ProfessorLayton.Tests;

using System;
using System.IO;
using System.Security.Cryptography;
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

    public static BinaryInfo FromStream(Stream stream)
    {
        if (stream is null) {
            return null;
        }

        stream.Position = 0;
        using var sha256 = SHA256.Create();
        sha256.ComputeHash(stream);
        string hash = BitConverter.ToString(sha256.Hash)
            .Replace("-", string.Empty)
            .ToLowerInvariant();

        long offset = stream is DataStream dataStream ? dataStream.Offset : 0;

        return new BinaryInfo {
            Length = stream.Length,
            Offset = offset,
            Sha256 = hash,
        };
    }

    public void ToYamlFile(string path)
    {
        var yaml = new SerializerBuilder()
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .Build()
            .Serialize(this);
        File.WriteAllText(path, yaml);
    }
}
