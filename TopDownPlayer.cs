using UnityEngine;

public class TopDownPlayer : MonoBehaviour {
    [SerializeField] private float speed = 10;

    public Vector3 velocity;
	
    private void Update() {
        Vector3 inputDir = new Vector3(
            Input.GetAxisRaw("Horizontal"),
            0,
            Input.GetAxisRaw("Vertical")
        );

        velocity = inputDir.normalized * speed * Time.deltaTime;

        transform.Translate(velocity);
    }
}
