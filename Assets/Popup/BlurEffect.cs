using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlurEffect : MonoBehaviour
{
    public Material material;

    private RenderTexture sourceBlurTexture;

    private RenderTexture horBlurTexture;

    private RenderTexture targetBlurTexture;

    private Camera camera;

    void Awake()
    {
        int width = Screen.width / 4;
        int height = Screen.height / 4;
        sourceBlurTexture = new RenderTexture(width, height, 16);
        horBlurTexture = new RenderTexture(width, height, 0);
        targetBlurTexture = new RenderTexture(width, height, 0);

        camera = Camera.main;
    }

    private void OnDestroy()
    {
        sourceBlurTexture.Release();
        horBlurTexture.Release();
        targetBlurTexture.Release();
    }

    private void OnEnable()
    {
        camera.targetTexture = sourceBlurTexture;

        material.SetVector("_TexelSize", new Vector4(1f / sourceBlurTexture.width, 1f / sourceBlurTexture.height, 0f, 0f));
    }

    private void LateUpdate()
    {
        Graphics.Blit(sourceBlurTexture, horBlurTexture, material, 0);
        Graphics.Blit(horBlurTexture, targetBlurTexture, material, 1);
    }

    private void OnDisable()
    {
        camera.targetTexture = null;
    }

    public Texture GetTexture()
    {
        return targetBlurTexture;
    }
}
