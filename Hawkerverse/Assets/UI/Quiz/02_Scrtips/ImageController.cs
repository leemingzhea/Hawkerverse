using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Lordmin
{
    public class ImageController : MonoBehaviour
    {

        private Image MyImage;

        private void Awake()
        {
            MyImage = transform.GetComponent<Image>();
        }

        private void OnEnable()
        {
            StartCoroutine(AlphaAnimation());
        }

        IEnumerator AlphaAnimation(float startAlpha = 0, float endAlpha = 1, float time = 1)
        {
            float t = 0;
            while (t < time)
            {
                t += Time.deltaTime;
                float a = Mathf.Lerp(startAlpha, endAlpha, t / time);
                MyImage.color = new Color(1, 1, 1, a);
                yield return null;
            }
        }
    }
}