using System.Collections;
using DG.Tweening;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	#region Variables
	[Header("Player")]
	public new Camera camera;
	public Rigidbody rb;
	public Collider wallCollider;

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
	public float maxWallrunDuration = 2f;
	public float wallrunTimeout = 2f;
	public bool canWallrun = true;
	public bool isWallrunning;

	private Vector3 wallrunDirection;
	private Vector3 wallrunNormal;
	private float wallrunStartTime;
	private float wallrunDuration;

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
			canWallrun = true;
		}

		if (!collision.contacts[0].thisCollider.CompareTag("Wall collider"))
		{
			return;
		}

		if ((slopeAngle >= 45f && slopeAngle >= 0) || (slopeAngle <= -45f && slopeAngle <= 0))
		{
			wallrunNormal = normal;
			StartWallrun(collision.GetContact(0));
		}
	}

	private void OnCollisionStay(Collision collision)
	{
		if (!collision.contacts[0].thisCollider.CompareTag("Wall collider"))
		{
			return;
		}

		Vector3 normal = collision.GetContact(0).normal;
		float slopeAngle;
		slopeAngle = Vector3.Angle(normal, Vector3.up);

		if (collision.contactCount > 0)
		{
			foreach (ContactPoint contact in collision.contacts)
			{
				slopeAngle = Vector3.Angle(contact.normal, Vector3.up);

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
					// Calculate wallrun direction vector
					wallrunDirection = Vector3.ProjectOnPlane(transform.forward, contact.normal);
					wallrunNormal = contact.normal;

					Wallrun(contact);
				}
				else if (slopeAngle <= -45f && slopeAngle <= 0)
				{
					// Calculate wallrun direction vector
					wallrunDirection = Vector3.ProjectOnPlane(transform.forward, contact.normal);
					wallrunNormal = contact.normal;

					Wallrun(contact);
				}
			}
		}
	}

	private void OnCollisionExit(Collision collision)
	{
		if (!isWallrunning)
		{
			return;
		}

		StartCoroutine(StopWallrun());
	}
	#endregion

	#region Wallrunning
	public void StartWallrun(ContactPoint contact)
	{
		if (!canWallrun)
		{
			return;
		}

		wallrunStartTime = Time.time;
		wallrunDuration = 0f;

		isGrounded = true;
		rb.useGravity = false;
		rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
		jumpsLeft = maxJumps;
		isWallrunning = true;

		// ScreenShaker.instance.ShakeScreen(0.5f, 0.125f);
		//TODO: implement features https://discord.com/channels/@me/834282615295967232/898327890289127426

		Vector3 line = transform.position - contact.point;
		bool isObjectOnRight;
		if (Vector3.Dot(transform.right, line) <= 0)
		{
			isObjectOnRight = true;
		}
		else
		{
			isObjectOnRight = false;
		}

		// Calculate dot product
		float dotProduct = Vector3.Dot(transform.forward.normalized, wallrunDirection.normalized);
		if (!isObjectOnRight)
		{
			dotProduct *= -1;
		}

		// Use dot product to rotate camera
		camera.transform.DORotate(new Vector3(camera.transform.eulerAngles.x, camera.transform.eulerAngles.y, camera.transform.eulerAngles.z + cameraRotationAmount * dotProduct), cameraRotateSpeed)
			.OnUpdate(() =>
			{
				float dotProduct = Vector3.Dot(transform.forward.normalized, wallrunDirection.normalized);
				if (!isObjectOnRight)
				{
					dotProduct *= -1;
				}
			});

		// float angle = camera.transform.eulerAngles.z;
		// DOTween.To(() => angle, x => angle = x, cameraRotationAmount * dotProduct, cameraRotateSpeed)
		// 	.OnUpdate(() =>
		// 	{
		// 		dotProduct = Vector3.Dot(transform.forward.normalized, wallrunDirection.normalized);
		// 		if (!isObjectOnRight)
		// 		{
		// 			dotProduct *= -1;
		// 		}

		// 		camera.transform.eulerAngles = new Vector3(camera.transform.eulerAngles.x, camera.transform.eulerAngles.y, angle);
		// 	});
	}

	public IEnumerator StopWallrun()
	{
		isGrounded = false;
		rb.useGravity = true;
		wallrunNormal = Vector3.zero;
		isWallrunning = false;
		canWallrun = false;

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

	public void Wallrun(ContactPoint contact)
	{
		if (!canWallrun)
		{
			return;
		}

		if (wallrunDuration >= maxWallrunDuration)
		{
			StartCoroutine(StopWallrun());
			return;
		}

		Vector3 line = transform.position - contact.point;
		bool isObjectOnRight;
		if (Vector3.Dot(transform.right, line) <= 0)
		{
			isObjectOnRight = true;
		}
		else
		{
			isObjectOnRight = false;
		}

		// Calculate dot product
		float dotProduct = Vector3.Dot(transform.forward.normalized, wallrunDirection.normalized);
		if (!isObjectOnRight)
		{
			dotProduct *= -1;
		}

		// Use dot product to rotate camera
		camera.transform.eulerAngles = new Vector3(camera.transform.eulerAngles.x, camera.transform.eulerAngles.y, cameraRotationAmount * dotProduct);

		// Debug.Log("Dot product: " + dotProduct);

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

		wallrunDuration = Time.time - wallrunStartTime;
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