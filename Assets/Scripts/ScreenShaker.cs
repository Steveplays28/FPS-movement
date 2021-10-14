using System;
using UnityEngine;

public class ScreenShaker : MonoBehaviour
{
	public static ScreenShaker instance;

	private void Awake()
	{
		instance = this;
	}
	public void ShakeScreen(float duration, float magnitude)
	{
		Vector3 originalPosition = transform.localPosition;

		float elapsedTime = 0f;

		while (elapsedTime < duration)
		{
			float x = UnityEngine.Random.Range(-1f, 1f) * magnitude;
			float y = UnityEngine.Random.Range(-1f, 1f) * magnitude;

			transform.localPosition = new Vector3(transform.localPosition.x + x, transform.localPosition.y + y, transform.localPosition.z);
		}
	}
}
