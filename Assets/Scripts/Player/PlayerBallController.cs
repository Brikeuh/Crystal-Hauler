using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerBallController : MonoBehaviour
{
    private Rigidbody rb;
    public TextMeshProUGUI countText;
    public GameObject winTextObject;

    private int count;

    private Vector3 jump;
    public float jumpForce = 1.0f;
    private bool isGrounded;

    public float speed = 10;
    private float movementX;
    private float movementY;

    void Start() 
    { 
        rb = GetComponent<Rigidbody>();
        jump = new Vector3(0.0f, 2.0f, 0.0f);
        count = 0;

        SetCountText();
        winTextObject.SetActive(false);
    }

    void OnMove(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();

        movementX = movementVector.x;
        movementY = movementVector.y;
    }

    void FixedUpdate()
    {
        Vector3 movement = new Vector3(movementX, 0.0f, movementY);

        rb.AddForce(movement * speed);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(jump * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Crystal") )
        {
            Debug.Log("Nom Nom Nom");
            other.gameObject.SetActive(false);
            count++;
            SetCountText();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (count == 0)
            {
                // Destroy the current object
                Destroy(gameObject);
                // Update the winText to display "You Lose!"
                winTextObject.gameObject.SetActive(true);
                winTextObject.GetComponent<TextMeshProUGUI>().color = Color.red;
                winTextObject.GetComponent<TextMeshProUGUI>().text = "You Lose!";
            }
            else if (count > 0)
            {
                if (collision.contacts.Length > 0) // Will replace this with a raycast in the future
                {
                    ContactPoint contact = collision.contacts[0];
                    Vector3 collisionDirection = contact.normal; // This is the direction of impact on *this* object
                    rb.AddForce(collisionDirection * 200f);
                    Debug.Log("Collision Direction: " + collisionDirection);
                }
                count--;
                SetCountText();
            }
            
            
        }
    }

    void OnCollisionStay()
    {
        isGrounded = true;
    }

    void SetCountText()
    {
        countText.text = "Crystals: " + count.ToString();
        if (count >= 12)
        {
            winTextObject.SetActive(true);
            Destroy(GameObject.FindGameObjectWithTag("Enemy"));
        }

    }
}
