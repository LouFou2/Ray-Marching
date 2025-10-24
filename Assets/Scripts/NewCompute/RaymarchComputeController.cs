using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class RaymarchComputeController : MonoBehaviour
{
    [SerializeField] ComputeShader raymarchCompute;
    [SerializeField] Shader blitShader;
    [SerializeField] float maxDistance = 20f;

    public List<Vector4> spheres = new List<Vector4>();

    Camera cam;
    Material blitMat;
    ComputeBuffer sphereBuffer;
    RenderTexture target;

    void Awake()
    {
        cam = GetComponent<Camera>();
        if (blitShader) blitMat = new Material(blitShader);
    }

    void InitRenderTexture()
    {
        int w = Screen.width;
        int h = Screen.height;

        if (target != null && target.width == w && target.height == h) return;

        if (target != null) target.Release();

        target = new RenderTexture(w, h, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear)
        {
            enableRandomWrite = true
        };
        target.Create();
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (raymarchCompute == null)
        {
            Graphics.Blit(source, destination);
            return;
        }

        InitRenderTexture();
        UpdateSphereBuffer();

        int kernel = raymarchCompute.FindKernel("CSMain");

        // Set resources
        raymarchCompute.SetTexture(kernel, "_Result", target);
        raymarchCompute.SetBuffer(kernel, "_Spheres", sphereBuffer);
        raymarchCompute.SetInt("_SphereCount", spheres.Count);

        // Pass matrices (CamFrustum rows must match the order used in compute shader)
        raymarchCompute.SetMatrix("_CamFrustum", CamFrustum(cam));
        raymarchCompute.SetMatrix("_CamToWorld", cam.cameraToWorldMatrix);
        raymarchCompute.SetVector("_CameraPos", cam.transform.position);
        raymarchCompute.SetFloat("_MaxDistance", maxDistance);

        // Also pass resolution explicitly
        raymarchCompute.SetVector("_Resolution", new Vector4(target.width, target.height, 0, 0));

        int threadGroupsX = Mathf.CeilToInt(target.width / 8.0f);
        int threadGroupsY = Mathf.CeilToInt(target.height / 8.0f);

        raymarchCompute.Dispatch(kernel, threadGroupsX, threadGroupsY, 1);

        // Blit result to screen
        if (blitMat != null)
        {
            blitMat.SetTexture("_MainTex", target);
            Graphics.Blit(target, destination, blitMat);
        }
        else
        {
            Graphics.Blit(target, destination);
        }
    }

    void UpdateSphereBuffer()
    {
        if (spheres == null || spheres.Count == 0)
        {
            if (sphereBuffer != null) { sphereBuffer.Release(); sphereBuffer = null; }
            return;
        }

        if (sphereBuffer == null || sphereBuffer.count != spheres.Count)
        {
            if (sphereBuffer != null) sphereBuffer.Release();
            sphereBuffer = new ComputeBuffer(spheres.Count, sizeof(float) * 4);
        }

        sphereBuffer.SetData(spheres);
    }

    void OnDisable()
    {
        if (sphereBuffer != null) sphereBuffer.Release();
        if (target != null) target.Release();
    }

    Matrix4x4 CamFrustum(Camera cam)
    {
        Matrix4x4 frustum = Matrix4x4.identity;

        float fov = Mathf.Tan((cam.fieldOfView * 0.5f) * Mathf.Deg2Rad);

        Vector3 up = Vector3.up * fov;
        Vector3 right = Vector3.right * fov * cam.aspect;

        Vector3 TL = (-Vector3.forward - right + up); // top left corner
        Vector3 TR = (-Vector3.forward + right + up);
        Vector3 BR = (-Vector3.forward + right - up);
        Vector3 BL = (-Vector3.forward - right - up);

        frustum.SetRow(0, new Vector4(TL.x, TL.y, TL.z, 0));
        frustum.SetRow(1, new Vector4(TR.x, TR.y, TR.z, 0));
        frustum.SetRow(2, new Vector4(BR.x, BR.y, BR.z, 0));
        frustum.SetRow(3, new Vector4(BL.x, BL.y, BL.z, 0));

        return frustum;
    }
}
