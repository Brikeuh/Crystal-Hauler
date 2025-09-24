using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player;
    private Vector3 offset;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        offset = transform.position - player.transform.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (player != null)
        {
            // The transform function here is attached to the CameraController "object". When you put player.transform, it then attaches to the player object created in line 5
            transform.position = player.transform.position + offset;
        }
    }
}
