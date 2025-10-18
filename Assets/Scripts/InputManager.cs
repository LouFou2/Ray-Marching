using UnityEngine;

public class InputManager : MonoBehaviour
{
    //This will use the mouse input to move a slime ball
    // It depends on the orientation of the camera how the xy movement should be translated in 3d space
    [SerializeField] private Camera cam;
    [SerializeField] private Transform controlledSphereTransform;
    [SerializeField] private float dampFollowSpeed = 1f;
    Vector3 mousePosition = Vector3.zero;
    Vector3 prevPosition = Vector3.zero; //previous position of object

    //***These two properties could be useful for shader stuff
    public float moveSpeed = 0f;
    public Vector3 moveDirection = Vector3.zero;

    void Start()
    {
        mousePosition = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, cam.nearClipPlane + 3));
        prevPosition = new Vector3(mousePosition.x, mousePosition.y, controlledSphereTransform.position.z);
    }
    void Update()
    {
        controlledSphereTransform.rotation = cam.transform.rotation;

        //get mouse movement
        mousePosition = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, cam.nearClipPlane + 3));

        Vector3 targetPosition = new Vector3(mousePosition.x, mousePosition.y, controlledSphereTransform.position.z);

        moveDirection = (targetPosition - prevPosition).normalized; //***might want to use this in shader later?

        //I would like to actually move the sphere along the objects local axis, so the "camera orientation" changes the direction
        controlledSphereTransform.position = Vector3.Lerp(controlledSphereTransform.position, targetPosition, dampFollowSpeed);

        moveSpeed = Vector3.Distance(controlledSphereTransform.position, prevPosition); //***might also want this for shader

    }
}
