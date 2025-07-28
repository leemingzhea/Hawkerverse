using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public Button[] levelButtons; // Assign buttons for Levels 1-8 in order

    void Start()
    {
        // First level is always unlocked
        PlayerPrefs.SetInt("Level1Unlocked", 1);

        for (int i = 0; i < levelButtons.Length; i++)
        {
            int levelNumber = i + 1;
            bool isUnlocked = PlayerPrefs.GetInt($"Level{levelNumber}Unlocked", 0) == 1;

            levelButtons[i].interactable = isUnlocked;
        }
    }

    public void LoadLevel(string levelName)
    {
        SceneManager.LoadScene(levelName);
    }
}
