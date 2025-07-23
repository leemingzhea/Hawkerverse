using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrinkDelivery : MonoBehaviour
{
    private bool isHoldingCup = false;
    private GameObject heldCup = null;

    void Update()
    {
        if (isHoldingCup && Input.GetKeyDown(KeyCode.E)) // press E near customer to deliver
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, 2f); // check nearby

            foreach (Collider hit in hits)
            {
                if (hit.CompareTag("Customer"))
                {
                    CustomerOrder customerOrder = hit.GetComponent<CustomerOrder>();

                    if (customerOrder != null && customerOrder.IsWaitingForDrink)
                    {
                        customerOrder.Interact(heldCup);

                        if (!customerOrder.IsWaitingForDrink) // means drink accepted
                        {
                            Destroy(heldCup);
                            heldCup = null;
                            isHoldingCup = false;
                        }
                    }


                        return;
                }
            }
        }
    }
    

    public void SetHeldCup(GameObject cup)
    {
        heldCup = cup;
        isHoldingCup = true;
    }
}