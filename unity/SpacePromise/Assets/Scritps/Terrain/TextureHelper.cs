using UnityEngine;

public static class TextureHelper
{
    public static Texture2D CreateGrayscaleTexture(float[,] data)
    {
        var width = data.GetLength(0);
        var height = data.GetLength(1);

        var texture = new Texture2D(width, height);

        var color = new Color[width * height];
        for (var verticalIndex = 0; verticalIndex < height; verticalIndex++)
        {
            for (var horizontalIndex = 0; horizontalIndex < width; horizontalIndex++)
            {
                color[verticalIndex * width + horizontalIndex] = Color.Lerp(
                    Color.black, Color.white, data[horizontalIndex, verticalIndex]);
            }
        }
        texture.SetPixels(color);
        texture.Apply();
        return texture;
    }
}