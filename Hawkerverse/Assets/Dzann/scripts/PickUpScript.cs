using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PickUpScript : MonoBehaviour
{
    public GameObject player;
    public Transform holdPos;
    public float throwForce = 500f;
    public float pickUpRange = 5f;
    public GameObject grabEText;
    public bool isHoldingObject = false;

    [SerializeField] private GameObject heldObj;

    public GameObject HeldObject
    {
        get { return heldObj; }
        set { heldObj = value; }
    }

    private Rigidbody heldObjRb;
    private int LayerNumber;
    private Blender targetedBlender;
    private Vector3 originalScale;
    public CustomerInteraction customerInteraction;


    void Start()
    {
        LayerNumber = LayerMask.NameToLayer("holdLayer");
        if (grabEText != null)
            grabEText.gameObject.SetActive(false);
    }

    void Update()
    {
        CheckForBlender();
        HandleHoverText();

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (heldObj != null)
            {
                // 👉 Only do Blender logic if we're actually looking at a blender
                if (targetedBlender != null)
                {
                    string heldName = heldObj.name.ToLower();

                    // Holding a fruit
                    if (IsFruitName(heldName))
                    {
                        if (!targetedBlender.IsBlended)
                        {
                            string fruitType = heldName.Split(' ')[0];
                            targetedBlender.AddFruit(fruitType);

                            Destroy(heldObj);
                            heldObj = null;
                            heldObjRb = null;
                            isHoldingObject = false;
                        }
                        else
                        {
                            Debug.Log("Blender must be cleared before adding more fruit.");
                        }
                    }
                    // Holding a cup
                    else if (heldObj.GetComponent<Cup>() != null)
                    {
                        Cup cup = heldObj.GetComponent<Cup>();
                        bool success = targetedBlender.TransferToCup(cup);
                        if (success)
                        {
                            return;
                        }
                    }
                }
                else if (customerInteraction == null || !customerInteraction.playerInRange)
                {
                    // ❗ Only drop if NOT looking at blender AND not inside customer trigger
                    //DropObject();
                    Debug.Log("Dropping object: " + heldObj.name);
                }
            }
            else
            {
                // Not holding anything
                if (targetedBlender != null)
                {
                    targetedBlender.Blend();
                }
                else
                {
                    TryPickUp();
                }
            }
        }

        if (heldObj != null)
        {
            MoveObject();

            if (Input.GetMouseButtonDown(0))
            {
                ThrowObject();
            }
        }
    }

    public void ClearHeldObject()
    {
        HeldObject = null;
        heldObjRb = null;
        isHoldingObject = false; // <--- Add this line
    }

    void HandleHoverText()
    {
        if (grabEText == null) return;

        if (heldObj != null)
        {
            grabEText.gameObject.SetActive(false);
            return;
        }

        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, pickUpRange))
        {
            string objName = hit.collider.gameObject.name.ToLower();

            if (IsFruitName(objName) || hit.collider.GetComponent<Blender>() != null || hit.collider.GetComponent<Cup>() != null)

            {
                grabEText.gameObject.SetActive(true);
                return;
            }
        }

        grabEText.gameObject.SetActive(false);
    }

    void CheckForBlender()
    {
        targetedBlender = null;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 2f))
        {
            Blender blender = hit.collider.GetComponent<Blender>();
            if (blender != null)
            {
                targetedBlender = blender;
            }
        }
    }

    void TryPickUp()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, pickUpRange))
        {
            string objName = hit.collider.gameObject.name.ToLower();

            if (IsFruitName(objName) || hit.collider.GetComponent<Cup>() != null)
            {
                GameObject pickUpObj = hit.collider.gameObject;
                Rigidbody rb = pickUpObj.GetComponent<Rigidbody>();

                if (rb != null)
                {
                    PickUp(pickUpObj, rb);
                }
            }
        }
    }


    void PickUp(GameObject pickUpObj, Rigidbody rb)
    {
        heldObj = pickUpObj;
        heldObjRb = rb;

        Debug.Log("pickedup: " + heldObj.name);

        originalScale = heldObj.transform.localScale;

        heldObjRb.isKinematic = true;
        heldObj.transform.SetParent(holdPos);
        heldObj.transform.localPosition = Vector3.zero;
        heldObj.transform.localRotation = Quaternion.identity;
        heldObj.transform.localScale = originalScale;
        heldObj.layer = LayerMask.NameToLayer("holdLayer");

        Collider objCol = heldObj.GetComponent<Collider>();
        Collider playerCol = player.GetComponent<Collider>();
        if (objCol != null && playerCol != null)
            Physics.IgnoreCollision(objCol, playerCol, true);

        isHoldingObject = true;

        // ✅ Notify DrinkDelivery if it's a Cup
        if (heldObj.GetComponent<Cup>() != null)
        {
            FindObjectOfType<DrinkDelivery>()?.SetHeldCup(heldObj);
        }
    }



    void DropObject()
    {
        if (heldObj == null || heldObjRb == null) return;

        ResetHeldObjectPhysics();
        heldObj = null;
        heldObjRb = null;
        isHoldingObject = false; // <--- Add this line
    }


    void ThrowObject()
    {
        if (heldObj == null || heldObjRb == null) return;

        ResetHeldObjectPhysics();
        heldObjRb.AddForce(transform.forward * throwForce);
        heldObj = null;
        heldObjRb = null;
        isHoldingObject = false; // <--- Add this line
    }


    void MoveObject()
    {
        if (heldObj != null)
            heldObj.transform.position = holdPos.position;
    }

    void ResetHeldObjectPhysics()
    {
        Collider objCol = heldObj.GetComponent<Collider>();
        Collider playerCol = player.GetComponent<Collider>();

        if (objCol != null && playerCol != null)
            Physics.IgnoreCollision(objCol, playerCol, false);

        heldObj.layer = 0;
        heldObjRb.isKinematic = false;
        heldObj.transform.SetParent(null);
        heldObj.transform.position += Vector3.up * 0.3f;
        heldObj.transform.localScale = originalScale; // Restore original size on drop
    }

    bool IsFruitName(string name)
    {
        return name.Contains("apple") ||
               name.Contains("banana") ||
               name.Contains("kiwi") ||
               name.Contains("strawberry") ||
               name.Contains("pineapple") ||
               name.Contains("lemon") ||
               name.Contains("coconut") ||
               name.Contains("pear") ||
               name.Contains("watermelon");
    }
    public void DropHeldObject()
    {
        DropObject();
    }
}