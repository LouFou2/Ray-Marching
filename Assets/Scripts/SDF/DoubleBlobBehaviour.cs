using UnityEngine;

public class DoubleBlobBehaviour : MonoBehaviour
{
    [SerializeField] private Transform controlBlob;
    [SerializeField] private Transform followerBlob;
    [SerializeField] private float maxFollowDistance = 1f;
    [SerializeField] private float tailMinScale = 0.1f;

    public float blobDistance;

    float ogFollowerScale;

    private void Start()
    {
        ogFollowerScale = followerBlob.localScale.x;
    }

    void Update()
    {
        float t = 0;
        blobDistance = Vector3.Distance(controlBlob.position, followerBlob.position);
        if(blobDistance > maxFollowDistance)
        {
            maxFollowDistance = blobDistance;
        }
        /*if (blobDistance > (controlBlob.localScale.x + followerBlob.localScale.x)) //the x scale value is the radius of each sphere
        {
            float lerped = Mathf.InverseLerp(maxFollowDistance, 0, blobDistance); // 0-1 result
            t = Mathf.Pow(lerped, 3f);
        }
        else
            t = ogFollowerScale;*/
        float lerped = Mathf.InverseLerp(0, maxFollowDistance, blobDistance); // 0-1 result
        t = Mathf.Pow(lerped, 3f);

        float targetScaleFactor = Mathf.Lerp(1, tailMinScale, t);
        followerBlob.localScale = new Vector3(ogFollowerScale * targetScaleFactor, ogFollowerScale * targetScaleFactor, ogFollowerScale * targetScaleFactor); 
    }
}
