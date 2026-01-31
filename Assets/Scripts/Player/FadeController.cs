using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeController : MonoBehaviour
{
    public static FadeController instance;

    public Image fadeImage;
    public float fadeSpeed = 1f;

    void Awake()
    {
        instance = this;
    }

    public IEnumerator FadeToBlack()
    {
        Color c = fadeImage.color;
        while (c.a < 1f)
        {
            c.a += Time.deltaTime * fadeSpeed;
            fadeImage.color = c;
            yield return null;
        }
    }

    public IEnumerator FadeToClear()
    {
        Color c = fadeImage.color;
        while (c.a > 0f)
        {
            c.a -= Time.deltaTime * fadeSpeed;
            fadeImage.color = c;
            yield return null;
        }
    }
}
