using System.Collections;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class TakeDamage : MonoBehaviour
{
    public float intensity = 0;

    Volume volume;
    Vignette vignette;
    VolumeProfile profile;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        volume = GetComponent<Volume>();

        profile = volume.profile;

        profile.TryGet<Vignette>(out vignette);

        if (!vignette)
        {
            Debug.Log("Vignette override is empty, please add a vignette override for this effect to work.");
            return;
        }
        else
        {
            vignette.active = false;
        }
    }

    public void DisplayDamageEffect()
    {
        StartCoroutine(TakeDamageEffect());
    }

    private IEnumerator TakeDamageEffect()
    {
        intensity = 0.4f;

        vignette.active = true;
        vignette.intensity.Override(intensity);

        yield return new WaitForSeconds(0.4f);

        while (intensity > 0)
        {
            intensity -= 0.01f;
            if (intensity < 0)
            {
                intensity = 0;
            }
            vignette.intensity.Override(intensity);
            yield return new WaitForSeconds(0.1f);
        }

        vignette.active = false;
        yield break;

    }
}
