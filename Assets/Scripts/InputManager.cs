using UnityEngine;

public class InputManager : MonoBehaviour
{
    //This will use the mouse input to move a slime ball
    // It depends on the orientation of the camera how the xy movement should be translated in 3d space
    [SerializeField] private Camera cam;
    [SerializeField] private Transform controllerTransform;
    [SerializeField] private float dampFollowSpeed = 1f;
    Vector3 mousePosition = Vector3.zero;
    Vector3 prevPosition = Vector3.zero; //previous position of object

    [SerializeField] float cursorDepth = 5f;

    void Start()
    {
        mousePosition = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, cam.nearClipPlane + cursorDepth));
        prevPosition = new Vector3(mousePosition.x, mousePosition.y, controllerTransform.position.z);
    }
    void Update()
    {
        controllerTransform.rotation = cam.transform.rotation;

        //get mouse movement
        mousePosition = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, cam.nearClipPlane + cursorDepth));

        Vector3 targetPosition = new Vector3(mousePosition.x, mousePosition.y, controllerTransform.position.z);

        //I would like to actually move the sphere along the objects local axis, so the "camera orientation" changes the direction
        controllerTransform.position = targetPosition;

        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }


    }
}
