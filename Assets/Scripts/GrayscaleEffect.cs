using UnityEngine;
[ExecuteInEditMode]
public class GrayscaleEffect : MonoBehaviour
{
    [SerializeField] private Material _grayscaleMaterial;
    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (_grayscaleMaterial != null)
            Graphics.Blit(src, dest, _grayscaleMaterial); // Ӧ�ò��ʵ���Ļ
        else
            Graphics.Blit(src, dest); // �޲���ʱֱ�����ԭͼ
    }
}
