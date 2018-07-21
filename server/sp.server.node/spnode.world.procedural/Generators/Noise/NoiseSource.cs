using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using spnode.world.procedural.Utils.Noise;
using SixLabors.ImageSharp.PixelFormats;

namespace spnode.world.procedural.Generators.Noise
{
    public class NoiseSource
    {
        private static readonly Dictionary<long, OpenSimplexNoise> OpenSimplex = new Dictionary<long, OpenSimplexNoise>();

        private static OpenSimplexNoise GetOpenSimplexNoise(long seed)
        {
            if (!OpenSimplex.ContainsKey(seed))
                OpenSimplex.Add(seed, new OpenSimplexNoise(seed));
            return OpenSimplex[seed];
        }

        public static double OpenSimplex0D(long seed, double x, double y, double scale, double freq, double amplitude) =>
            GetOpenSimplexNoise(seed).Evaluate(x / scale * freq, y / scale * freq) * amplitude;

        public static double[] OpenSimplex2D(
            long seed, 
            double offsetX, double offsetY, 
            int width, int height,
            double scale, double freq, double amplitude)
        {
            var noise = GetOpenSimplexNoise(seed);
            var size = width * height;
            var data = new double[size];
            for (var indexY = 0; indexY < height; indexY++)
            for (var indexX = 0; indexX < width; indexX++)
                data[indexX + indexY * width] = noise.Evaluate(
                                                    (offsetX + indexX) / scale * freq,
                                                    (offsetY + indexY) / scale * freq)
                                                * amplitude;
            return data;
        }
    }
    
    public static class NoiseExtensions 
    {
        public static TDest[] Select<TSource, TDest>(this TSource[] source, Func<int, TSource, TDest> func)
        {
            var dest = new TDest[source.Length];
            for (var index = 0; index < source.Length; index++)
                dest[index] = func(index, source[index]);
            return dest;
        }

        public static T[] Apply<T>(this T[] source, Func<int, T, T> func)
        {
            for (var index = 0; index < source.Length; index++)
                source[index] = func(index, source[index]);
            return source;
        }

        
        public static double[] Add(this double[] source, double add) =>
            source.Add((i, val) => add);
        
        public static double[] Add(this double[] source, double[] data) =>
            source.Add((i, val) => data[i]);

        public static double[] Add(this double[] source, Func<int, double, double> func) => 
            source.Apply((i, val) => val + func(i, val));

        public static double[] Mul(this double[] source, double mul) =>
            source.Mul((i, val) => mul);
        
        public static double[] Mul(this double[] source, double[] data) =>
            source.Mul((i, val) => data[i]);
        
        public static double[] Mul(this double[] source, Func<int, double, double> func) =>
            source.Apply((i, val) => val * func(i, val));

        public static ReadOnlySpan<Bgra32> AsPixelData(this byte[] source)
        {
            return source.Select((index, val) => new Bgra32(val, val, val));
        }
    } 
}