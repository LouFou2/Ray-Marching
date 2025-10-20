using UnityEngine;

public class ElasticFollower : MonoBehaviour
{
    [SerializeField] Transform controlTransform;
    [SerializeField] Transform followerTransform;
    [SerializeField] float dampFollowSpeed = 0.5f;
    [SerializeField] float dampBounceFactor = 0.5f;

    Vector3 stretchedPointPos;
    Vector3 targetPos;

    //for the elastic we need to store a maximum distance, like the maximum "stretch" point
    float maxStretchDist = 0f;
    float dampedDistance = 0f;

    float bounceTime;

    void Update()
    {
        Vector3 currentFollowPos = followerTransform.position;
        Vector3 currentControlPos = controlTransform.position;
        float followerDist = Vector3.Distance(currentFollowPos, currentControlPos);

        //just want to clamp the follow value to maybe the sum of both spheres' radii
        float sphere1R = controlTransform.localScale.x;
        float clampedDistance = (sphere1R *2) * 1.5f;
        if (followerDist > clampedDistance) followerDist = clampedDistance;

        //calculate the stretch
        if (followerDist > maxStretchDist)
        {
            maxStretchDist = followerDist;
            dampedDistance = maxStretchDist;
            stretchedPointPos = currentFollowPos; //store the position where it is at the max stretch
            //bounceTime = 0; //reset it until the bounce actually starts
        }
        dampedDistance *= dampBounceFactor;

        Vector3 storedStretchDirection = (currentControlPos - stretchedPointPos).normalized; //*TODO this stretched point position should be dynamically updated
        
        // make an "overshoot" target that is in the direction of the controller, but past it
        targetPos = currentControlPos + (storedStretchDirection * maxStretchDist);
        Vector3 direction = (targetPos - currentFollowPos).normalized;

        //what can I do with a sine wave?
        bounceTime += Time.deltaTime * 20;
        float sinWaveValue = Mathf.Sin(bounceTime); // -1 to 1 range
        float scaledSinValue = sinWaveValue * dampedDistance; //this will go to the negative direction if sin value is negative
        Vector3 sinTarget = currentControlPos + (direction * scaledSinValue);

        followerTransform.position = Vector3.Slerp(followerTransform.position, sinTarget, dampFollowSpeed);
        //i can also scale the follwer sphere depending on how far it's dragging
        float distanceFactor = 1-(dampedDistance/clampedDistance);
        float ogScale = controlTransform.localScale.x;
        ogScale *= distanceFactor;
        followerTransform.localScale = new Vector3(ogScale, ogScale, ogScale);

        maxStretchDist = dampedDistance; // this means that any new "stretch" will reset the above calculations for stretch distance

    }
}
