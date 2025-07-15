using System.Collections.Generic;
using UnityEngine;

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
    //public GameObject pineappleInsidePrefab;
    public GameObject lemonInsidePrefab;
    public GameObject coconutInsidePrefab;
    public GameObject pearInsidePrefab;
    public GameObject watermelonInsidePrefab;

    private List<string> fruitsInBlender = new List<string>();

    // Adds fruit to blender and spawns visual
    public void AddFruit(string fruitType)
    {
        Debug.Log($"AddFruit called with fruitType: '{fruitType}'");

        fruitsInBlender.Add(fruitType);

        GameObject visualPrefab = GetFruitPrefab(fruitType);
        if (visualPrefab != null && blenderContentsParent != null)
        {
            Debug.Log($"Instantiating visual prefab for '{fruitType}'");
            GameObject visual = Instantiate(visualPrefab, blenderContentsParent);
            visual.transform.localPosition = Vector3.zero; // Snap to center
        }
        else
        {
            Debug.Log($"Fruit prefab for '{fruitType}' is null or blenderContentsParent is missing.");
        }
    }

    // Triggers blending and spawns liquid
    public void Blend()
    {
        Debug.Log("Blend() was triggered!");
        // Destroy fruit visuals
        foreach (Transform child in blenderContentsParent)
        {
            Destroy(child.gameObject);
        }

        // Destroy old liquid
        if (currentLiquid != null)
        {
            Destroy(currentLiquid);
        }

        // Determine blended color
        Color blendedColor = DetermineSmoothieColor();

        // Instantiate new liquid
        if (liquidPrefab != null && liquidSpawnPoint != null)
        {
            currentLiquid = Instantiate(liquidPrefab, liquidSpawnPoint.position, Quaternion.identity, liquidSpawnPoint);

            Renderer rend = currentLiquid.GetComponent<Renderer>();
            if (rend != null)
            {
                rend.material.color = blendedColor;
            }
        }
        else
        {
            Debug.Log("Missing liquidPrefab or liquidSpawnPoint.");
        }

        fruitsInBlender.Clear();
    }

    // Helper: Get visual prefab for fruit
    private GameObject GetFruitPrefab(string fruitType)
    {
        switch (fruitType.ToLower())
        {
            case "apple": return appleInsidePrefab;
            case "banana": return bananaInsidePrefab;
            case "kiwi": return kiwiInsidePrefab;
            case "strawberry": return strawberryInsidePrefab;
            //case "pineapple": return pineappleInsidePrefab;
            case "lemon": return lemonInsidePrefab;
            case "coconut": return coconutInsidePrefab;
            case "pear": return pearInsidePrefab;
            case "watermelon": return watermelonInsidePrefab;
            default:
                Debug.Log($"No prefab found for fruit type '{fruitType}'");
                return null;
        }
    }

    // Color mixing logic
    private Color DetermineSmoothieColor()
    {
        bool hasRed = fruitsInBlender.Contains("apple") || fruitsInBlender.Contains("strawberry") || fruitsInBlender.Contains("watermelon");
        bool hasYellow = fruitsInBlender.Contains("banana") || fruitsInBlender.Contains("lemon");
        bool hasGreen = fruitsInBlender.Contains("kiwi") || fruitsInBlender.Contains("pear");

        if (hasRed && hasYellow && hasGreen)
            return new Color(0.6f, 0.4f, 0.4f); // brownish
        else if (hasRed && hasYellow)
            return new Color(1f, 0.5f, 0.2f); // orange
        else if (hasRed)
            return Color.red;
        else if (hasYellow)
            return Color.yellow;
        else if (hasGreen)
            return Color.green;
        else
            return Color.gray; // default
    }
}
