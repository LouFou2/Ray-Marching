using System.Collections.Generic;
using UnityEngine;

public class Tracer : MonoBehaviour
{
    [SerializeField] Transform controlTipTransform;
    [SerializeField] GameObject spherePrefab;
    [SerializeField] float distanceBetweenSpawns = 1f;
    [SerializeField] float sphereMoveSpeed = 3f;
    [SerializeField] float scaleNoiseFactor = 0.2f;

    Vector3 originalScale;
    Vector3 previousPos;

    List<GameObject> spawnedSpheres = new List<GameObject>();
    List<Vector3> spawnPositions = new List<Vector3>();

    float sphereMovementTime = 0f;

    GameObject finalSphere;
    bool spawnedFinalSphere;

    private void Start()
    {
        previousPos = controlTipTransform.position;
        originalScale = controlTipTransform.localScale;
        distanceBetweenSpawns = originalScale.x * 1.5f;
    }
    void Update()
    {
        
        sphereMovementTime += Time.deltaTime * sphereMoveSpeed;
        float smooth = sphereMovementTime / distanceBetweenSpawns;

        if (Vector3.Distance(previousPos, controlTipTransform.position) > distanceBetweenSpawns && spawnedSpheres.Count <= 32 && Input.GetMouseButton(0))
        {
            GameObject newSphere = Instantiate(spherePrefab, controlTipTransform.position, Quaternion.identity);
            newSphere.transform.localScale = originalScale;
            spawnedSpheres.Add(newSphere);
            spawnPositions.Add(controlTipTransform.position);
            previousPos = controlTipTransform.position;
        }
        // lets move the spheres along the traced "line" (along the sequence of points)
        for (int s = 0; s < spawnedSpheres.Count - 1; s++)
        {
            spawnedSpheres[s].transform.position = Vector3.Lerp(spawnPositions[s], spawnPositions[s+1], smooth);
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
    }
}
