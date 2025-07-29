using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerInteraction : MonoBehaviour
{
    public CustomerOrder order;
    public CustomerQueueManager queueManager;
    public GameObject player;

    public bool playerInRange = false;
    private bool hasShownOrder = false;
    public bool drinkDelivered = false;
    public bool EmptyBool = true;

    public bool isAtFront = false;
    public PickUpScript pickUpScript;


    void Start()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player");

        if (pickUpScript == null && player != null)
            pickUpScript = player.GetComponentInChildren<PickUpScript>();
    }

    void Update()
    {
        if (pickUpScript != null)
        {
            Debug.Log("Is Holding Object: " + pickUpScript.isHoldingObject);
        }

        if (!playerInRange || !Input.GetKeyDown(KeyCode.E)) return;

        // Step 1: Customer hasn't ordered yet
        if (!order.HasOrdered)
        {
            Debug.Log("Customer has not ordered. Starting dialogue.");
            GameManager.Instance.ShowDialogue("What would you like?");
            order.StartOrder();

            return;
        }

        // Step 2: Customer is waiting for a drink
        if (order.IsWaitingForDrink)
        {
            if (pickUpScript != null && pickUpScript.isHoldingObject) 
            {
                TryDeliverDrink();
                Debug.Log("Player is holding a drink. Trying to deliver...");
            }
            else if (!hasShownOrder)
            {
                GameManager.Instance.ShowDialogue("I'd like a " + order.GetDrinkName() + " drink!");
                hasShownOrder = true;
            }
            else
            {
                Debug.Log("Player has no drink yet.");

            }
            return;
        }

        Debug.Log("Nothing to do.");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            Debug.Log("Player entered interaction range with customer.");
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

    void TryDeliverDrink()
    {
        Debug.Log("TryDeliverDrink() called.");

        if (!order.IsWaitingForDrink)
        {
            Debug.Log("Order is NOT waiting for a drink.");
            GameManager.Instance.ShowDialogue("I'm done!");
            return;
        }

        AcceptDrink();
    }

    void AcceptDrink()
    {
        Debug.Log("Drink accepted by customer.");
        GameManager.Instance.ShowDialogue("Thanks! That’s the drink I wanted!");

        if (pickUpScript != null && pickUpScript.HeldObject != null)
        {
            Destroy(pickUpScript.HeldObject);
            pickUpScript.ClearHeldObject();
        }

        drinkDelivered = true;
        Invoke(nameof(HideDialogue), 2f);
    }

    void HideDialogue()
    {
        GameManager.Instance.HideDialogue();
    }
}
