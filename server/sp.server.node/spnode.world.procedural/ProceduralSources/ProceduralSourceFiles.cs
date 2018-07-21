using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace spnode.world.procedural.ProceduralSources
{
    public static class ProceduralSourceFiles
    {
        private static readonly Dictionary<Resource, string> resourceNameMap = new Dictionary<Resource, string>
        {
            {Resource.AliasCities, "spnode.world.procedural.ProceduralSources.Alias.cities.json"},
            {Resource.AliasIslands, "spnode.world.procedural.ProceduralSources.Alias.islands.json"},
            {Resource.AliasPlanets, "spnode.world.procedural.ProceduralSources.Alias.planets.json"}
        };
        
        public static Task<string> AliasCitiesJsonAsync()
        {
            return AsString(GetResourceFile(Resource.AliasCities));
        }

        public static Task<string> AliasIslandsJsonAsync()
        {
            return AsString(GetResourceFile(Resource.AliasIslands));
        }

        public static Task<string> AliasPlanetsJsonAsync()
        {
            return AsString(GetResourceFile(Resource.AliasPlanets));
        }

        private static Task<string> AsString(Stream stream) => 
            new StreamReader(stream).ReadToEndAsync();

        private static Stream GetResourceFile(Resource resource) => 
            typeof(ProceduralSourceFiles).GetTypeInfo().Assembly.GetManifestResourceStream(resourceNameMap[resource]);

        private enum Resource
        {
            AliasCities,
            AliasIslands,
            AliasPlanets
        }
    }
}
