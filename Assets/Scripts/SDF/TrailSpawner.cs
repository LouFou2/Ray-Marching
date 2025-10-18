using UnityEngine;

public class TrailSpawner : MonoBehaviour
{
    [SerializeField] private Transform controlBlob;

    [SerializeField] private float maxFollowDistance = 1f;

    public float blobDistance;

    float blobScale = 0f;

    void Start()
    {
        blobScale = controlBlob.localScale.x;
    }

    void Update()
    {
        //i want to spawn spheres when the follow point gets far enough away
    }
}
