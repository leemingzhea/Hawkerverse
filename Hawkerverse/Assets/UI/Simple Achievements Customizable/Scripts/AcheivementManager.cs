using UnityEngine;

namespace RedstoneinventeGameStudio.Tutorial
{
    public class AcheivementManager : MonoBehaviour
    {
        public delegate void TriggerAchievement(AchievementSO achievementSO);
        public static TriggerAchievement triggerAchievement;

        public GameObject achievementCard;
        public Transform achievementTransform;

        private void Awake()
        {
            triggerAchievement += OnAchievementTriggered;
        }

        private void OnDestroy()
        {
            triggerAchievement -= OnAchievementTriggered;
        }

        public void OnAchievementTriggered(AchievementSO achievementSO)
        {
            GameObject card = Instantiate(achievementCard, achievementTransform);
            card.GetComponent<AchievementCard>().Initialize(achievementSO);
        }
    }
}