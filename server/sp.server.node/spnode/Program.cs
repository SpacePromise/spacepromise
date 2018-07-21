using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using spnode.world.procedural.Generators.Data.Alias;
using spnode.world.procedural.Generators.Noise;
using spnode.world.procedural.Utils.Noise;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace spnode
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var size = 1024;
            var increment = 0.1d;

//            var data = new Bgra32[size*size];
//            var noise = new OpenSimplexNoise();
//            for (var yIndex = 0; yIndex < size; yIndex+=1)
//            for (var xIndex = 0; xIndex < size; xIndex += 1)
//            {
//                var val = (byte) (noise.Evaluate(xIndex * increment, yIndex * increment) * 255);
//                data[xIndex + yIndex * size] = new Bgra32(val, val, val);
//            }

            var sw = Stopwatch.StartNew();

            var noiseSource = new NoiseSource();
            var data = new double[size * size];

            // 567ms
//            data.Add(NoiseSource.OpenSimplex2D(0, 0, 0, size, size, increment*2, increment*2));
//            data.Add(NoiseSource.OpenSimplex2D(0, 0, 0, size, size, increment /2, increment / 2).Mul(0.7));
//            data.Add(NoiseSource.OpenSimplex2D(0, 0, 0, size, size, increment / 10, increment / 10).Mul(0.5));

            data.Add(NoiseSource.OpenSimplex2D(DateTime.Now.Ticks, 0, 0, size, size, 5d, 10d, 2d));
//            
            var dataScaled = data
                .Select((i, val) => (byte) ((val) * 255));
            
            sw.Stop();
            Console.WriteLine("In " + sw.ElapsedMilliseconds + "ms");
            
            using (var fs = File.OpenWrite("./image.bmp"))
            using (var image = Image.LoadPixelData(dataScaled.AsPixelData(), size, size))
            {
                image.SaveAsBmp(fs);
                await fs.FlushAsync();
                fs.Close();
            }

            Console.WriteLine("Done.");
            Console.ReadLine();
        }
    }
}
