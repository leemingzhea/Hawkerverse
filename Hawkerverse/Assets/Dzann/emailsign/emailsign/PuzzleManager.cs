using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    public PuzzleSlot[] slots;
    public GameObject winPanel;
    public GameObject wrongPanel;
    
    [Header("Sound Effects")]
    public AudioSource audioSource;
     [Header("CorrectSoundClip")]
    public AudioClip correctSound;
    [Header("WrongSoundClip")]
    public AudioClip wrongSound;

    void Awake()
    {
        winPanel.SetActive(false);
        wrongPanel.SetActive(false);
    }

    void Update()
    {
        if (wrongPanel.activeSelf && Input.GetMouseButtonDown(0))
        {
            wrongPanel.SetActive(false);
        }
    }

    public void CheckAnswers()
    {
        foreach (var slot in slots)
        {
            if (slot.placedPiece == null || !slot.IsCorrect())
            {
                if (audioSource && wrongSound)
                    audioSource.PlayOneShot(wrongSound);
                Debug.Log("Playing wrong audio");

                winPanel.SetActive(false);
                wrongPanel.SetActive(true);
                return;
            }
            else if (slot.placedPiece == null || slot.IsCorrect())
            {
                if (audioSource && correctSound)
                    audioSource.PlayOneShot(correctSound);
                Debug.Log("Playing correct audio");

                winPanel.SetActive(true);
                wrongPanel.SetActive(false);
                Debug.Log("All slots correct! Showing win panel.");
                
            }
        }
    }
}
