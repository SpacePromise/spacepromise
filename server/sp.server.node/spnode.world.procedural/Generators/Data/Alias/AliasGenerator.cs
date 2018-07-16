using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using spnode.world.procedural.Extensions;
using spnode.world.procedural.Utils.MarkovChain;

namespace spnode.world.procedural.Generators.Data.Alias
{
    public class AliasGenerator
    {
        private static readonly Dictionary<AliasKind, int> KindOrder = new Dictionary<AliasKind, int>
        {
            {AliasKind.City, 4},
            {AliasKind.Island, 3},
            {AliasKind.Planet, 3},
        };

        private readonly Dictionary<AliasKind, MarkovChain<char>> chainCache = new Dictionary<AliasKind, MarkovChain<char>>();

        public string Generate(AliasKind kind) => this.Generate(kind, new Random());

        public string Generate(AliasKind kind, int seed) => this.Generate(kind, new Random(seed));

        public string Generate(AliasKind kind, Random random)
        {
            return string.Concat(this.GetChain(kind).Chain(random)).ToTitleCase();
        }

        private MarkovChain<char> GetChain(AliasKind kind)
        {
            if (this.chainCache.ContainsKey(kind))
                return this.chainCache[kind];

            var chain = new MarkovChain<char>(KindOrder[kind]);
            foreach (var item in GetSource(kind))
                chain.Add(item);

            if (!this.chainCache.ContainsKey(kind))
                this.chainCache.Add(kind, chain);

            return chain;
        }

        private static IEnumerable<string> GetSource(AliasKind kind)
        {
            switch (kind)
            {
                case AliasKind.City:
                    return JsonConvert.DeserializeObject<List<string>>(ProceduralSources.ProceduralSourceFiles.AliasCitiesJson());
                case AliasKind.Island:
                    return JsonConvert.DeserializeObject<List<string>>(ProceduralSources.ProceduralSourceFiles.AliasPlanetsJson());
                case AliasKind.Planet:
                    return JsonConvert.DeserializeObject<List<string>>(ProceduralSources.ProceduralSourceFiles.AliasIslandsJson());
                default: return new List<string>();
            }
        }
    }
}