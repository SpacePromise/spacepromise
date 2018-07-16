using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using spnode.world.procedural.Generators.Data.Alias;

namespace spnode
{
    class Program
    {
        static void Main(string[] args)
        {
            var nameService = new AliasGenerator();
            for (int index = 0; index < 100; index++)
            {
                Console.WriteLine(nameService.Generate(AliasKind.City));
            }

            Console.ReadLine();
        }
    }
}
