using UnityEngine;

public class FootstepFootprintSpawner : MonoBehaviour
{
    public GameObject footprintPrefab;
    public float stepDistance = 0.7f;
    public Transform leftFoot;
    public Transform rightFoot;

    bool isLeft = true;
    Vector3 lastPos;

    void Start()
    {
        lastPos = transform.position;
    }

    void Update()
    {
        float dist = Vector3.Distance(transform.position, lastPos);
        if (dist >= stepDistance && IsMoving())
        {
            SpawnFootprint();
            lastPos = transform.position;
            isLeft = !isLeft;
        }
    }

    bool IsMoving()
    {
        // adapt if you have your own movement controller
        return new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).sqrMagnitude > 0.01f;
    }

    void SpawnFootprint()
    {
        Transform foot = isLeft && leftFoot != null ? leftFoot : (rightFoot != null ? rightFoot : transform);
        Vector3 pos = new Vector3(foot.position.x, foot.position.y + 0.02f, foot.position.z);
        Quaternion rot = Quaternion.Euler(90, transform.eulerAngles.y, 0);
        Instantiate(footprintPrefab, pos, rot);
    }
}
