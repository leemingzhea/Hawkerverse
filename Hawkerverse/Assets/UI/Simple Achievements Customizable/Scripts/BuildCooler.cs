using UnityEngine;

namespace RedstoneinventeGameStudio.Tutorial
{
    public class BuildCooler : MonoBehaviour
    {
        public AchievementSO buildAchievement;

        public void Build()
        {
            AcheivementManager.triggerAchievement?.Invoke(buildAchievement);
        }
    }
}
