using UnityEngine;

namespace TheCubers
{
	/// <summary>
	/// Translates and rotates from Point1 to Point2 over time.
	/// </summary>
	public class AniTranslate : MonoBehaviour
	{
		public Transform Point1;
		public Transform Point2;
		public float Speed;
		public AnimationCurve Curve;

		private float position;
		private float direction;

		void Start()
		{
			position = 0f;
			direction = 1f;
		}

		void Update()
		{
			position += Speed * direction * Time.deltaTime;
			if (position < 0f || position > 1f)
			{
				direction *= -1f;
				position += Speed * direction * Time.deltaTime;
			}
			Mathf.Clamp01(position);

			transform.position = Vector3.Lerp(Point1.position, Point2.position, Curve.Evaluate(position));
			transform.rotation = Quaternion.Lerp(Point1.rotation, Point2.rotation, Curve.Evaluate(position));
		}
	}
}
