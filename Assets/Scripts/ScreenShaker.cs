using System.Collections;
using UnityEngine;

public class ScreenShaker : MonoBehaviour
{
	public static ScreenShaker instance;
	public AnimationCurve curve;

	private void Awake()
	{
		instance = this;
	}

	public void ShakeScreen(float duration, float magnitude)
	{
		StartCoroutine(ShakeScreenEnum(duration, magnitude));
	}

	private IEnumerator ShakeScreenEnum(float duration, float magnitude)
	{
		Vector3 startPosition = transform.position;
		float elapsedTime = 0f;

		while (elapsedTime < duration)
		{
			elapsedTime += Time.deltaTime;

			Vector3 positionAtFrame = transform.position;
			float strength = curve.Evaluate(elapsedTime / duration);
			transform.position = positionAtFrame + Random.insideUnitSphere * strength;

			yield return null;
		}

		transform.position = startPosition;
	}
}
