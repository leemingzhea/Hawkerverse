using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Blender : MonoBehaviour
{
    [Header("Blender Visuals")]
    public Transform blenderContentsParent; // Parent for visual fruit pieces

    [Header("Liquid Settings")]
    public GameObject liquidPrefab; // Prefab of the smoothie/liquid
    public Transform liquidSpawnPoint; // Where the liquid should appear
    private GameObject currentLiquid;

    [Header("Fruit Inside Prefabs")]
    public GameObject appleInsidePrefab;
    public GameObject bananaInsidePrefab;
    public GameObject kiwiInsidePrefab;
    public GameObject strawberryInsidePrefab;
    public GameObject lemonInsidePrefab;
    public GameObject coconutInsidePrefab;
    public GameObject pearInsidePrefab;
    public GameObject watermelonInsidePrefab;

    private List<string> fruitsInBlender = new List<string>();

    private bool isBlended = false;
    public bool IsBlended => isBlended;

    private Color currentSmoothieColor = Color.gray;
    public Color GetSmoothieColor() => currentSmoothieColor;

    // Adds fruit to blender and spawns visual
    public void AddFruit(string fruitType)
    {
        if (isBlended)
        {
            Debug.Log("Blender already contains a smoothie. Please clear it first.");
            return;
        }

        Debug.Log($"AddFruit called with fruitType: '{fruitType}'");

        fruitsInBlender.Add(fruitType);

        GameObject visualPrefab = GetFruitPrefab(fruitType);
        if (visualPrefab != null && blenderContentsParent != null)
        {
            Debug.Log($"Instantiating visual prefab for '{fruitType}'");
            GameObject visual = Instantiate(visualPrefab, blenderContentsParent);
            visual.transform.localPosition = Vector3.zero;

            Rigidbody rb = visual.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
            }
        }
        else
        {
            Debug.Log($"Fruit prefab for '{fruitType}' is null or blenderContentsParent is missing.");
        }
    }

    // Triggers blending and spawns liquid
    public void Blend()
    {
        if (fruitsInBlender.Count == 0)
        {
            Debug.Log("Cannot blend an empty blender.");
            return;
        }

        Debug.Log("Blend() was triggered!");

        foreach (Transform child in blenderContentsParent)
        {
            Destroy(child.gameObject);
        }

        if (currentLiquid != null)
        {
            Destroy(currentLiquid);
        }

        currentSmoothieColor = DetermineSmoothieColor();

        if (liquidPrefab != null && liquidSpawnPoint != null)
        {
            currentLiquid = Instantiate(liquidPrefab, liquidSpawnPoint.position, Quaternion.identity, liquidSpawnPoint);

            Renderer rend = currentLiquid.GetComponent<Renderer>();
            if (rend != null)
            {
                rend.material.color = currentSmoothieColor;
            }
        }
        else
        {
            Debug.Log("Missing liquidPrefab or liquidSpawnPoint.");
        }

        fruitsInBlender.Clear();
        isBlended = true;
    }

    // Clears the blender (e.g. when transferring or discarding)
    public void ClearBlender()
    {
        Debug.Log("Clearing blender contents.");
        foreach (Transform child in blenderContentsParent)
        {
            Destroy(child.gameObject);
        }

        if (currentLiquid != null)
        {
            Destroy(currentLiquid);
            currentLiquid = null;  // <--- important to clear reference
        }

        fruitsInBlender.Clear();
        isBlended = false;
        currentSmoothieColor = Color.gray;
    }

        public bool TransferToCup(Cup cup)
    {
        if (!isBlended)
        {
            Debug.Log("No smoothie to transfer.");
            return false;
        }

        if (cup == null)
        {
            Debug.Log("No cup detected.");
            return false;
        }

        cup.SetSmoothie(currentSmoothieColor); 
        ClearBlender();
        Debug.Log("Smoothie transferred to cup.");
        return true;
    }


    // Get correct prefab based on fruit type
    private GameObject GetFruitPrefab(string fruitType)
    {
        switch (fruitType.ToLower())
        {
            case "apple": return appleInsidePrefab;
            case "banana": return bananaInsidePrefab;
            case "kiwi": return kiwiInsidePrefab;
            case "strawberry": return strawberryInsidePrefab;
            case "lemon": return lemonInsidePrefab;
            case "coconut": return coconutInsidePrefab;
            case "pear": return pearInsidePrefab;
            case "watermelon": return watermelonInsidePrefab;
            default:
                Debug.Log($"No prefab found for fruit type '{fruitType}'");
                return null;
        }
    }

    private Color DetermineSmoothieColor()
    {
        return JuiceColorManager.GetBlendedColor(fruitsInBlender);
    }
}
