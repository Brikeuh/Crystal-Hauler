using UnityEngine;

public class ScurryAway : MonoBehaviour
{
    public float speed = 5f;
    public float lifetime = 3f;
    public Vector3 direction = new Vector3(1, 0, 0);

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.Translate(direction.normalized * speed * Time.deltaTime, Space.World);
    }
}
