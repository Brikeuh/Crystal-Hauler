using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class HUDController : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI healthText;
    public GameObject player;
    void SetScoreText(float score)
    {
        scoreText.text = "Crystals: " + score.ToString();
    }

    void SetHealthText(int health)
    {
        healthText.text = "Health: " + health.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        SetScoreText(player.GetComponent<PlayerController>().crystalCount);
        SetHealthText(player.GetComponent<PlayerController>().health);
    }
}
