using UnityEngine;

public class RockImpactSound : MonoBehaviour
{
    private AudioSource audioSource;

    void Start()
    {
        // Get the AudioSource on this rock
        audioSource = GetComponent<AudioSource>();
    }

    void OnCollisionEnter(Collision collision)
    {
        // Only play if it's a strong enough impact
        if (collision.relativeVelocity.magnitude > 2f)
        {
            if (audioSource != null && audioSource.clip != null)
            {
                audioSource.Play();
            }
        }
    }
}
