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
    public AudioClip correctSound;
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

                winPanel.SetActive(false);
                wrongPanel.SetActive(true);
                return;
            }
        }

        if (audioSource && correctSound)
            audioSource.PlayOneShot(correctSound);

        winPanel.SetActive(true);
        wrongPanel.SetActive(false);
        Debug.Log("All slots correct! Showing win panel.");
    }
}
