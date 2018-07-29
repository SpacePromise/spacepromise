using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using spnode.world.procedural.Generators.Noise;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace spnode.app.docker
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var node = new SpNode();
            node.Start();

            //var random = new Random();
            //for (int index = 0; index < 100; index++)
            //{
            //    var seed = random.Next(1, int.MaxValue);
            //    await NewTerrain(seed);
            //}

            // Wait for node to exit
            while (node.IsRunning)
                Thread.Sleep(100);
        }

        private static async Task NewTerrain(int seed)
        {
            var sw = Stopwatch.StartNew();

            var size = 1024 * 2;
            var offsetX = size / 2;
            var offsetY = size / 2;
            var scale = 5d;
            var initialFrequency = 0.5d;
            var octaves = 6;
            var octaveOffsetScale = 100000;
            var amplitudePerOctave = 1.5d;
            var freqPerOctave = 0.5d;

            var data = NoiseSource.OpenSimplex2DOctaves(seed, size, offsetX, offsetY, scale, initialFrequency, octaves,
                octaveOffsetScale, amplitudePerOctave, freqPerOctave);
            data.Add((i, val) =>
            {
                var x = (double) i % size / size;
                var y = (double) i / size / size;
                var d = Math.Max(Math.Abs(x * 2 - 1), Math.Abs(y * 2 - 1));
                var f = NoiseSource.FalloffFunc(d, 1d, 5d);
                return f;
            });

            var minHeight = data.Min();
            var maxHeight = data.Max();

            var dataScaled = data.Select((i, val) => (byte) ((val / maxHeight) * 255));

            sw.Stop();
            Console.WriteLine("In " + sw.ElapsedMilliseconds + "ms");

            using (var fs = File.OpenWrite("./images/image" + seed + ".bmp"))
            using (var image = Image.LoadPixelData(
                dataScaled.AsPixelData((i, b) =>
                    new Bgra32(b < 255 / 2 ? b : (byte) 0, b < 255 / 2 ? b : (byte) 0, b)),
                size, size))
            {
                image.SaveAsBmp(fs);
                await fs.FlushAsync();
                fs.Close();
            }
        }
    }
}