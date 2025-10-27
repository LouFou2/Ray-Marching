using System.Collections.Generic;
using UnityEngine;

public class ElasticFollower2 : MonoBehaviour
{
    [SerializeField]
    RaymarchComputeController raymarchCamScript;
    [SerializeField] Transform controlTipTransform;
    [SerializeField] float distanceBetweenPoints = 1f;

    Vector3 previousPos;

    [SerializeField] List<Vector4> _spheresData = new List<Vector4>();
    Vector4 controlSphereData; // this is only to store the original values so it can be recreated if things reset

    private void Start()
    {

        previousPos = controlTipTransform.position;

        _spheresData = raymarchCamScript.spheres;
        controlSphereData = _spheresData[0];
    }

    void Update()
    {
        if (_spheresData[0] != new Vector4(
                controlTipTransform.position.x,
                controlTipTransform.position.y,
                controlTipTransform.position.z,
                _spheresData[0].w))
        {
            UpdateControlPoint();
        }
    }

    void UpdateControlPoint()
    {
        _spheresData[0] = new Vector4(
                controlTipTransform.position.x,
                controlTipTransform.position.y,
                controlTipTransform.position.z,
                _spheresData[0].w);

        //int lastIndex = _spheresData.Count - 1; // don't really need this until I start working on a "chain"

        raymarchCamScript.spheres = _spheresData;
    }

    /*void PassDataToCam(List<Vector4> sphereData)
    {
        raymarchCamScript.spheres = sphereData;
    }*/
}
