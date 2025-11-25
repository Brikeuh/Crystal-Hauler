using UnityEngine;

public class FootprintFade : MonoBehaviour
{
    public float lifetime = 3f;
    public float fadeTime = 1f;
    Material mat;
    Color originalColor;

    void Start()
    {
        var renderer = GetComponentInChildren<Renderer>();
        if (renderer != null)
        {
            mat = renderer.material;
            originalColor = mat.color;
        }
        Destroy(gameObject, lifetime + fadeTime);
        StartCoroutine(FadeOut());
    }

    System.Collections.IEnumerator FadeOut()
    {
        yield return new WaitForSeconds(lifetime);
        float t = 0f;
        while (t < fadeTime)
        {
            t += Time.deltaTime;
            if (mat != null)
            {
                float alpha = Mathf.Lerp(1f, 0f, t / fadeTime);
                mat.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            }
            yield return null;
        }
    }
}
