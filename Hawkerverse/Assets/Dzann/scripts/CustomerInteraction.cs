using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerInteraction : MonoBehaviour
{
    public CustomerOrder order;
    public CustomerQueueManager queueManager;
    public GameObject player;

    private bool playerInRange = false;
    private bool hasShownOrder = false;
    public bool drinkDelivered = false;

    public bool isAtFront = false;


    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("E pressed and player is in range.");

            if (!order.HasOrdered) // Corrected from 'customerOrder'
            {
                Debug.Log("Customer has not ordered. Starting dialogue.");
                GameManager.Instance.ShowDialogue("What would you like?");
                order.StartOrder(); // Start the customer's order
            }
            else if (order.IsWaitingForDrink) // Corrected from 'customerOrder'
            {
                Debug.Log("Customer has ordered. Trying to deliver drink.");
                TryDeliverDrink(); // Try delivering the drink
            }
            else
            {
                Debug.Log("Nothing to do.");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            GameManager.Instance.HideDialogue();
        }
    }

    void ShowPromptAfterOrder()
    {
        if (playerInRange && order.IsWaitingForDrink)
        {
            GameManager.Instance.ShowDialogue("I'd like a " + order.GetDrinkName() + " drink!");
        }
    }

    void TryDeliverDrink()
    {
        Debug.Log("TryDeliverDrink() called.");

        if (!order.IsWaitingForDrink)
        {
            Debug.Log("Order is NOT waiting for a drink.");
            GameManager.Instance.ShowDialogue("I'm done!");
            return;
        }
        else
        {
            Debug.Log("Order is waiting for a drink.");
        }

        PickUpScript pickUpScript = player.GetComponent<PickUpScript>();
        if (pickUpScript == null)
        {
            Debug.LogWarning("PickUpScript component NOT found on player.");
            GameManager.Instance.ShowDialogue("I'd like a " + order.GetDrinkName() + " drink!");
            return;
        }

        if (pickUpScript.HeldObject == null)
        {
            Debug.Log("Player is NOT holding any drink.");
            GameManager.Instance.ShowDialogue("I'd like a " + order.GetDrinkName() + " drink!");
            return;
        }

        GameObject heldCup = pickUpScript.HeldObject;
        Debug.Log("Player is holding cup: " + heldCup.name);

        bool accepted = order.TryReceiveDrink(heldCup);
        Debug.Log($"order.TryReceiveDrink returned: {accepted}");

        if (accepted)
        {
            Debug.Log("Drink accepted by customer.");
            GameManager.Instance.ShowDialogue("Thanks! That’s the drink I wanted!");
            Destroy(heldCup);
            pickUpScript.ClearHeldObject();
            Invoke(nameof(HideDialogue), 2f);
            drinkDelivered = true;
        }
        else
        {
            Debug.Log("Drink was NOT accepted by customer.");
            GameManager.Instance.ShowDialogue("That's not what I ordered.");
            Invoke(nameof(HideDialogue), 2f);
        }
    }

    void HideDialogue()
    {
        GameManager.Instance.HideDialogue();
    }
}
