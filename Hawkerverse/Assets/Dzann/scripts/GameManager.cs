using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject dialogueUI;
    public TextMeshProUGUI dialogueText;

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (dialogueUI != null)
            dialogueUI.SetActive(false); // Hide dialogue UI on start
    }

    public void ShowDialogue(string message)
    {
        if (dialogueUI != null && dialogueText != null)
        {
            dialogueUI.SetActive(true);
            dialogueText.text = message;
        }
    }

    public void HideDialogue()
    {
        if (dialogueUI != null)
        {
            dialogueUI.SetActive(false);
        }
    }
}
