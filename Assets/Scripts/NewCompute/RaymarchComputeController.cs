using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class RaymarchComputeController : MonoBehaviour
{
    [SerializeField] ComputeShader raymarchCompute;
    [SerializeField] Shader blitShader;
    [SerializeField] float maxDistance = 20f;

    public  List<Vector4> spheres = new List<Vector4>();

    Camera cam;
    Material blitMat;
    ComputeBuffer sphereBuffer;
    RenderTexture target;

    void Start()
    {
        cam = GetComponent<Camera>();
        blitMat = new Material(blitShader);
        InitRenderTexture();
    }

    void InitRenderTexture()
    {
        if (target != null && target.width == Screen.width && target.height == Screen.height)
            return;

        if (target != null) target.Release();

        target = new RenderTexture(Screen.width, Screen.height, 0,
            RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
        target.enableRandomWrite = true;
        target.Create();
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        InitRenderTexture();
        UpdateSphereBuffer();

        int kernel = raymarchCompute.FindKernel("CSMain");
        raymarchCompute.SetTexture(kernel, "_Result", target);
        raymarchCompute.SetBuffer(kernel, "_Spheres", sphereBuffer);
        raymarchCompute.SetInt("_SphereCount", spheres.Count);
        raymarchCompute.SetMatrix("_CamFrustum", CamFrustum(cam));
        raymarchCompute.SetMatrix("_CamToWorld", cam.cameraToWorldMatrix);
        raymarchCompute.SetVector("_CameraPos", cam.transform.position);
        raymarchCompute.SetFloat("_MaxDistance", maxDistance);

        int threadGroupsX = Mathf.CeilToInt(Screen.width / 8.0f);
        int threadGroupsY = Mathf.CeilToInt(Screen.height / 8.0f);

        raymarchCompute.Dispatch(kernel, threadGroupsX, threadGroupsY, 1);

        // Draw the result
        blitMat.SetTexture("_MainTex", target);
        Graphics.Blit(null, destination, blitMat);
    }

    void UpdateSphereBuffer()
    {
        if (spheres.Count == 0) return;

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
    }

    Matrix4x4 CamFrustum(Camera cam)
    {
        Matrix4x4 frustum = Matrix4x4.identity;
        float fov = Mathf.Tan((cam.fieldOfView * 0.5f) * Mathf.Deg2Rad);

        Vector3 up = Vector3.up * fov;
        Vector3 right = Vector3.right * fov * cam.aspect;

        Vector3 TL = -Vector3.forward - right + up;
        Vector3 TR = -Vector3.forward + right + up;
        Vector3 BR = -Vector3.forward + right - up;
        Vector3 BL = -Vector3.forward - right - up;

        frustum.SetRow(0, TL);
        frustum.SetRow(1, TR);
        frustum.SetRow(2, BR);
        frustum.SetRow(3, BL);
        return frustum;
    }
}
