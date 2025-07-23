using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class CustomerInteraction : MonoBehaviour
{
    public GameObject chatUI;
    public TextMeshProUGUI dialogueText;
    public Transform interactionCameraPos;
    public Camera mainCamera;

    private bool inRange = false;
    private bool hasOrdered = false;

    private CustomerOrder customerOrder;

    void Start()
    {
        chatUI.SetActive(false);
        customerOrder = GetComponent<CustomerOrder>();  // Get reference to the CustomerOrder script
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            inRange = true;
            Debug.Log("Player entered interaction zone.");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            inRange = false;
            Debug.Log("Player left interaction zone.");
        }
    }

    void Update()
    {
        if (inRange && !hasOrdered && customerOrder != null)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("E key pressed, starting order interaction.");

                // Compose order string based on CustomerOrder's requested fruits
                string orderText = $"Hi! I'd like a {string.Join(" and ", customerOrder.requestedFruits)} smoothie, please.";

                ShowDialogue(orderText);
                hasOrdered = true;

                // Move camera to face the customer
                mainCamera.transform.position = interactionCameraPos.position;
                mainCamera.transform.rotation = interactionCameraPos.rotation;
            }
        }
    }

    public void ShowDialogue(string text)
    {
        chatUI.SetActive(true);
        dialogueText.text = text;
    }

    public void HideDialogue()
    {
        chatUI.SetActive(false);
    }
}
