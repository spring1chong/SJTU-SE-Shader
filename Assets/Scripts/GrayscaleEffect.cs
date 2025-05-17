using UnityEngine;
[ExecuteInEditMode]
public class GrayscaleEffect : MonoBehaviour
{
    [SerializeField] private Material _grayscaleMaterial;
    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (_grayscaleMaterial != null)
            Graphics.Blit(src, dest, _grayscaleMaterial); // 应用材质到屏幕
        else
            Graphics.Blit(src, dest); // 无材质时直接输出原图
    }
}
