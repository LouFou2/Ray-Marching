using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class RaymarchComputeController : MonoBehaviour
{
    [SerializeField] ComputeShader raymarchCompute;
    [SerializeField] Shader blitShader;
    [SerializeField] float maxRenderDistance = 20f;
    public Cubemap skyBox;
    
    List<PointData> pointsData = new List<PointData>();
    [SerializeField] int pointCount = 1;

    ComputeBuffer pointsA;
    ComputeBuffer pointsB;

    Camera cam;
    Material blitMat;
    RenderTexture target;

    [SerializeField] float time = 0;

    int mainKernel;
    int updatePointsKernel;

    [System.Serializable]
    struct PointData
    {
        public Vector4 pos_radius; // x,y,z,r
        public Vector4 vel_pad;    // x,y,z,pad
    }

    void Awake()
    {
        cam = GetComponent<Camera>();
        if (blitShader) blitMat = new Material(blitShader);
        
        mainKernel = raymarchCompute.FindKernel("CSMain");
        updatePointsKernel = raymarchCompute.FindKernel("UpdatePoints");

        //raymarchCompute.SetTexture(mainKernel, "_SkyBoxCubeMap", skyBox);
        
        InitPointsBuffer(); // initialize spheres once at start
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
        time = Time.deltaTime;

        if (raymarchCompute == null)
        {
            Graphics.Blit(source, destination);
            return;
        }

        InitRenderTexture();
        //UpdateSphereBufferIfCountChanged();

        raymarchCompute.SetFloat("time", time);

        // *** Disabling point updates for now
        // The update kernel is kept ready, but not actually used yet...
        // raymarchCompute.Dispatch(updatePointsKernel, Mathf.Max(1, pointCount / 64), 1, 1);
        // older one: raymarchCompute.Dispatch(updatePointsKernel, pointCount, 1, 1);

        // GPU Paramaters
        raymarchCompute.SetTexture(mainKernel, "_Result", target);
        raymarchCompute.SetTexture(mainKernel, "_SkyBoxCubeMap", skyBox);
        raymarchCompute.SetInt("_PointCount", pointCount);
        
        // Camera Stuff
        raymarchCompute.SetMatrix("_CamFrustum", CamFrustum(cam));
        raymarchCompute.SetMatrix("_CamToWorld", cam.cameraToWorldMatrix);
        raymarchCompute.SetVector("_CameraPos", cam.transform.position);
        raymarchCompute.SetFloat("_MaxDistance", maxRenderDistance);
        raymarchCompute.SetVector("_Resolution", new Vector4(target.width, target.height, 0, 0));

        // === Buffers ===
        raymarchCompute.SetBuffer(mainKernel, "_PointsRead", pointsA);

        int threadGroupsX = Mathf.CeilToInt(target.width / 8.0f);
        int threadGroupsY = Mathf.CeilToInt(target.height / 8.0f);

        raymarchCompute.Dispatch(mainKernel, threadGroupsX, threadGroupsY, 1);

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

    public void InitPointsBuffer()
    {
        if (pointsA != null) pointsA.Release();
        if (pointsB != null) pointsB.Release();

        pointsA = new ComputeBuffer(pointCount, sizeof(float) * 8);
        pointsB = new ComputeBuffer(pointCount, sizeof(float) * 8);

        // init particles
        PointData[] points = new PointData[pointCount];
        for (int i = 0; i < pointCount; i++)
        {
            points[i].pos_radius = new Vector4(0, 0, 10, 0.5f);
            points[i].vel_pad = Vector4.zero;
        }

        pointsA.SetData(points);
        pointsB.SetData(points);

        raymarchCompute.SetBuffer(mainKernel, "_PointsRead", pointsA);
        raymarchCompute.SetBuffer(updatePointsKernel, "_PointsRead", pointsA);
        raymarchCompute.SetBuffer(updatePointsKernel, "_PointsWrite", pointsB);
        raymarchCompute.SetInt("_PointCount", pointCount);
    }
    /*
    void UpdateSphereBufferIfCountChanged()
    {
        if (pointsA == null || pointsA.count != pointsData.Count)
            InitPointsBuffer();
    }
    public void UpdateFinalSphere(Vector4 finalSphereData)
    {
        if (pointsA == null) return;
        // Write only one element (final index) into the GPU buffer
        int lastIndex = pointsData.Count - 1;
        pointsA.SetData(new Vector4[] { finalSphereData }, 0, lastIndex, 1);
    }
    public void UpdateSegmentLength(float segmentLength) // distance between points
    {
        raymarchCompute.SetFloat("segmentLength", segmentLength);
    }
    */
    void OnDisable()
    {
        if (pointsA != null) pointsA.Release();
        if (pointsB != null) pointsB.Release();
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
