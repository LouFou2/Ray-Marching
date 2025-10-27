using System.Collections.Generic;
using UnityEngine;

public class Tracer3 : MonoBehaviour
{
    [SerializeField]
    RaymarchComputeController raymarchCamScript;
    [SerializeField] Transform controlTipTransform;
    [SerializeField] float distanceBetweenSpawns = 1f;
    //[SerializeField] float sphereMoveSpeed = 3f; //*** what was this for again?
    [SerializeField] float inflateSpeed = 10f;
    [SerializeField] int maxSphereCount = 2;

    float originalScale;
    Vector3 previousPos;

    [SerializeField] List<Vector4> _spheresData = new List<Vector4>();
    Vector4 controlSphereData;


    private void Start()
    {

        previousPos = controlTipTransform.position;

        _spheresData = raymarchCamScript.spheres;

        controlSphereData = _spheresData[0];
        originalScale = controlSphereData.w;
        distanceBetweenSpawns = originalScale * 1.5f;

        PassDataToCam(_spheresData);
    }

    void Update()
    {
        _spheresData[0] = new Vector4(
                controlTipTransform.position.x,
                controlTipTransform.position.y,
                controlTipTransform.position.z,
                originalScale);

        // --- NEW: send only the first sphere to GPU ---
        raymarchCamScript.UpdateFirstSphere(_spheresData[0]);

        int lastIndex = _spheresData.Count - 1;

        //at the moment i am limiting the total amount of spheres (128), but only because frame rate drops a lot... I would like to optimise this issue away though
        if (Vector3.Distance(previousPos, controlTipTransform.position) > distanceBetweenSpawns && _spheresData.Count <= maxSphereCount-1 && Input.GetMouseButton(0))
        {
            // we add a new Vector4 to the _spheresData list
            previousPos = controlTipTransform.position;

            Vector4 newData = new Vector4(
                controlTipTransform.position.x,
                controlTipTransform.position.y,
                controlTipTransform.position.z,
                originalScale);

            _spheresData.Add(newData);

            PassDataToCam(_spheresData);
        }
        //inflating the sphere
        if (Vector3.Distance(previousPos, controlTipTransform.position) < distanceBetweenSpawns && _spheresData.Count > 1 && Input.GetMouseButton(0))
        {
            _spheresData[lastIndex] += new Vector4(0,0,0, Time.deltaTime * inflateSpeed);
            distanceBetweenSpawns = _spheresData[lastIndex].w;
            PassDataToCam(_spheresData);
        }

        if (Input.GetMouseButton(1))
        {
            _spheresData.Clear();
            _spheresData.Add(new Vector4(
                controlTipTransform.position.x,
                controlTipTransform.position.y,
                controlTipTransform.position.z,
                originalScale));
            PassDataToCam(_spheresData);
        }
    }
    void PassDataToCam(List<Vector4> sphereData)
    {
        raymarchCamScript.spheres = sphereData;
        raymarchCamScript.InitSphereBuffer();
    }
}
