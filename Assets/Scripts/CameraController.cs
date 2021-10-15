using UnityEngine;

public class CameraController : MonoBehaviour
{
	public PlayerController player;

	public float sensX = 2f;
	public float sensY = 2f;
	public float clampAngle = 90f;

	private float rotationX = 0f;
	private float rotationY = 0f;

	private void Start()
	{
		HideCursor();
	}

	private void FixedUpdate()
	{
		Look();

		// if (Cursor.lockState == CursorLockMode.Locked && !Cursor.visible)
		// {
		// 	Look();
		// }
	}

	public void Look()
	{
		rotationX -= Input.GetAxis("Mouse Y") * sensX * Time.smoothDeltaTime;
		rotationY += Input.GetAxis("Mouse X") * sensY * Time.smoothDeltaTime;

		rotationX = Mathf.Clamp(rotationX, -clampAngle, clampAngle);

		transform.eulerAngles = new Vector3(rotationX, rotationY, transform.eulerAngles.z);
		player.transform.eulerAngles = new Vector3(player.transform.eulerAngles.x, rotationY, player.transform.eulerAngles.z);
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
