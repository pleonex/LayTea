﻿// Copyright (c) 2021 SceneGate

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
namespace Yarhl.Experimental.TestFramework;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using Yarhl.FileSystem;

public class NodeContainerInfo
{
    public string Name { get; set; }

    public string FormatType { get; set; }

    public BinaryInfo Stream { get; set; }

    public Dictionary<string, object> Tags { get; set; }

    public bool CheckChildren { get; set; }

    public Collection<NodeContainerInfo> Children { get; set; }

    public static NodeContainerInfo FromYaml(string path)
    {
        string yaml = File.ReadAllText(path);
        return new DeserializerBuilder()
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .Build()
            .Deserialize<NodeContainerInfo>(yaml);
    }

    public static NodeContainerInfo FromNode(Node node)
    {
        return new NodeContainerInfo {
            Name = node.Name,
            FormatType = node.Format?.GetType().FullName,
            Tags = node.Tags
                .Where(x => x.Value?.GetType().IsPrimitive || x.Value is string)
                .ToDictionary(x => x.Key, x => x.Value),
            Stream = BinaryInfo.FromStream(node.Stream),
            CheckChildren = true,
            Children = new(node.Children.Select(FromNode).ToArray()),
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
