using System.Collections.Generic;
using UnityEngine;

public class TrailSpawner : MonoBehaviour
{
    [SerializeField] private DoubleBlobBehaviour doubleBlobScript;
    float controlBlobRadius;
    float tailBlobRadius;
    float distance = 0f;
    float totalRadii = 0f;
    float _maxFollowDistance;
    float _tailMinScale;

    [SerializeField] GameObject spherePrefab;
    List<GameObject> spherePrefabs = new List<GameObject>();

    private void Start()
    {
        controlBlobRadius = doubleBlobScript.ogControlRadius;
        tailBlobRadius = doubleBlobScript.ogFollowerRadius;

        spherePrefabs.Add(doubleBlobScript.followerBlob); //we'll add follower blobs from the tail towards the head

        _maxFollowDistance = doubleBlobScript.maxFollowDistance;
        _tailMinScale = doubleBlobScript.tailMinScale;
    }
    void Update()
    {
        //the index of the last added item
        int lastIndex = spherePrefabs.Count - 1;

        distance = doubleBlobScript.blobDistance;
        Vector3 controlPos = doubleBlobScript.controlBlob.transform.position;
        Vector3 headDirection = doubleBlobScript.controllerDirection;

        //add up all the radii of all spheres
        totalRadii = 0;
        totalRadii += controlBlobRadius;
        for (int r = 0; r < spherePrefabs.Count; r++)
        {
            float sphereRadius = spherePrefabs[r].transform.localScale.x;
            totalRadii += sphereRadius * 2; // so actually adding diameters, not radii
        }
        totalRadii -= spherePrefabs[0].transform.localScale.x;//just delete the tail radius, because it added as diameter

        if (distance - totalRadii > totalRadii)
        {
            //the new sphere's position is halfway the distance from last spawned sphere towards the control sphere
            Vector3 newSpherePos = spherePrefabs[lastIndex].transform.position + (distance/(spherePrefabs.Count+1)) * headDirection;
            GameObject newSphere = Instantiate(spherePrefab, newSpherePos, Quaternion.identity);
            
            //scale the new sphere
            float interDistance = Vector3.Distance(newSphere.transform.position, controlPos);
            float invLerped = Mathf.InverseLerp(0, doubleBlobScript.maxFollowDistance, interDistance); // 0-1 result
            float t = Mathf.Pow(invLerped, 3f);
            float targetScaleFactor = Mathf.Lerp(1, doubleBlobScript.tailMinScale, t);
            newSphere.transform.localScale = new Vector3(controlBlobRadius * targetScaleFactor, controlBlobRadius * targetScaleFactor, controlBlobRadius * targetScaleFactor);

            spherePrefabs.Add(newSphere);
        }
        //gotta delete spheres again when the head and tail is close enough...
        for (int s = spherePrefabs.Count-1; s > 0; s--)//its gonna count down the list and stop before 0(the tail index)
        {
            if (totalRadii > distance)
            {
                Destroy(spherePrefabs[s]);
                spherePrefabs.Remove(spherePrefabs[s]);
            }
            
        }

        //place all spheres linearly along the line between the head and the tail
        Vector3 currentPos = spherePrefabs[0].transform.position;
        for (int i = 1; i < spherePrefabs.Count; i++)
        {
            float spacing = spherePrefabs[i - 1].transform.localScale.x + spherePrefabs[i].transform.localScale.x;
            currentPos += headDirection * spacing;
            spherePrefabs[i].transform.position = currentPos;
        

            //we also need to scale them all
            float interDistance = Vector3.Distance(spherePrefabs[i].transform.position, controlPos);
            float invLerped = Mathf.InverseLerp(0, _maxFollowDistance, interDistance); // 0-1 result
            float t = Mathf.Pow(invLerped, 3f);
            float targetScaleFactor = Mathf.Lerp(1, _tailMinScale, t);
            spherePrefabs[i].transform.localScale = new Vector3(controlBlobRadius * targetScaleFactor, controlBlobRadius * targetScaleFactor, controlBlobRadius * targetScaleFactor);
        }
    }
}
