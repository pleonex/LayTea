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
namespace SceneGate.Games.ProfessorLayton.PerformanceTests
{
    using System;
    using System.IO;
    using BenchmarkDotNet.Attributes;
    using SceneGate.Games.ProfessorLayton.Containers;
    using Yarhl.IO;

    [MemoryDiagnoser]
    public class LzssdCompressionTests
    {
        private DataStream stream;

        [ParamsAllValues]
        public TestStreamType Type { get; set; }

        [GlobalSetup]
        public void SetUp()
        {
            switch (Type) {
                case TestStreamType.Random:
                    var random = new Random();
                    stream = new DataStream();
                    for (int i = 0; i < 800 * 1024; i++) {
                        stream.WriteByte((byte)random.Next(256));
                    }

                    break;

                case TestStreamType.MsgText:
                    string commonPath = Path.Combine(TestData.RootFromOutputPath, "containers", "ll_common.darc");
                    stream = DataStreamFactory.FromFile(commonPath, FileOpenMode.Read, 292, 384261);
                    break;
            }
        }

        [GlobalCleanup]
        public void CleanUp()
        {
            stream?.Dispose();
        }

        [Benchmark]
        public void Encode()
        {
            var compression = new LzssdCompression();
            var output = compression.Convert(stream);
            output.Dispose();
        }
    }
}
