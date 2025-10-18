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

    public float moveSpeed = 0f;

    void Start()
    {
        mousePosition = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, (cam.farClipPlane - cam.nearClipPlane) / 2));
        prevPosition = new Vector3(mousePosition.x, mousePosition.y, controlledSphereTransform.position.z);
    }
    void Update()
    {
        controlledSphereTransform.rotation = cam.transform.rotation;

        //get mouse movement
        mousePosition = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, (cam.farClipPlane - cam.nearClipPlane) / 2));

        Vector3 targetPosition = new Vector3(mousePosition.x, mousePosition.y, controlledSphereTransform.position.z);
        controlledSphereTransform.position = Vector3.Lerp(controlledSphereTransform.position, targetPosition, dampFollowSpeed);

        moveSpeed = Vector3.Distance(controlledSphereTransform.position, prevPosition);
        Debug.Log(moveSpeed);

    }
}
