using UnityEngine;

public class DoubleBlobBehaviour : MonoBehaviour
{
    public GameObject controlBlob;
    public GameObject followerBlob;
    public float maxFollowDistance = 1f;
    public float tailMinScale = 0.1f;

    public float blobDistance;
    public Vector3 controllerDirection;

    public float ogControlRadius;
    public float ogFollowerRadius;

    private void Start()
    {
        ogControlRadius = controlBlob.transform.localScale.x;
        ogFollowerRadius = followerBlob.transform.localScale.x;
    }

    void Update()
    {
        float t = 0;
        blobDistance = Vector3.Distance(controlBlob.transform.position, followerBlob.transform.position);
        controllerDirection = (controlBlob.transform.position - followerBlob.transform.position).normalized;

        if (blobDistance > maxFollowDistance)
        {
            maxFollowDistance = blobDistance;
        }
        
        float invLerped = Mathf.InverseLerp(0, maxFollowDistance, blobDistance); // 0-1 result
        t = Mathf.Pow(invLerped, 3f);

        float targetScaleFactor = Mathf.Lerp(1, tailMinScale, t);
        followerBlob.transform.localScale = new Vector3(ogControlRadius * targetScaleFactor, ogControlRadius * targetScaleFactor, ogControlRadius * targetScaleFactor);
    }
}
