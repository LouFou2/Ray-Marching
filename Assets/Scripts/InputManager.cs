using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private PlayerInput playerInput;
    //This will use the mouse input to move a slime ball
    // It depends on the orientation of the camera how the xy movement should be translated in 3d space
    [SerializeField] private Camera cam;
    [SerializeField] private Transform controllerTransform;
    Vector3 objectTargetPos = Vector3.zero;

    [SerializeField] float mouseSensitivity = 100f;
    [SerializeField] float cursorDepth = 5f;
    [SerializeField] float moveSpeed = 12f;
    [SerializeField] float rotationSmoothTime = 0.05f;
    [SerializeField] float moveSmoothTime = 0.1f;
    [SerializeField] float controlSmoothTime = 0.05f;

    Vector3 moveVelocity;
    Vector2 rotationSmoothVelocity;
    Vector2 currentRotation;
    Vector2 targetRotation;

    bool controlSuspend;

    private InputAction touchPress;
    private InputAction touchPosition;

    Vector3 screenPosition;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        touchPress = playerInput.actions["TouchPress"];
        touchPosition = playerInput.actions["Move"];
    }
    /*
    private void OnEnable()
    {
        touchPress.performed += TouchPressed;
    }
    private void OnDisable()
    {
        touchPress.performed -= TouchPressed;
    }
    
    private void TouchPressed(InputAction.CallbackContext context)
    {
        float value = context.ReadValue<float>();
        screenPosition = touchPosition.ReadValue<Vector2>();
        Vector3 addDepth = new Vector3(screenPosition.x, screenPosition.y, cursorDepth);
        
        Vector3 touchCamPos = cam.ScreenToWorldPoint(addDepth);

        Debug.Log(touchCamPos);

        objectTargetPos = touchCamPos;
    }*/

    private void Start()
    {
        /*
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        */
    }

    void Update()
    {
        //suspend control with "e"
        if (Input.GetKey(KeyCode.E)) controlSuspend = !controlSuspend;

        /*
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        //Debug.Log("mouse x: " + mouseX + "mouse y: " + mouseY);

        // rotating the camera with the mouse
        targetRotation.x -= mouseY;
        targetRotation.y += mouseX;
        

        // rotating the camera with the mouse
        targetRotation.x -= mouseY;
        targetRotation.y += mouseX;

        // Smoothly interpolate rotation
        currentRotation.x = Mathf.SmoothDamp(currentRotation.x, targetRotation.x, ref rotationSmoothVelocity.x, rotationSmoothTime);
        currentRotation.y = Mathf.SmoothDamp(currentRotation.y, targetRotation.y, ref rotationSmoothVelocity.y, rotationSmoothTime);

        cam.transform.localRotation = Quaternion.Euler(currentRotation.x, currentRotation.y, 0f);

        //moving camera with wasd
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        float moveY = Input.GetKey(KeyCode.Space) ? 1 : Input.GetKey(KeyCode.LeftShift) ? -1 : 0;

        Vector3 moveInput = cam.transform.right * moveX + cam.transform.up * moveY + cam.transform.forward * moveZ;
        Vector3 targetMove = moveInput * moveSpeed * Time.deltaTime;
        cam.transform.position += targetMove;
        */
        //use mouse movement for setting the control object's target position
        //objectTargetPos =  cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, cam.nearClipPlane + cursorDepth));
        // Get the current finger position while pressed
        if (touchPress.IsPressed())
        {
            Vector2 screenPos = touchPosition.ReadValue<Vector2>();

            // Convert to world space
            Vector3 worldPos = cam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, cursorDepth));

            objectTargetPos = worldPos;
        }

        if (!controlSuspend) controllerTransform.position = objectTargetPos;

        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }

        //cam.transform.LookAt(controllerTransform);
    }

    public Vector3 GetControlPointPos()
    {
        return controllerTransform.position;
    }
}
