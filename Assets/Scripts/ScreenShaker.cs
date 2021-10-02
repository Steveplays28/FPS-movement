using System;
using UnityEngine;

namespace Assets.Scripts
{
    public class ScreenShaker : MonoBehaviour
    {
		public ScreenShaker instance;

		private void Awake() {
			instance=this;
		}
        public void ShakeScreen(float duration, float magnitude) {
			Vector3 originalPosition = transform.localPosition;

			float elapsedTime = 0f;

			while (elapsedTime < duration) {
				float x = Random.Range(-1f, 1f) * magnitude;
				float y = Random.Range(-1f, 1f) * magnitude;

				transform.localPosition = new Vector3(transform.localPosition + x, transform.localPosition + y, transform.localPosition.z);
			}
		}
    }
}
