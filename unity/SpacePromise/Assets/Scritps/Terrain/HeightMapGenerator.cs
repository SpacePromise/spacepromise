using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.Win32;
using UnityEngine;

[ExecuteInEditMode]
public class HeightMapGenerator : MonoBehaviour
{
    public int Seed;

    public int Width;
    public int Height;
    public float Scale = 100;
    public int Octaves;
    public float AmplitudePerOctave;
    public float FreqPerOctave;
    public Vector2 offset;
    public AnimationCurve HeightPower;

    private float[,] map;

    public bool isGenerateRequested;

    public double lastRenderIn;
    private float[,] falloffMap;


    private void Start()
    {
    }

    private void Update()
    {
        if (this.isGenerateRequested)
        {
            this.isGenerateRequested = false;
            this.GenerateMap();
        }
    }

    private void GenerateMap()
    {
        var sw = Stopwatch.StartNew();

        var width = this.Width;
        var height = this.Height;
        this.map = NoiseGenerator
            .GenerateBlank(width, height)
            // Base
            .OctaveNoisePass(
                width, height,
                this.Scale, this.offset, this.Seed,
                this.Octaves, this.AmplitudePerOctave, this.FreqPerOctave)
            // Falloff
            .Add(width, height, (x, y, val) =>
                NoiseGenerator.FalloffFunc(
                    Mathf.Max(
                        Mathf.Abs(x / (float) width * 2 - 1),
                        Mathf.Abs(y / (float) height * 2 - 1)),
                    1, 2.2f))
            // Height power
            .Multiply(width, height, (x, y, val) => this.HeightPower.Evaluate(val));

        sw.Stop();
        this.lastRenderIn = sw.Elapsed.TotalMilliseconds;

        FindObjectOfType<HeightMapDisplay>().DrawHeightMap(this.map);
    }
}

public static class NoiseGenerator
{
    public static float[,] Multiply(this float[,] data, int width, int height, Func<int, int, float, float> func)
    {
        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                data[x, y] *= func(x, y, data[x, y]);
            }
        }

        return data;
    }

    public static float[,] Add(this float[,] data, int width, int height, Func<int, int, float, float> func)
    {
        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                data[x, y] = data[x, y] - func(x, y, data[x, y]);
            }
        }

        return data;
    }

    public static float[,] Subtract(this float[,] data, int width, int height, float[,] value)
    {
        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                data[x, y] = data[x, y] - value[x, y];
            }
        }

        return data;
    }

    public static float FalloffFunc(float value, float falloff, float baseSize)
    {
        return Mathf.Pow(value, falloff) / (Mathf.Pow(value, falloff) + Mathf.Pow(baseSize - baseSize * value, falloff));
    }

    public static float[,] FalloffPass(this float[,] data, int size, Func<float, float> func = null)
    {
        if (func == null)
            func = v => v;

        for (var x = 0; x < size / 2; x++)
        {
            for (var y = 0; y < size / 2; y++)
            {
                var xval = x / (float)size * 2 - 1;
                var yval = y / (float)size * 2 - 1;
                var val = func(Mathf.Max(Mathf.Abs(xval), Mathf.Abs(yval)));
                data[x, y] = val;
                data[size - x - 1, y] = val;
                data[size - x - 1, size - y - 1] = val;
                data[x, size - y - 1] = val;
            }
        }

        return data;
    }

    public static float[,] OctaveNoisePass(this float[,] data, int width, int height, float scale, Vector2 offset, int seed, int octaves, float amplitudePerOctave, float freqPerOctave)
    {
        var random = new System.Random(seed);

        var amplitude = 1f;
        var maxNoise = 0f;
        var octaveOffsets = new Vector2[octaves];
        for (int index = 0; index < octaves; index++)
        {
            octaveOffsets[index] = new Vector2(
                random.Next(-100000, 100000) + offset.x,
                random.Next(-100000, 100000) + offset.y);
            maxNoise += amplitude;
            amplitude *= amplitudePerOctave;
        }

        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                amplitude = 1f;
                var freq = 1f;
                var value = 0f;

                for (int octave = 0; octave < octaves; octave++)
                {
                    value += Mathf.PerlinNoise(
                                  (x + octaveOffsets[octave].x) / scale * freq,
                                  (y + octaveOffsets[octave].y) / scale * freq) * amplitude;

                    amplitude *= amplitudePerOctave;
                    freq *= freqPerOctave;   
                }

                data[x, y] = value / maxNoise;
            }
        }

        return data;
    }

    public static float[,] GenerateBlank(int width, int height)
    {
        return new float[width, height];
    }
}
