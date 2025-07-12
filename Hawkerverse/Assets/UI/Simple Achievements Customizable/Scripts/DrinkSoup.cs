using UnityEngine;

namespace RedstoneinventeGameStudio.Tutorial
{
    public class DrinkSoup : MonoBehaviour
    {
        public AchievementSO drinkSoupAchievement;

        public void DrinkSoupM()
        {
            AcheivementManager.triggerAchievement?.Invoke(drinkSoupAchievement);
        }
    }
}
