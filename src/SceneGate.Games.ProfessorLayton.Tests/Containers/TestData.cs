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
namespace SceneGate.Games.ProfessorLayton.Tests.Containers
{
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using NUnit.Framework;

    public static class TestData
    {
        public static IEnumerable DarcParams {
            get => GetStreamAndInfoCollection("darc.txt");
        }

        public static string ContainersResources {
            get => Path.Combine(TestDataBase.RootFromOutputPath, "containers");
        }

        public static IEnumerable GetStreamAndInfoCollection(string listName)
        {
            return TestDataBase.ReadTestListFile(Path.Combine(ContainersResources, listName))
                    .Select(line => line.Split(','))
                    .Select(data => new TestFixtureData(
                        Path.Combine(ContainersResources, data[0]),
                        Path.Combine(ContainersResources, data[1]))
                        .SetArgDisplayNames(data[0], data[1]));
        }

        public static IEnumerable GetSubstreamAndInfoCollection(string listName)
        {
            string path = Path.Combine(ContainersResources, listName);
            IEnumerable<string[]> lines = TestDataBase.ReadTestListFile(path)
                    .Select(line => line.Split(','));
            foreach (string[] info in lines) {
                yield return new TestCaseData(
                    Path.Combine(ContainersResources, info[1]),
                    int.Parse(info[2]),
                    int.Parse(info[3]),
                    Path.Combine(ContainersResources, info[0]))
                    .SetArgDisplayNames(info[0]);
            }
        }
    }
}
