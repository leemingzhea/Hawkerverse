using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RedstoneinventeGameStudio.Tutorial
{
    public class AchievementCard : MonoBehaviour
    {
        public AchievementSO thisAchievement;

        [SerializeField] TMP_Text titleDisp;
        [SerializeField] TMP_Text descriptionDisp;

        [SerializeField] Image spriteDisp;
        [SerializeField] CanvasGroup canvasGroup;

        [SerializeField] AudioSource audioSource;

        public float waitBeforeExit;

        public float incr = 0.01f;
        public float wait = 0.001f;

        public void Initialize(AchievementSO achievementSO)
        {
            thisAchievement = achievementSO;
            RefreshDisp();

            StartCoroutine(WaitAndDestroy());
        }

        void RefreshDisp()
        {
            titleDisp.text = thisAchievement.name;
            descriptionDisp.text = thisAchievement.description;

            spriteDisp.sprite = thisAchievement.sprite;

            audioSource.PlayOneShot(thisAchievement.clip);
        }

        IEnumerator WaitAndDestroy()
        {
            yield return StartCoroutine(Fade(true));
            yield return new WaitForSeconds(waitBeforeExit);
            yield return StartCoroutine(Fade(false));

            Destroy(gameObject);
        }

        IEnumerator Fade(bool fadeIn)
        {
            for (float i = 0; i <= 1; i += incr)
            {
                canvasGroup.alpha = Mathf.Clamp01(fadeIn ? i : 1 - i);
                yield return new WaitForSeconds(wait);
            }
        }
    }
}