using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

        public Task<string> GenerateAsync(AliasKind kind) => this.GenerateAsync(kind, new Random());

        public Task<string> GenerateAsync(AliasKind kind, int seed) => this.GenerateAsync(kind, new Random(seed));

        public async Task<string> GenerateAsync(AliasKind kind, Random random)
        {
            return string.Concat((await this.GetChainAsync(kind)).Chain(random)).ToTitleCase();
        }

        private async Task<MarkovChain<char>> GetChainAsync(AliasKind kind)
        {
            if (this.chainCache.ContainsKey(kind))
                return this.chainCache[kind];

            var chain = new MarkovChain<char>(KindOrder[kind]);
            foreach (var item in await GetSourceAsync(kind))
                chain.Add(item);

            if (!this.chainCache.ContainsKey(kind))
                this.chainCache.Add(kind, chain);

            return chain;
        }

        private static async Task<IEnumerable<string>> GetSourceAsync(AliasKind kind)
        {
            switch (kind)
            {
                case AliasKind.City:
                    return JsonConvert.DeserializeObject<List<string>>(await ProceduralSources.ProceduralSourceFiles.AliasCitiesJsonAsync());
                case AliasKind.Island:
                    return JsonConvert.DeserializeObject<List<string>>(await ProceduralSources.ProceduralSourceFiles.AliasIslandsJsonAsync());
                case AliasKind.Planet:
                    return JsonConvert.DeserializeObject<List<string>>(await ProceduralSources.ProceduralSourceFiles.AliasPlanetsJsonAsync());
                default: return new List<string>();
            }
        }
    }
}