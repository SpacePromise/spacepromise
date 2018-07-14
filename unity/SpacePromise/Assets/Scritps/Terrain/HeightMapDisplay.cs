using UnityEngine;

public class HeightMapDisplay : MonoBehaviour
{
    public Renderer TextureRenderer;

    public void DrawHeightMap(float[,] heightMap)
    {
        var texture = TextureHelper.CreateGrayscaleTexture(heightMap);

        this.TextureRenderer.sharedMaterial.mainTexture = texture;
        this.TextureRenderer.transform.localScale = new Vector3(texture.width, 1, texture.height);
    }
}