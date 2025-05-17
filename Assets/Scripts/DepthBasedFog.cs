using UnityEngine;
using System;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class DepthBasedFog : MonoBehaviour
{
    public Shader fogShader;
    public Color fogColor = Color.gray;
    public float fogDensity = 0.01f;
    public float fogStartHeight = 0.0f;
    public float fogHeightFalloff = 0.1f;

    private Material fogMaterial;
    private Camera mainCamera;

    void OnEnable()
    {
        mainCamera = GetComponent<Camera>();
        mainCamera.depthTextureMode |= DepthTextureMode.Depth;
    }

    [ImageEffectOpaque]
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (fogShader == null)
        {
            Graphics.Blit(source, destination);
            return;
        }

        if (fogMaterial == null)
        {
            fogMaterial = new Material(fogShader);
            fogMaterial.hideFlags = HideFlags.HideAndDontSave;
        }

        float camNear = mainCamera.nearClipPlane;
        float camFar = mainCamera.farClipPlane;
        float camFov = mainCamera.fieldOfView;
        float camAspect = mainCamera.aspect;

        Matrix4x4 frustumCorners = Matrix4x4.identity;

        float fovWHalf = camFov * 0.5f;
        Vector3 toRight = mainCamera.transform.right * Mathf.Tan(fovWHalf * Mathf.Deg2Rad) * camAspect;
        Vector3 toTop = mainCamera.transform.up * Mathf.Tan(fovWHalf * Mathf.Deg2Rad);

        Vector3 topLeft = (mainCamera.transform.forward - toRight + toTop);
        float camScale = topLeft.magnitude * camFar;

        topLeft.Normalize();
        topLeft *= camScale;

        Vector3 topRight = (mainCamera.transform.forward + toRight + toTop);
        topRight.Normalize();
        topRight *= camScale;

        Vector3 bottomRight = (mainCamera.transform.forward + toRight - toTop);
        bottomRight.Normalize();
        bottomRight *= camScale;

        Vector3 bottomLeft = (mainCamera.transform.forward - toRight - toTop);
        bottomLeft.Normalize();
        bottomLeft *= camScale;

        frustumCorners.SetRow(0, bottomLeft);
        frustumCorners.SetRow(1, bottomRight);
        frustumCorners.SetRow(2, topRight);
        frustumCorners.SetRow(3, topLeft);

        fogMaterial.SetMatrix("_FrustumCornersWS", frustumCorners);
        fogMaterial.SetVector("_CameraWS", mainCamera.transform.position);
        fogMaterial.SetColor("_FogColor", fogColor);
        fogMaterial.SetFloat("_FogDensity", fogDensity);
        fogMaterial.SetFloat("_FogStartHeight", fogStartHeight);
        fogMaterial.SetFloat("_FogHeightFalloff", fogHeightFalloff);

        fogMaterial.SetFloat("_Linear01Depth", 1.0f / (camFar - camNear));
        fogMaterial.SetFloat("_LinearEyeDepth", 1.0f / (1.0f / camFar - 1.0f / camNear));

        Graphics.Blit(source, destination, fogMaterial);
    }

    void OnDisable()
    {
        if (fogMaterial != null)
        {
            DestroyImmediate(fogMaterial);
        }
    }
}
