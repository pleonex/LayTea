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
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using YamlDotNet.Serialization;
    using YamlDotNet.Serialization.NamingConventions;

    /// <summary>
    /// Provide context to messages from embedded resources.
    /// </summary>
    internal class MessageContextProvider
    {
        private const string CharacterSection = "Characters";
        private static readonly Dictionary<LondonLifeRegion, string> ResourceInfo = new Dictionary<LondonLifeRegion, string>
        {
            { LondonLifeRegion.Usa, "msg_sections_us.yml" },
        };

        private readonly int firstNameIdx;
        private readonly int nameEntriesPerCharacter;
        private readonly int lastNamedDialogIdx;
        private readonly int dialogsPerCharacter;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageContextProvider" /> class.
        /// </summary>
        /// <param name="region">The region of the game.</param>
        public MessageContextProvider(LondonLifeRegion region)
        {
            Sections = new ReadOnlyCollection<MessageSection>(LoadInfo(region));

            var nameSection = Sections.FirstOrDefault(s => s.Name == CharacterSection);
            firstNameIdx = nameSection?.Start ?? -1;
            nameEntriesPerCharacter = nameSection?.Entries.Count ?? 0;

            lastNamedDialogIdx = (Sections.Skip(1).FirstOrDefault()?.Start - 1) ?? int.MaxValue;
            dialogsPerCharacter = Sections.FirstOrDefault()?.Entries.Count ?? 1;
        }

        /// <summary>
        /// Gets a read-only collection of section information.
        /// </summary>
        public ReadOnlyCollection<MessageSection> Sections { get; }

        /// <summary>
        /// Gets the message index of the character name saying the dialog or -1 if it's unknown.
        /// </summary>
        /// <param name="messageIdx">The index of the dialog.</param>
        /// <returns>Index of the character name or -1 if unknown.</returns>
        public int GetNameIndex(int messageIdx)
        {
            if (messageIdx > lastNamedDialogIdx) {
                return -1;
            }

            int charId = messageIdx / dialogsPerCharacter;
            int nameIdx = firstNameIdx + (charId * nameEntriesPerCharacter);
            return nameIdx;
        }

        private static IList<MessageSection> LoadInfo(LondonLifeRegion region)
        {
            if (!ResourceInfo.ContainsKey(region)) {
                throw new InvalidOperationException("Region not supported");
            }

            string resourceName = ResourceInfo[region];
            string resourcePath = $"{typeof(MessageContextProvider).Namespace}.{resourceName}";
            var assembly = typeof(MessageCollection2PoContainer).Assembly;

            using var stream = assembly.GetManifestResourceStream(resourcePath);
            using var reader = new StreamReader(stream);
            return new DeserializerBuilder()
                .WithNamingConvention(LowerCaseNamingConvention.Instance)
                .Build()
                .Deserialize<IList<MessageSection>>(reader);
        }
    }
}
