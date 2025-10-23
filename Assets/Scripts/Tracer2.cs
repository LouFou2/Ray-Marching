using System.Collections.Generic;
using UnityEngine;

public class Tracer2 : MonoBehaviour
{
    [SerializeField]
    RaymarchCamera raymarchCamScript;
    [SerializeField] Transform controlTipTransform;
    [SerializeField] float distanceBetweenSpawns = 1f;
    [SerializeField] float sphereMoveSpeed = 3f;
    [SerializeField] float scaleNoiseFactor = 0.2f;

    Vector3 originalScale;
    Vector3 previousPos;

    public List<Vector4> _spheresData = new List<Vector4>(1);
    Vector4 controlSphereData;

    private void Start()
    {

        previousPos = controlTipTransform.position;
        
        _spheresData = raymarchCamScript._spheres;
        controlSphereData = _spheresData[0];
        originalScale = new Vector3(controlSphereData.w, controlSphereData.w, controlSphereData.w);
        distanceBetweenSpawns = originalScale.x * 1.5f;
    }

    void Update()
    {
        _spheresData[0] = new Vector4(
                controlTipTransform.position.x,
                controlTipTransform.position.y,
                controlTipTransform.position.z,
                originalScale.x);

        //at the moment i am limiting the total amount of spheres (128), but only because frame rate drops a lot... I would like to optimise this issue away though
        if (Vector3.Distance(previousPos, controlTipTransform.position) > distanceBetweenSpawns && _spheresData.Count <= 127 && Input.GetMouseButton(0))
        {
            // we add a new Vector4 to the _spheresData list
            previousPos = controlTipTransform.position;

            Vector4 newData = new Vector4(
                controlTipTransform.position.x,
                controlTipTransform.position.y,
                controlTipTransform.position.z, 
                originalScale.x);

            _spheresData.Add(newData);

            PassDataToCam(_spheresData);
        }

        if (Input.GetMouseButton(1))
        {
            _spheresData.Clear();
            _spheresData.Add(controlSphereData);
            PassDataToCam(_spheresData);
        }
    }
    void PassDataToCam(List<Vector4> sphereData)
    {
        raymarchCamScript._spheres = sphereData;
    }
}
