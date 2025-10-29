using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player;
    public float smoothSpeed = 5f;
    public float transitionCooldown = 0.5f; // delay before rechecking state after change

    private Vector3 originalOffset;
    private Quaternion originalRotation;

    private bool isTopDown = false;
    private float transitionTimer = 0f;
    private Camera cam;
    public static bool ChangeView = false;
    void Awake()
    {
        if (player == null)
        {
            Debug.LogError("CameraController: Player not assigned!");
            return;
        }

        // Store the initial offset and rotation of the camera
        originalOffset = transform.position - player.transform.position;
        originalRotation = transform.rotation;
        cam = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        if (player == null)
            return;

        transitionTimer -= Time.deltaTime;

        bool blocked = IsViewBlocked();

        // Only allow state changes if cooldown has expired
        if (transitionTimer <= 0f)
        {
            if (blocked && !isTopDown)
            {
                isTopDown = true;
                transitionTimer = transitionCooldown; // lock state briefly
            }
            else if (!blocked && isTopDown)
            {
                isTopDown = false;
                transitionTimer = transitionCooldown; // lock state briefly
            }
        }

        if (isTopDown)
        {
            // Move camera directly above player
            Vector3 topDownPos = player.transform.position + Vector3.up * originalOffset.magnitude;
            transform.position = Vector3.Lerp(transform.position, topDownPos, Time.deltaTime * smoothSpeed);

            // Look straight down
            Quaternion topDownRot = Quaternion.Euler(90f, 0f, 0f);
            transform.rotation = Quaternion.Lerp(transform.rotation, topDownRot, Time.deltaTime * smoothSpeed);
        }
        else
        {
            // Restore original position and rotation
            Vector3 targetPos = player.transform.position + originalOffset;
            transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * smoothSpeed);
            transform.rotation = Quaternion.Lerp(transform.rotation, originalRotation, Time.deltaTime * smoothSpeed);
        }
    }

    bool IsViewBlocked()
    {
        // Compute where the camera *should* be if it were in original position
    Vector3 intendedCameraPos = player.transform.position + originalOffset;

    // Direction from intended camera position to player
    Vector3 direction = player.transform.position - intendedCameraPos;
    float distance = direction.magnitude;

    // Avoid self-collision
    if (distance < 0.1f)
        return false;

    // Raycast from the intended position (not current position)
    if (Physics.Raycast(intendedCameraPos, direction.normalized, out RaycastHit hit, distance))
    {
        if (hit.transform.gameObject != player)
            return true; // Something is between camera and player
    }

    return false;
       
    }
}
