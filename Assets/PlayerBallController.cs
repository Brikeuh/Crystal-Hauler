using UnityEngine;
using UnityEngine.UI;

public class PlayerBallController : MonoBehaviour
{
    public float force = 12f;
    public float maxSpeed = 10f;
    public Text scoreText;
    public Text playerNameText;
    int score = 0;
    Rigidbody rb;

    void Awake() { rb = GetComponent<Rigidbody>(); UpdateHUD(); }

    void FixedUpdate()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 input = new Vector3(h, 0f, v);
        rb.AddForce(input * force, ForceMode.Acceleration);
        Vector3 vel = rb.linearVelocity; Vector3 flat = new Vector3(vel.x, 0, vel.z);
        if (flat.magnitude > maxSpeed) { flat = flat.normalized * maxSpeed; rb.linearVelocity = new Vector3(flat.x, vel.y, flat.z); }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Collectible")) { score++; Destroy(other.gameObject); UpdateHUD(); }
    }

    void UpdateHUD() { if (scoreText) scoreText.text = $"Crystals: {score}"; }
}
