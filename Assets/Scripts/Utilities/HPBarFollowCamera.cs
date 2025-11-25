using UnityEngine;

public class HPBarFollowCamera : MonoBehaviour
{
    private new Camera camera;

    private void Start()
    {
        camera = Camera.main;
    }
    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - camera.transform.position);
    }
}
