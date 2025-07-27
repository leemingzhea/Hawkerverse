using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CustomerInteraction : MonoBehaviour
{
    public GameObject player;
    public GameObject dialogueUI;
    public TextMeshProUGUI dialogueText;

    private bool playerInRange = false;
    private CustomerOrder order;
    private bool hasShownOrder = false;
    public bool drinkDelivered = false;

    void Start()
    {
        order = GetComponent<CustomerOrder>();
        HideDialogue();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;

            // Show order if it's the first interaction
            if (!hasShownOrder && order.IsWaitingForDrink)
            {
                ShowDialogue("Hi! I'd like a " + order.GetDrinkName() + " drink!");
                hasShownOrder = true;
                Invoke(nameof(ShowPromptAfterOrder), 2.5f); // Show delivery prompt after a short delay
            }
            else
            {
                ShowDialogue("I'd like a " + order.GetDrinkName() + " drink!");
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            HideDialogue();
        }
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            TryDeliverDrink();
        }
    }

    void ShowPromptAfterOrder()
    {
        if (playerInRange && order.IsWaitingForDrink)
        {
            ShowDialogue("I'd like a " + order.GetDrinkName() + " drink!");
        }
    }

    void ShowDialogue(string message)
    {
        if (dialogueUI != null && dialogueText != null)
        {
            dialogueUI.SetActive(true);
            dialogueText.text = message;
        }
    }

    void HideDialogue()
    {
        if (dialogueUI != null)
        {
            dialogueUI.SetActive(false);
        }
    }

    void TryDeliverDrink()
    {
        if (!order.IsWaitingForDrink)
        {
            ShowDialogue("I'm done!");
            return;
        }

        PickUpScript pickUpScript = player.GetComponent<PickUpScript>();
        if (pickUpScript.HeldObject == null)
        {
            ShowDialogue("I'd like a " + order.GetDrinkName() + " drink!");
            return;
        }

        // Get reference to held cup
        GameObject heldCup = pickUpScript.HeldObject;

        // Try delivering using centralized method from CustomerOrder
        bool accepted = order.TryReceiveDrink(heldCup);

        if (accepted)
        {
            ShowDialogue("Thanks! That’s the drink I wanted!");
            Destroy(heldCup);
            pickUpScript.ClearHeldObject();
            Invoke(nameof(HideDialogue), 2f);
            drinkDelivered = true;

        }
        else
        {
            ShowDialogue("That's not what I ordered.");
            Invoke(nameof(HideDialogue), 2f);
        }
    }
}
