using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrinkDelivery : MonoBehaviour
{
    [SerializeField] private float interactRange = 2f;  // Fixed: define interact range in inspector
    private GameObject heldCup = null;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log($"[DrinkDelivery] Interacting with: {heldCup?.name}");

            Collider[] hits = Physics.OverlapSphere(transform.position, interactRange);
            foreach (var hit in hits)
            {
                CustomerOrder customerOrder = hit.GetComponent<CustomerOrder>();
                if (customerOrder != null)
                {
                    if (heldCup != null)
                    {
                        bool success = customerOrder.TryReceiveDrink(heldCup);
                        Debug.Log($"Trying to give drink to {customerOrder.name}: {(success ? "SUCCESS" : "FAIL")}");
                        if (success)
                        {
                            // Destroy cup or mark as used
                            Destroy(heldCup);
                            heldCup = null;
                            return;
                        }
                    }
                    else
                    {
                        Debug.Log("No drink held to deliver.");
                    }
                }
            }
        }
    }

    public void SetHeldCup(GameObject cup)
    {
        heldCup = cup;
    }

    public GameObject GetHeldCup()
    {
        return heldCup;
    }

    public bool HasCup()
    {
        return heldCup != null;
    }
}
