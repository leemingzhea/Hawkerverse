using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeController : MonoBehaviour
{
    public CanvasGroup canvasGroup;       // Attached to the image you want to fade
    public Image currentImage;            // The image currently showing
    public Sprite[] imageSequence;        // List of sprites to swap between
    private int currentImageIndex = 0;

    public float fadeDuration = 1.0f;     // Duration of fade in/out
    public bool deactivateAfterFade = true; // Optionally deactivate the previous image GameObject

    void Start()
    {
        canvasGroup.alpha = 1f;
        currentImage.sprite = imageSequence[currentImageIndex];
    }

    public void OnImageClick()
    {
        // Go to the next image in the array
        int nextIndex = (currentImageIndex + 1) % imageSequence.Length;
        StartCoroutine(FadeToNextImage(nextIndex));
    }

    private IEnumerator FadeToNextImage(int nextIndex)
    {
        // Fade out
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 0f;

        // Switch image
        currentImage.sprite = imageSequence[nextIndex];
        currentImageIndex = nextIndex;

        // Fade in
        timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 1f;
    }
}
