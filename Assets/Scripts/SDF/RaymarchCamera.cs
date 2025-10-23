using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
[ExecuteInEditMode]

public class RaymarchCamera : MonoBehaviour
{
    [SerializeField]
    private Shader _shader;

    public Material _raymarchMaterial
    {
        get
        {
            if (!_raymarchMat && _shader)
            {
                _raymarchMat = new Material(_shader);
                _raymarchMat.hideFlags = HideFlags.HideAndDontSave;
            }
            return _raymarchMat;
        }
    }
    private Material _raymarchMat;

    public Camera _camera
    {
        get
        {
            if (!_cam)
            {
                _cam = GetComponent<Camera>();
            }
            return _cam;
        }
    }
    private Camera _cam;

    public float _maxDistance;

    //public Vector4 _sphere1; //*** ultimately this is what I want to pass in from another script, probably also a list, so that might change the shader script's setup
    //public Vector4 _sphere2;

    public List<Vector4> _spheres = new List<Vector4>();
    ComputeBuffer buffer;

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (!_raymarchMaterial)
        {
            Graphics.Blit(source, destination);
            return;
        }

        //buffer = new ComputeBuffer(_spheres.Count, SphereData.GetSize());
        // Recreate buffer only if needed
        if (buffer == null || buffer.count != _spheres.Count)
        {
            if (buffer != null)
                buffer.Release();

            buffer = new ComputeBuffer(_spheres.Count, SphereData.GetSize());
        }

        SphereData[] sphereDatas = new SphereData[_spheres.Count];
        for (int i = 0; i < _spheres.Count; i++)
        {
            SphereData s = new SphereData()
            {
                sphereInfo = _spheres[i]
            };
            sphereDatas[i] = s;
        }

        buffer.SetData(sphereDatas);

        _raymarchMaterial.SetBuffer("_spheresBuffer", buffer);
        _raymarchMaterial.SetInt("_sphereCount", _spheres.Count);
        _raymarchMaterial.SetMatrix("_CamFrustum", CamFrustum(_camera));
        _raymarchMaterial.SetMatrix("_CamToWorld", _camera.cameraToWorldMatrix);
        _raymarchMaterial.SetFloat("_maxDistance", _maxDistance);

        RenderTexture.active = destination;
        GL.PushMatrix();
        GL.LoadOrtho();
        _raymarchMaterial.SetPass(0);
        GL.Begin(GL.QUADS);

        //BL
        GL.MultiTexCoord2(0, 0.0f, 0.0f);
        GL.Vertex3(0.0f, 0.0f, 3.0f);
        //BR
        GL.MultiTexCoord2(0, 1.0f, 0.0f);
        GL.Vertex3(1.0f, 0.0f, 2.0f);
        //TR
        GL.MultiTexCoord2(0, 1.0f, 1.0f);
        GL.Vertex3(1.0f, 1.0f, 1.0f);
        //TL
        GL.MultiTexCoord2(0, 0.0f, 1.0f);
        GL.Vertex3(0.0f, 1.0f, 0.0f);

        GL.End();
        GL.PopMatrix();

    }

    private Matrix4x4 CamFrustum(Camera cam)
    {
        Matrix4x4 frustum = Matrix4x4.identity;

        float fov = Mathf.Tan((cam.fieldOfView * 0.5f) * Mathf.Deg2Rad);

        Vector3 goUp = Vector3.up * fov;
        Vector3 goRight = Vector3.right * fov * cam.aspect;

        Vector3 TL = (-Vector3.forward - goRight + goUp); // top left corner
        Vector3 TR = (-Vector3.forward + goRight + goUp);
        Vector3 BR = (-Vector3.forward + goRight - goUp);
        Vector3 BL = (-Vector3.forward - goRight - goUp);

        frustum.SetRow(0, TL);
        frustum.SetRow(1, TR);
        frustum.SetRow(2, BR);
        frustum.SetRow(3, BL);

        return frustum;
    }

    private void OnDisable()
    {
        if (buffer != null)
        {
            buffer.Release();
            buffer = null;
        }
    }
}

public struct SphereData
{
    public Vector4 sphereInfo;

    public static int GetSize()
    {
        return sizeof (float) * 4 ;
    }
}