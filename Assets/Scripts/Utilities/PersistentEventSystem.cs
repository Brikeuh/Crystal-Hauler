using UnityEngine;
using UnityEngine.EventSystems; // Required for EventSystem

public class PersistentEventSystem : MonoBehaviour
{
    private static PersistentEventSystem instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            // Destroy duplicate Event Systems if one already exists
            Destroy(gameObject);
        }
    }
}