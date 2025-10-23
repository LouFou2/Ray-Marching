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

    float sphereMovementTime = 0f;

    public List<Vector4> _spheresData = new List<Vector4>(1);
    Vector4 controlSphereData;

    private void Start()
    {

        previousPos = controlTipTransform.position;
        originalScale = controlTipTransform.localScale;
        distanceBetweenSpawns = originalScale.x * 1.5f;

        _spheresData = raymarchCamScript._spheres;
    }
    void Update()
    {

        sphereMovementTime += Time.deltaTime * sphereMoveSpeed;
        float smooth = sphereMovementTime / distanceBetweenSpawns;

        _spheresData[0] = new Vector4(
                controlTipTransform.position.x,
                controlTipTransform.position.y,
                controlTipTransform.position.z,
                originalScale.x);

        if (Vector3.Distance(previousPos, controlTipTransform.position) > distanceBetweenSpawns && _spheresData.Count <= 32 && Input.GetMouseButton(0))
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
/*
        // lets move the spheres along the traced "line" (along the sequence of points)
        for (int s = 0; s < _spheresData.Count - 1; s++)
        {
            spawnedSpheres[s].transform.position = Vector3.Lerp(spawnPositions[s], spawnPositions[s + 1], smooth);
            //we can also scale these a little bit randomly
            float noiseSample = Mathf.PerlinNoise(spawnedSpheres[s].transform.position.x, spawnedSpheres[s].transform.position.y);
            float scaleOffset = noiseSample * scaleNoiseFactor;
            spawnedSpheres[s].transform.localScale = new Vector3(scaleOffset, scaleOffset, scaleOffset);
        }
        if (!spawnedFinalSphere && spawnPositions.Count >= 1) // one final sphere on the tip that doesn't move
        {
            finalSphere = Instantiate(spherePrefab, spawnPositions[0], Quaternion.identity);
            finalSphere.transform.localScale = originalScale;
            spawnedFinalSphere = true;
        }

        if (sphereMovementTime >= distanceBetweenSpawns) sphereMovementTime = 0;

        if (Input.GetMouseButton(1))
        {
            foreach (GameObject sphere in spawnedSpheres)
            {
                Destroy(sphere);
            }
            spawnedSpheres.Clear();
            spawnPositions.Clear();
            Destroy(finalSphere);
            spawnedFinalSphere = false;
            sphereMovementTime = 0;
        }
*/
    }
    void PassDataToCam(List<Vector4> sphereData)
    {
        raymarchCamScript._spheres = sphereData;
    }
}
