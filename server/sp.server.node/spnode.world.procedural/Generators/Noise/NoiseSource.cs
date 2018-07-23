using System;
using System.Buffers;
using System.Collections.Generic;
using System.Numerics;
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
                data[indexX + indexY * width] = ((noise.Evaluate(
                                                    (offsetX + indexX) / scale * freq,
                                                    (offsetY + indexY) / scale * freq) + 1) / 2d) * amplitude;
            return data;
        }

        public static double[] OpenSimplex2DOctaves(
            int seed,
            int size, int offsetX, int offsetY, double scale, double initialFrequency, 
            int octaves, int octaveOffsetScale, double amplitudePerOctave, double freqPerOctave)
        {
            var random = new Random(seed);

            var data = new double[size * size];

            var currentAmplitude = 1d;
            var currentFrequency = initialFrequency;
            var maxNoise = 0d;
            var octaveOffsets = new Vector2[octaves];

            for (var octaveIndex = 0; octaveIndex < octaves; octaveIndex++)
            {
                octaveOffsets[octaveIndex] = new Vector2(
                    random.Next(-octaveOffsetScale, octaveOffsetScale) + offsetX,
                    random.Next(-octaveOffsetScale, octaveOffsetScale) + offsetY);
                data.Add(OpenSimplex2D(seed, octaveOffsets[octaveIndex].X, octaveOffsets[octaveIndex].Y, size, size, scale, currentFrequency, currentAmplitude));
                maxNoise += currentAmplitude;
                currentAmplitude *= amplitudePerOctave;
                currentFrequency *= freqPerOctave;
            }

            return data.Mul(1d / maxNoise);
        }

        public static double FalloffFunc(double value, double falloff, double baseSize) => 
            Math.Pow(value, falloff) / (Math.Pow(value, falloff) + Math.Pow(baseSize - baseSize * value, falloff));
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

        public static ReadOnlySpan<Bgra32> AsPixelData(this byte[] source, Func<int, byte, Bgra32> func) => 
            source.Select(func);
    } 
}