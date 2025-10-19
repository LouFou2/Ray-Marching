using System.Collections.Generic;
using UnityEngine;

public class TrailSpawner2 : MonoBehaviour
{
    [SerializeField] private GameObject headObject;
    [SerializeField] private Transform tailTransform;
    [SerializeField] private GameObject spherePrefab;
    [SerializeField] private float maxTailDistance = 9f;

    private List<GameObject> spherePrefabs = new List<GameObject>();

    float headRadius;
    float totalRadii = 0f;
    Vector3 tailDirection;

    void Start()
    {
        headRadius = headObject.transform.localScale.x; //the x-value is used in the raymarch to calculate the radius of the sphere
        totalRadii = headRadius; // the head only uses the radius, as the head is in the center of the "diameter"
        spherePrefabs.Clear();
    }

    void FixedUpdate() // doing it in fixed update because the tail is moved by physics
    {
        Vector3 headPos = headObject.transform.position;
        Vector3 tailPos = tailTransform.position;
        float distance = Vector3.Distance(headPos, tailPos);
        distance = Mathf.Clamp(distance, 0, maxTailDistance);
        tailDirection = (tailPos - headPos).normalized;

        if (distance > totalRadii) // if the distance of the tail is outside the radii of all existing spheres
        {
            //we start spawning more spheres
            GameObject newSphere = Instantiate(spherePrefab, tailPos, Quaternion.identity);
            newSphere.SetActive(false);

            spherePrefabs.Add(newSphere);

        }
        else
        {
            // we need to remove and destroy the last sphere in the list
            int lastIndex = spherePrefabs.Count - 1;
            if (lastIndex > -1) 
            {
                GameObject sphereToRemove = spherePrefabs[lastIndex];
                totalRadii -= sphereToRemove.transform.localScale.x;
                Destroy(sphereToRemove);
                spherePrefabs.Remove(sphereToRemove);
            }
            
        }

        //we finally position and scale all the tailing spheres in the list
        totalRadii = headRadius; // the head only uses the radius, as the head is in the center of the first "diameter"
        for (int i = 0; i < spherePrefabs.Count; i++)
        {
            spherePrefabs[i].transform.position = headPos + (totalRadii * tailDirection);

            //scale the new sphere
            float newSphereDistance = Vector3.Distance(spherePrefabs[i].transform.position, headPos);
            newSphereDistance = Mathf.Clamp(newSphereDistance, 0, maxTailDistance);
            float invLerped = Mathf.InverseLerp(maxTailDistance, 0, newSphereDistance); //gives us a 0-1 value representing wher the tail is
            float factoredScale = headRadius * invLerped;
            float exponent = Mathf.Pow(factoredScale, 3);
            spherePrefabs[i].transform.localScale = new Vector3(exponent, exponent, exponent);

            spherePrefabs[i].SetActive(true);

            totalRadii += exponent;
        }
    }
}
