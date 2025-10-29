using UnityEngine;

public class InputManager : MonoBehaviour
{
    //This will use the mouse input to move a slime ball
    // It depends on the orientation of the camera how the xy movement should be translated in 3d space
    [SerializeField] private Camera cam;
    [SerializeField] private Transform controllerTransform;
    Vector3 objectTargetPos = Vector3.zero;

    [SerializeField] float mouseSensitivity = 100f;
    [SerializeField] float cursorDepth = 5f;
    [SerializeField] float moveSpeed = 12f;

    float xRotation;
    float yRotation;

    bool controlSuspend;
    
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        //suspend control with "e"
        if (Input.GetKey(KeyCode.E)) controlSuspend = !controlSuspend;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        //Debug.Log("mouse x: " + mouseX + "mouse y: " + mouseY);

        // rotating the camera with the mouse
        xRotation -= mouseY;
        yRotation += mouseX;

        cam.transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);

        //moving camera with wasd
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        float moveY = Input.GetKey(KeyCode.Space) ? 1 : Input.GetKey(KeyCode.LeftShift) ? -1 : 0;

        Vector3 move = cam.transform.right * moveX + cam.transform.up * moveY + cam.transform.forward * moveZ;
        cam.transform.position += move * moveSpeed * Time.deltaTime;

        //use mouse movement for setting the control object's target position
        objectTargetPos = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, cam.nearClipPlane + cursorDepth));

        if(!controlSuspend) controllerTransform.position = objectTargetPos;

        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }

    }

    public Vector3 GetControlPointPos()
    {
        return controllerTransform.position;
    }
}
