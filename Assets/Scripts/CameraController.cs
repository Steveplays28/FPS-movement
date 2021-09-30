using UnityEngine;

public class CameraController : MonoBehaviour
{
    public PlayerController player;

    public float sensitivity = 100f;
    public float clampAngle = 90f;

    private void Start()
    {
        HideCursor();
    }

    private void LateUpdate()
    {
        if (Cursor.lockState == CursorLockMode.Locked && !Cursor.visible)
        {
            Look();
        }
    }

    private void Look()
    {
        if (transform.rotation.eulerAngles.x > clampAngle || transform.rotation.eulerAngles.x > -clampAngle)
        {
            transform.rotation *= Quaternion.Euler(-Input.GetAxis("Mouse Y") * sensitivity, 0, 0);
        }

        player.transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * sensitivity, 0);
    }

    public void ToggleCursorMode()
    {
        Cursor.visible = !Cursor.visible;

        if (Cursor.lockState == CursorLockMode.None)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void HideCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void ShowCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}
