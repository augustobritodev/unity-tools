using UnityEngine;

public class TopDownCamera : MonoBehaviour {

	private struct SafeZone
	{
		public Vector3 center;
		
		public SafeZone(Bounds bounds, Vector3 size)
		{
			center = Vector3.zero;
		}
		
		public void Update(Bounds bounds)
		{
			
		}
	}
	
	[Header("References")]

	[Tooltip("A target for the camera to follow.")]
	[SerializeField]
	private Transform target;

	[Tooltip("Target top collider. Used to calculate distance between collider top point and camera.")]
	[SerializeField]
	private Collider topCollider;

	private Vector3 currentVelocity;

	[Header("Settings")]

	[SerializeField] 
	[Range(0.0f, 1.0f)]
	private float smoothTime = 0.1f;

	[SerializeField] 
	[Range(0.0f, 20.0f)]
	private float height = 8f;

	[SerializeField] 
	[Range(45f, 90f)]
	private float angle = 90f;

	[SerializeField] 
	[Range(0.1f, 10f)]
	private float angleFactor = 90f;

	[Header("Look Ahead")]

	[SerializeField] 
	[Range(0.0f, 20.0f)]
	private float horizontalLookAhead = 5f;

	[SerializeField] 
	[Range(0.0f, 20.0f)]
	private float verticalLookAhead = 5f;

	[Header("Safe Zone")]

	[Tooltip("Safe Zone Size")]
	[SerializeField]
	private Vector3 safeZoneSize;

	[Tooltip("Enable/Disable Safe Zone")]
	[SerializeField]
	private bool enableSafeZone;

	[Tooltip("Color of Safe Zone")]
	[SerializeField]
	private Color colorSafeZone = Color.blue;

	// Private fields
	private SafeZone safe;
	private Camera cam;

	private void Awake() {
		// Try to find default target and top collider
		if (target == null) {
			target = FindObjectOfType<TopDownPlayer>().transform;
		}
		if (topCollider == null) {
			topCollider = target.Find("Head").GetComponent<Collider>();
		}

		cam = GetComponent<Camera>();

		safe = new SafeZone(topCollider.bounds, safeZoneSize);
	}

	private void OnDrawGizmos() {
		Gizmos.color = colorSafeZone;
		Gizmos.DrawCube(safe.center, safeZoneSize);
	}

	private void LateUpdate() {
		if(target != null && topCollider != null) {

			safe.Update(topCollider.bounds);

			Vector3 dir = target.position - transform.position;
			Debug.DrawRay(transform.position, dir, Color.red);

			// Use law of sines to calculate Z distance based in camera angle.
			var zRelativeToAngle =  height/Mathf.Sin(angle * Mathf.Deg2Rad) * Mathf.Sin((90 - angle) * Mathf.Deg2Rad);

			var dx = (Input.mousePosition.x - Screen.width * 0.5f) / Screen.width;;
			var dz = (Input.mousePosition.y - Screen.height * 0.5f) / Screen.height;

			// Set Target Position
			Vector3 targetPosition = new Vector3(
				target.position.x + dx * horizontalLookAhead,
				height,
				target.position.z - zRelativeToAngle + dz * verticalLookAhead
			);

			// Set Current Position
			transform.position = Vector3.SmoothDamp(
				transform.position,
				targetPosition,
				ref currentVelocity,
				smoothTime
			);

			// Set Rotation
			transform.eulerAngles = new Vector3(
				angle - dz * angleFactor,
				0,
				0
			);
		}
		else {
			Debug.LogWarning("Target or Top Collider not found!");
		}
	}
}
