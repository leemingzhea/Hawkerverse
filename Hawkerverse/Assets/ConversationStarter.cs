using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueEditor;

public class ConversationStarter : MonoBehaviour
{
    [SerializeField] private NPCConversation taxConversation;
    public GameObject badgeUI;

    private void Start()
    {
        badgeUI.SetActive(false);
    }
    private void OnTriggerStay(Collider other)
    {
        Debug.Log("OnTriggerStay called, player entered trigger");
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player is in range for conversation.");
            if (Input.GetKeyDown(KeyCode.F))
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                Debug.Log("F key pressed, starting conversation.");
                ConversationManager.Instance.StartConversation(taxConversation);
            }
        }
    }

    public void ShowBadgeUI()
    {
        if (badgeUI != null)
        {
            badgeUI.SetActive(true);
            Debug.Log("Badge UI shown.");
        }
        else
        {
            Debug.LogWarning("Badge UI is not assigned.");
        }
    }
    public void HideBadgeUI()
    {
        if (badgeUI != null)
        {
            badgeUI.SetActive(false);
            Debug.Log("Badge UI hidden.");
        }
        else
        {
            Debug.LogWarning("Badge UI is not assigned.");
        }
    }
}