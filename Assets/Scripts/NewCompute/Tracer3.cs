using System.Collections.Generic;
using UnityEngine;

public class Tracer3 : MonoBehaviour
{
    [SerializeField]
    RaymarchComputeController raymarchCamScript;
    [SerializeField] Transform controlTipTransform;
    [SerializeField] float distanceBetweenSpawns = 1f;
    [SerializeField] float inflateSpeed = 10f;
    [SerializeField] int maxSphereCount = 2;

    float originalScale;
    Vector3 previousPos;

    [SerializeField] List<Vector4> _spheresData = new List<Vector4>();
    Vector4 controlSphereData;

    int lastIndex = 0;

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
        //int lastIndex = _spheresData.Count - 1;

        _spheresData[lastIndex] = new Vector4(
                controlTipTransform.position.x,
                controlTipTransform.position.y,
                controlTipTransform.position.z,
                originalScale);

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
        /*//inflating the sphere
        if (Vector3.Distance(previousPos, controlTipTransform.position) < distanceBetweenSpawns && _spheresData.Count > 1 && Input.GetMouseButton(0))
        {
            _spheresData[lastIndex] += new Vector4(0,0,0, Time.deltaTime * inflateSpeed);
            distanceBetweenSpawns = _spheresData[lastIndex].w;

            PassDataToCam(_spheresData);
        }*/

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

        // --- send only the last sphere to GPU ---
        lastIndex = _spheresData.Count - 1;
        raymarchCamScript.UpdateFinalSphere(_spheresData[lastIndex]);
    }
    void PassDataToCam(List<Vector4> sphereData)
    {
        raymarchCamScript.spheres = sphereData;
        raymarchCamScript.InitSphereBuffer();
    }
}
