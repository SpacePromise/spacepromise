using System;
using System.Collections.Generic;
using System.Text;
using spnode.world.procedural.Properties;

namespace spnode.world.procedural.ProceduralSources
{
    public static class ProceduralSourceFiles
    {
        public static string AliasCitiesJson()
        {
            return Encoding.UTF8.GetString(Resources.cities);
        }

        public static string AliasIslandsJson()
        {
            return Encoding.UTF8.GetString(Resources.islands);
        }

        public static string AliasPlanetsJson()
        {
            return Encoding.UTF8.GetString(Resources.planets);
        }
    }
}
