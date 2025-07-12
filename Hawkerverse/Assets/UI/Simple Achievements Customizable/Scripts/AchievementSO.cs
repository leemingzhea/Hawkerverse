using UnityEngine;

namespace RedstoneinventeGameStudio.Tutorial
{
    [CreateAssetMenu(fileName = "New Achievement", menuName = "New Achievement")]
    public class AchievementSO : ScriptableObject
    {
        public new string name;
        public string description;

        public Sprite sprite;
        public AudioClip clip;
    }
}

