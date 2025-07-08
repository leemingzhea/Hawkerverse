using System.Collections.Generic;
using UnityEngine;

public class Blender : MonoBehaviour
{
    [Header("Blender Visuals")]
    public Renderer liquidRenderer;
    public Transform blenderContentsParent;

    [Header("Fruit Visuals Inside Blender")]
    public GameObject appleInsidePrefab;
    public GameObject bananaInsidePrefab;
    public GameObject kiwiInsidePrefab;
    public GameObject strawberryInsidePrefab;
    public GameObject pineappleInsidePrefab;
    public GameObject lemonInsidePrefab;
    public GameObject coconutInsidePrefab;
    public GameObject pearInsidePrefab;
    public GameObject watermelonInsidePrefab;

    private HashSet<string> addedFruits = new HashSet<string>();

    public void AddHeldFruitByName(GameObject heldFruit)
    {
        string objName = heldFruit.name.ToLower();

        if (objName.Contains("apple")) HandleFruit("Apple", appleInsidePrefab, heldFruit);
        else if (objName.Contains("banana")) HandleFruit("Banana", bananaInsidePrefab, heldFruit);
        else if (objName.Contains("kiwi")) HandleFruit("Kiwi", kiwiInsidePrefab, heldFruit);
        else if (objName.Contains("strawberry")) HandleFruit("Strawberry", strawberryInsidePrefab, heldFruit);
        else if (objName.Contains("pineapple")) HandleFruit("Pineapple", pineappleInsidePrefab, heldFruit);
        else if (objName.Contains("lemon")) HandleFruit("Lemon", lemonInsidePrefab, heldFruit);
        else if (objName.Contains("coconut")) HandleFruit("Coconut", coconutInsidePrefab, heldFruit);
        else if (objName.Contains("pear")) HandleFruit("Pear", pearInsidePrefab, heldFruit);
        else if (objName.Contains("watermelon")) HandleFruit("Watermelon", watermelonInsidePrefab, heldFruit);
    }

    private void HandleFruit(string fruitName, GameObject visualPrefab, GameObject heldFruit)
    {
        if (!addedFruits.Contains(fruitName))
        {
            addedFruits.Add(fruitName);

            if (visualPrefab != null && blenderContentsParent != null)
            {
                GameObject visual = Instantiate(visualPrefab, blenderContentsParent);
                visual.transform.localPosition = Vector3.zero;
            }
        }

        Destroy(heldFruit);
    }

    public void BlendFruits()
    {
        if (addedFruits.Count < 2)
        {
            Debug.Log("Not enough fruit to blend!");
            return;
        }

        Color smoothieColor = Color.gray;
        string smoothieName = "Unknown Smoothie";

        if (addedFruits.SetEquals(new HashSet<string> { "Apple", "Banana" }))
        {
            smoothieColor = Color.yellow;
            smoothieName = "Apple Banana Smoothie";
        }
        else if (addedFruits.SetEquals(new HashSet<string> { "Apple", "Watermelon" }))
        {
            smoothieColor = Color.red;
            smoothieName = "Apple Watermelon Smoothie";
        }
        else if (addedFruits.SetEquals(new HashSet<string> { "Banana", "Watermelon" }))
        {
            smoothieColor = new Color(1f, 0.5f, 0f); // Orange
            smoothieName = "Banana Watermelon Smoothie";
        }

        if (liquidRenderer != null)
        {
            liquidRenderer.material.color = smoothieColor;
        }

        Debug.Log($"Blended: {smoothieName}");

        addedFruits.Clear();
        foreach (Transform child in blenderContentsParent)
        {
            Destroy(child.gameObject);
        }
    }
}
