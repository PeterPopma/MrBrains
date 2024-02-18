using Cinemachine;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    CinemachineVirtualCamera vcamMainCamera;

    [SerializeField] private float moveByKeySpeed = 4f; 
    [SerializeField] private float lookSpeed = 2f;
    [SerializeField] private float zoomSpeed = 2f;
    Vector3 cameraPosition;

    private float cameraYaw = 0f;
    private float cameraPitch = 0f;
    private float cameraRoll = 0f;
    private float cameraDistance = 3f;
    private bool buttonCameraForward;
    private bool buttonCameraBack;
    private bool buttonCameraLeft;
    private bool buttonCameraRight;
    private bool buttonCameraUp;
    private bool buttonCameraDown;
    private bool buttonCameraLookAround;
    private bool buttonCameraRollLeft;
    private bool buttonCameraRollRight;
    private Vector2 look;
    private float zoom;
    private float timeMovingStarted;

    public Vector3 CameraPosition { get => cameraPosition; set => cameraPosition = value; }

    public void Awake()
    {
        vcamMainCamera = GetComponent<CinemachineVirtualCamera>();
    }

    public void Start()
    {
        cameraPosition = Camera.main.transform.position;
    }

    private void OnLook(InputValue value)
    {
        look = value.Get<Vector2>();
    }

    private void OnZoom(InputValue value)
    {
        zoom = value.Get<float>();
    }

    private void OnCameraForward(InputValue value)
    {
        timeMovingStarted = Time.time;
        buttonCameraForward = value.isPressed;
//        CameraSetMoveMode();
    }

    private void OnCameraBack(InputValue value)
    {
        timeMovingStarted = Time.time;
        buttonCameraBack = value.isPressed;
    }

    private void OnCameraLeft(InputValue value)
    {
        timeMovingStarted = Time.time;
        buttonCameraLeft = value.isPressed;
    }

    private void OnCameraRight(InputValue value)
    {
        timeMovingStarted = Time.time;
        buttonCameraRight = value.isPressed;
    }

    private void OnCameraUp(InputValue value)
    {
        timeMovingStarted = Time.time;
        buttonCameraUp = value.isPressed;
    }

    private void OnCameraDown(InputValue value)
    {
        timeMovingStarted = Time.time;
        buttonCameraDown = value.isPressed;
    }

    private void OnCameraLookAround(InputValue value)
    {
        buttonCameraLookAround = value.isPressed;
    }

    private void OnCameraRollLeft(InputValue value)
    {
        buttonCameraRollLeft = value.isPressed;
    }

    private void OnCameraRollRight(InputValue value)
    {
        buttonCameraRollRight = value.isPressed;
    }

    private void OnCameraReset()
    {
        cameraPitch = 0;
        cameraYaw = 0;
        cameraRoll = 0;
    }

    public void LateUpdate()
    {
        vcamMainCamera.transform.eulerAngles = new Vector3(cameraPitch, cameraYaw, cameraRoll);
        vcamMainCamera.transform.position = cameraPosition;
    }

    private void Update()
    {
        float moveSpeed = moveByKeySpeed * Mathf.Pow(2, (Time.time - timeMovingStarted));

        if (buttonCameraRollLeft)
        {
            cameraRoll -= Time.deltaTime * 80;
        }
        if (buttonCameraRollRight)
        {
            cameraRoll += Time.deltaTime * 80;
        }
        if (buttonCameraForward)
        {
            cameraPosition += transform.forward * Time.deltaTime * moveSpeed;
        }
        if (buttonCameraBack)
        {
            cameraPosition -= transform.forward * Time.deltaTime * moveSpeed;
        }
        if (buttonCameraRight)
        {
            cameraPosition += transform.right * Time.deltaTime * moveSpeed;
        }
        if (buttonCameraLeft)
        {
            cameraPosition -= transform.right * Time.deltaTime * moveSpeed;
        }
        if (buttonCameraUp)
        {
            cameraPosition += transform.up * Time.deltaTime * moveSpeed;
        }
        if (buttonCameraDown)
        {
            cameraPosition -= transform.up * Time.deltaTime * moveSpeed;
        }

        // Look around when right mouse is pressed
        if (buttonCameraLookAround)
        {
            cameraYaw += lookSpeed * look.x;
            cameraPitch -= lookSpeed * look.y;

            //            Game.Instance.CameraRotationX += lookSpeed * look.y;
            //            Game.Instance.CameraRotationY -= lookSpeed * look.x;
        }

        // Zoom in and out with Mouse Wheel
        // TODO : should change camera field of view
   //     transform.Translate(0, 0, zoom * zoomSpeed, Space.Self);
    }
}