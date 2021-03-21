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
namespace SceneGate.Games.ProfessorLayton.Texts.LondonLife
{
    using System;

    /// <summary>
    /// A function part of the content of a message.
    /// </summary>
    public class MessageFunction : IMessageElement
    {
        private const int FirstFunction = 0xF0;
        private static readonly string[] Mnemonics = new[] {
            null,   // new line, part of raw text
            "next_box",
            "color",
            "wait",
            "name",
            null,   // user input, part of Message class
            null,   // not used
            null,   // not used
            null,   // not used
            "variable",
            "unknown_FA",
            "save_load",
            "unknown_FC",
            null,   // not used
            null,   // not used
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageFunction" /> class.
        /// </summary>
        /// <param name="id">The ID of the function.</param>
        /// <param name="mnemonic">The mnemonic of the message.</param>
        /// <param name="arg">Optional argument for the function call.</param>
        public MessageFunction(int id, string mnemonic, short? arg) =>
            (Id, Mnemonic, Argument) = (id, mnemonic, arg);

        /// <summary>
        /// Gets the ID of the function.
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// Gets the mnemonic of the function.
        /// </summary>
        public string Mnemonic { get; }

        /// <summary>
        /// Gets or sets the argument of the function if any.
        /// </summary>
        public short? Argument { get; set; }

        /// <summary>
        /// Creates a new function from its ID.
        /// </summary>
        /// <param name="id">The ID of the function.</param>
        /// <param name="arg">The argument of the function, if any.</param>
        /// <returns>A new message function.</returns>
        public static MessageFunction FromId(int id, short? arg)
        {
            int idx = id - FirstFunction;
            return new MessageFunction(id, Mnemonics[idx], arg);
        }

        /// <summary>
        /// Creates a new function from its mnemonic.
        /// </summary>
        /// <param name="mnemonic">The mnemonic of the function.</param>
        /// <param name="arg">The argument of the function, if any.</param>
        /// <returns>A new message function.</returns>
        public static MessageFunction FromMnemonic(string mnemonic, short? arg)
        {
            int idx = Array.IndexOf(Mnemonics, mnemonic);
            return new MessageFunction(FirstFunction + idx, mnemonic, arg);
        }
    }
}
