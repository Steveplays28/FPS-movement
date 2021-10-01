using System.Collections;
using DG.Tweening;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	#region Variables
	[Header("Player")]
	public new Camera camera;
	public Rigidbody rb;

	private Vector3 localVelocity;

	[Header("Movement")]
	public bool[] inputs;

	[Space]
	public int acceleration = 500;
	public int counterAcceleration = 500;
	public float walkMaxSpeed = 10;

	[Space]
	public bool canCrouch = true;
	public bool isCrouching;
	public float crouchMaxSpeed;
	public int crouchAcceleration;
	public int crouchCounterAcceleration;

	[Space]
	public float runMaxSpeed;
	public bool isRunning;
	public int runAcceleration;
	public int runCounterAcceleration;

	[Space]
	public float maxSpeed;
	public bool canMoveFAndB = true;
	public bool canMoveLAndR = true;

	[Header("Wallrunning")]
	public float cameraRotateSpeed = 0.5f;
	public float cameraRotationAmount = 25f;
	public float wallrunTimeout = 2f;
	public bool canWallrun = true;
	public bool isWallrunning;

	private Vector3 wallrunDirection;
	private Vector3 wallrunNormal;

	[Header("Jumping")]
	public int jumpHeight;
	public int maxJumps;
	public int jumpsLeft;
	public bool canJump = true;

	private bool isGrounded = true;
	#endregion

	private void Start()
	{
		maxSpeed = walkMaxSpeed;
		Application.targetFrameRate = 30;
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			Jump();
		}
		if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.LeftShift))
		{
			Run();
		}
	}

	private void FixedUpdate()
	{
		inputs = new bool[]
		{
			Input.GetKey(KeyCode.W),
			Input.GetKey(KeyCode.S),
			Input.GetKey(KeyCode.A),
			Input.GetKey(KeyCode.D)
		};

		// Convert world space rigidbody velocity to local velocity
		localVelocity = transform.InverseTransformDirection(rb.velocity);

		#region Wallrunning
		//if (/*canWallrun*/ true)
		//{
		//    if (Physics.Raycast(transform.position, transform.right, out RaycastHit hitR, 1))
		//    {
		//        wallrunDirection = Quaternion.AngleAxis(90, Vector3.up) * hitR.normal;
		//        wallrunNormal = hitR.normal;

		//        if (!isWallrunning)
		//        {
		//            StartWallrun(false);
		//        }
		//        Wallrun();
		//    }
		//    else if (Physics.Raycast(transform.position, -transform.right, out RaycastHit hitL, 1))
		//    {
		//        wallrunDirection = Quaternion.AngleAxis(-90, Vector3.up) * hitL.normal;
		//        wallrunNormal = hitL.normal;

		//        if (!isWallrunning)
		//        {
		//            StartWallrun(true);
		//        }
		//        Wallrun();
		//    }
		//    else
		//    {
		//        if (isWallrunning)
		//        {
		//            StartCoroutine(StopWallrun());
		//        }
		//    }

		//    //Debug.DrawRay(transform.position, wallrunDirection, Color.cyan, 1);
		//}
		#endregion

		#region Movement
		if (!isWallrunning)
		{
			if (localVelocity.z < maxSpeed)
			{
				if (inputs[0] && canMoveFAndB)
				{
					if (localVelocity.z < 0)
					{
						localVelocity.z = 0;
						rb.velocity = transform.TransformDirection(localVelocity);
					}
					rb.AddForce(transform.forward * acceleration, ForceMode.Force);
				}
			}
			if (localVelocity.z > -maxSpeed)
			{
				if (inputs[1] && canMoveFAndB)
				{
					if (localVelocity.z > 0)
					{
						localVelocity.z = 0;
						rb.velocity = transform.TransformDirection(localVelocity);
					}
					rb.AddForce(transform.forward * -acceleration, ForceMode.Force);
				}
			}

			if (localVelocity.x < maxSpeed)
			{
				if (inputs[3] && canMoveLAndR)
				{
					if (localVelocity.x < 0)
					{
						localVelocity.x = 0;
						rb.velocity = transform.TransformDirection(localVelocity);
					}
					rb.AddForce(transform.right * acceleration, ForceMode.Force);
				}
			}
			if (localVelocity.x > -maxSpeed)
			{
				if (inputs[2] && canMoveLAndR)
				{
					if (localVelocity.x > 0)
					{
						localVelocity.x = 0;
						rb.velocity = transform.TransformDirection(localVelocity);
					}
					rb.AddForce(transform.right * -acceleration, ForceMode.Force);
				}
			}

			//Counter movement
			if (isGrounded && (!inputs[0]) && (!inputs[1]))
			{
				if (localVelocity.z > 0)
				{
					rb.AddRelativeForce(Vector3.back * localVelocity.z * counterAcceleration, ForceMode.Force);
				}
				else if (localVelocity.z < 0)
				{
					rb.AddRelativeForce(Vector3.forward * -localVelocity.z * counterAcceleration, ForceMode.Force);
				}
			}

			if ((!inputs[2]) && (!inputs[3]))
			{
				if (localVelocity.x > 0)
				{
					rb.AddRelativeForce(Vector3.left * localVelocity.x * counterAcceleration, ForceMode.Force);
				}
				else if (localVelocity.x < 0)
				{
					rb.AddRelativeForce(Vector3.right * -localVelocity.x * counterAcceleration, ForceMode.Force);
				}
			}
		}
		#endregion
	}

	#region Collision
	private void OnCollisionEnter(Collision collision)
	{
		Vector3 normal = collision.GetContact(0).normal;
		float slopeAngle = Vector3.Angle(normal, Vector3.up);

		if (slopeAngle < 45f && slopeAngle > -45f)
		{
			isGrounded = true;
			jumpsLeft = maxJumps;
		}

		if (slopeAngle >= 45f && slopeAngle >= 0)
		{
			// wallrunDirection = Quaternion.AngleAxis(90, Vector3.up) * normal;
			wallrunNormal = normal;

			StartWallrun(false);
		}
		else if (slopeAngle <= -45f && slopeAngle <= 0)
		{
			// wallrunDirection = Quaternion.AngleAxis(-90, Vector3.up) * normal;
			wallrunNormal = normal;

			StartWallrun(true);
		}
	}

	private void OnCollisionStay(Collision collision)
	{
		Vector3 normal = collision.GetContact(0).normal;
		float slopeAngle;

		slopeAngle = Vector3.Angle(normal, Vector3.up);

		// if (!isWallrunning)
		// {
		// 	if (slopeAngle >= 45f && slopeAngle >= 0)
		// 	{
		// 		wallrunDirection = Quaternion.AngleAxis(90, Vector3.up) * normal;
		// 		wallrunNormal = normal;

		// 		StartWallrun(false);
		// 	}
		// 	else if (slopeAngle <= -45f && slopeAngle <= 0)
		// 	{
		// 		wallrunDirection = Quaternion.AngleAxis(-90, Vector3.up) * normal;
		// 		wallrunNormal = normal;

		// 		StartWallrun(true);
		// 	}
		// }

		if (collision.contactCount > 0)
		{
			foreach (ContactPoint contact in collision.contacts)
			{
				slopeAngle = Vector3.Angle(contact.normal, Vector3.up);
				// float e = 90;

				//! Smooth rotation aka wallrun assist
				// // cast a ray to the right of the player object
				// if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.right), out RaycastHit hit, 30))
				// {
				// 	// orient the Moving Object's Left direction to Match the Normals on his Right
				// 	var RunnerRotation = Quaternion.FromToRotation(Vector3.left, hit.normal);

				// 	//Smooth rotation
				// 	transform.rotation = Quaternion.Slerp(transform.rotation, RunnerRotation, Time.deltaTime * 10);
				// }

				//! stuff
				if (slopeAngle >= 45f && slopeAngle >= 0)
				{
					// e *= Mathf.Round(Vector3.Dot(transform.rotation.eulerAngles, contact.normal));
					// wallrunDirection = Quaternion.AngleAxis(e, Vector3.up) * contact.normal;

					// Calculate wallrun direction vector
					wallrunDirection = Vector3.ProjectOnPlane(transform.forward, contact.normal);
					wallrunNormal = contact.normal;

					Wallrun();
				}
				else if (slopeAngle <= -45f && slopeAngle <= 0)
				{
					// e *= Mathf.Round(Vector3.Dot(transform.rotation.eulerAngles, contact.normal));
					// wallrunDirection = Quaternion.AngleAxis(e, Vector3.up) * contact.normal;

					// Calculate wallrun direction vector
					wallrunDirection = Vector3.ProjectOnPlane(transform.forward, contact.normal);
					wallrunNormal = contact.normal;

					Wallrun();
				}
			}
		}
	}

	private void OnCollisionExit(Collision collision)
	{
		StartCoroutine(StopWallrun());
	}
	#endregion

	#region Wallrunning
	public void StartWallrun(bool leftSide)
	{
		isGrounded = true;
		rb.useGravity = false;
		rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
		jumpsLeft = maxJumps;
		isWallrunning = true;

		// e
		// Vector3 rotDir = Vector3.ProjectOnPlane(wallrunNormal, Vector3.up);
		// Quaternion rotation = Quaternion.AngleAxis(-90f, Vector3.up);
		// rotDir = rotation * rotDir;
		// float angle = Vector3.SignedAngle(Vector3.up, wallrunNormal, Quaternion.AngleAxis(90f, rotDir) * wallrunNormal);
		// angle -= 90;
		// angle /= 180;
		// Vector3 playerDir = transform.forward;
		// Vector3 normal = new Vector3(wallrunNormal.x, 0, wallrunNormal.z);

		// float camRot = Vector3.Cross(playerDir, normal).y * angle;
		// e

		//float someAngle = 0;
		//DOTween.To(() => someAngle, x => someAngle = x, cameraRotationAmount * camRot, cameraRotateSpeed)
		//    .OnUpdate(() =>
		//    {
		//        camera.transform.eulerAngles = new Vector3(camera.transform.eulerAngles.x, camera.transform.eulerAngles.y, someAngle);
		//    });
	}

	public IEnumerator StopWallrun()
	{
		isGrounded = false;
		rb.useGravity = true;
		wallrunNormal = Vector3.zero;
		isWallrunning = false;
		canWallrun = false;

		Debug.Log("Stop wallrun");
		float angle = camera.transform.eulerAngles.z;
		float zero;
		if (angle > 180)
		{
			zero = 360f;
		}
		else
		{
			zero = 0f;
		}
		DOTween.To(() => angle, x => angle = x, zero, cameraRotateSpeed)
			.OnUpdate(() =>
			{
				camera.transform.eulerAngles = new Vector3(camera.transform.eulerAngles.x, camera.transform.eulerAngles.y, angle);
			});

		yield return new WaitForSeconds(wallrunTimeout);
		canWallrun = true;
	}

	public void Wallrun()
	{
		// float angle = Vector3.Dot(transform.forward, wallrunDirection);
		// camera.transform.eulerAngles = new Vector3(camera.transform.eulerAngles.x, camera.transform.eulerAngles.y, cameraRotationAmount * angle);

		// Stick to wall
		// rb.AddForce(-wallrunNormal * 100);

		if (inputs[0] && canMoveFAndB && localVelocity.z < maxSpeed && localVelocity.x < maxSpeed)
		{
			rb.AddForce(wallrunDirection * acceleration, ForceMode.Force);
		}
		else if (inputs[1] && canMoveFAndB)
		{
			rb.AddForce(-wallrunDirection * acceleration, ForceMode.Force);
		}

		if (!inputs[0] && !inputs[1])
		{
			if (Vector3.Dot(rb.velocity, wallrunDirection) > 0)
			{
				rb.AddForce(wallrunDirection * -Vector3.Dot(rb.velocity, wallrunDirection) * counterAcceleration, ForceMode.Force);
			}
			else if (Vector3.Dot(rb.velocity, wallrunDirection) < 0)
			{
				rb.AddForce(wallrunDirection * -Vector3.Dot(rb.velocity, wallrunDirection) * counterAcceleration, ForceMode.Force);
			}
		}
	}
	#endregion

	#region Jumping
	public void Jump()
	{
		//if (currentHealth <= 0f)
		//{
		//    return;
		//}

		if (jumpsLeft > 0 && canJump)
		{
			if (isWallrunning)
			{
				rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
				rb.AddForce(new Vector3(0, jumpHeight, 0), ForceMode.Impulse);
				//rb.velocity = transform.TransformVector(new Vector3(0, 0, localVelocity.z * 2));
				jumpsLeft -= 1;
				StartCoroutine(StopWallrun());
			}
			else
			{
				rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
				rb.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
				jumpsLeft -= 1;
				isGrounded = false;
			}
		}
	}
	#endregion

	#region Running
	public void Run()
	{
		//if (isCrouching)
		//{
		//    return;
		//}

		if (isRunning == false)
		{
			maxSpeed = runMaxSpeed;
			acceleration *= 2;
			isRunning = true;
		}
		else
		{
			maxSpeed = walkMaxSpeed;
			acceleration /= 2;
			isRunning = false;
		}
	}
	#endregion

	#region Crouching
	public void Crouch()
	{
		if (isRunning)
		{
			Run();
		}

		if (isCrouching == false)
		{
			maxSpeed = crouchMaxSpeed;
			acceleration /= 2;
			counterAcceleration /= 2;
			isCrouching = true;
		}
		else
		{
			maxSpeed = walkMaxSpeed;
			acceleration *= 2;
			counterAcceleration *= 2;
			isRunning = false;
		}
	}
	#endregion
}