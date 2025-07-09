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

    private GameObject heldObj;
    private Rigidbody heldObjRb;
    private int LayerNumber;
    private Blender targetedBlender;
    private Vector3 originalScale;

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
            if (heldObj != null && targetedBlender != null)
            {
                heldObj.transform.SetParent(null); // unparent from hand
                targetedBlender.AddHeldFruitByName(heldObj); // blender destroys it
                heldObj = null;
                heldObjRb = null;
            }
            else if (heldObj == null)
            {
                TryPickUp();
            }
            else
            {
                DropObject();
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

            if (IsFruitName(objName))
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

            if (IsFruitName(objName))
            {
                GameObject pickUpObj = hit.collider.gameObject;
                Rigidbody rb = pickUpObj.GetComponent<Rigidbody>();

                if (rb != null)
                {
                    heldObj = pickUpObj;
                    heldObjRb = rb;
                    originalScale = heldObj.transform.localScale;

                    heldObjRb.isKinematic = true;
                    heldObj.transform.SetParent(holdPos);
                    heldObj.transform.localPosition = Vector3.zero;
                    heldObj.transform.localRotation = Quaternion.identity;
                    heldObj.transform.localScale = originalScale;
                    heldObj.layer = LayerNumber;

                    Collider objCol = heldObj.GetComponent<Collider>();
                    Collider playerCol = player.GetComponent<Collider>();
                    if (objCol != null && playerCol != null)
                        Physics.IgnoreCollision(objCol, playerCol, true);
                }
            }
        }
    }

    void DropObject()
    {
        if (heldObj == null || heldObjRb == null) return;

        ResetHeldObjectPhysics();
        heldObj = null;
        heldObjRb = null;
    }

    void ThrowObject()
    {
        if (heldObj == null || heldObjRb == null) return;

        ResetHeldObjectPhysics();
        heldObjRb.AddForce(transform.forward * throwForce);
        heldObj = null;
        heldObjRb = null;
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
        heldObj.transform.position += Vector3.up * 0.3f; // prevent clipping into floor
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
}
