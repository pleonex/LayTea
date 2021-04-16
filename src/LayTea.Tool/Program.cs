﻿// Copyright (c) 2020 Benito Palacios Sánchez
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
namespace SceneGate.Games.ProfessorLayton.Tool
{
    using System;
    using SceneGate.Games.ProfessorLayton.Containers;
    using SceneGate.Games.ProfessorLayton.Texts.LondonLife;
    using Yarhl.FileSystem;
    using Yarhl.Media.Text;

    /// <summary>
    /// Main program class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Main entry-point.
        /// </summary>
        /// <param name="args">Application arguments.</param>
        public static void Main(string[] args)
        {
            args = new[] {
                "/workspaces/ProfessorLayton/src/LayTea.Tests/Resources/containers/ll_common.darc",
                "/workspaces/ProfessorLayton/artifacts/output",
            };

            if (args.Length != 2) {
                Console.WriteLine("Invalid number of arguments");
                return;
            }

            string input = args[0];
            string output = args[1];

            var textNodes = NodeFactory.FromFile(input)
                .TransformWith<BinaryDarc2Container>()
                .Children[2]
                .TransformWith<DencDecompression>()
                .TransformWith<Binary2MessageCollection>()
                .TransformWith<MessageCollection2PoContainer, LondonLifeRegion>(LondonLifeRegion.Usa)
                .Children;
            foreach (var node in textNodes) {
                node.TransformWith<Po2Binary>()
                .Stream.WriteTo($"{output}/{node.Name}.po");
            }
        }
    }
}
