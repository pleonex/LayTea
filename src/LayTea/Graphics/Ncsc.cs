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
using Texim.Compressions.Nitro;

namespace SceneGate.Games.ProfessorLayton.Graphics
{
    /// <summary>
    /// Screen map with format NCSC.
    /// </summary>
    public class Ncsc : IScreenMap
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Ncsc" /> class.
        /// </summary>
        public Ncsc()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Ncsc" /> class.
        /// </summary>
        /// <param name="width">The width of the image.</param>
        /// <param name="height">The height of the image.</param>
        /// <param name="maps">The map information.</param>
        public Ncsc(int width, int height, MapInfo[] maps) =>
            (Width, Height, Maps) = (width, height, maps);

        /// <inheritdoc/>
        public MapInfo[] Maps { get; init; }

        /// <inheritdoc/>
        public int Width { get; init; }

        /// <inheritdoc/>
        public int Height { get; init; }

        /// <summary>
        /// Gets or sets the name of the indexed image.
        /// </summary>
        public string ImageName { get; set; }
    }
}
