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

        cup.SetColor(currentSmoothieColor);
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
        bool hasApple = fruitsInBlender.Contains("apple");
        bool hasBanana = fruitsInBlender.Contains("banana");
        bool hasKiwi = fruitsInBlender.Contains("kiwi");
        bool hasStrawberry = fruitsInBlender.Contains("strawberry");
        bool hasPineapple = fruitsInBlender.Contains("pineapple");
        bool hasLemon = fruitsInBlender.Contains("lemon");
        bool hasCoconut = fruitsInBlender.Contains("coconut");
        bool hasPear = fruitsInBlender.Contains("pear");
        bool hasWatermelon = fruitsInBlender.Contains("watermelon");

        int fruitCount = fruitsInBlender.Count;

        // Single fruit colors
        if (fruitCount == 1)
        {
            if (hasApple) return new Color(1f, 0.3f, 0.3f);
            if (hasBanana) return new Color(1f, 1f, 0.4f);
            if (hasKiwi) return new Color(0.6f, 1f, 0.4f);
            if (hasStrawberry) return new Color(1f, 0.2f, 0.4f);
            if (hasPineapple) return new Color(1f, 0.95f, 0.3f);
            if (hasLemon) return new Color(1f, 1f, 0.6f);
            if (hasCoconut) return new Color(1f, 1f, 0.9f);
            if (hasPear) return new Color(0.8f, 1f, 0.6f);
            if (hasWatermelon) return new Color(1f, 0.4f, 0.5f);
        }

        // Two fruit combinations (as before)
        if (fruitCount == 2)
        {
            if (hasApple && hasBanana) return new Color(1f, 1f, 0.3f);
            if (hasApple && hasKiwi) return new Color(0.8f, 0.6f, 0.4f);
            if (hasApple && hasStrawberry) return new Color(1f, 0.2f, 0.3f);
            if (hasApple && hasPineapple) return new Color(1f, 0.8f, 0.3f);
            if (hasApple && hasLemon) return new Color(1f, 0.9f, 0.4f);
            if (hasApple && hasCoconut) return new Color(1f, 0.8f, 0.7f);
            if (hasApple && hasPear) return new Color(0.9f, 0.8f, 0.5f);
            if (hasApple && hasWatermelon) return new Color(1f, 0.3f, 0.3f);

            if (hasBanana && hasKiwi) return new Color(0.8f, 1f, 0.3f);
            if (hasBanana && hasStrawberry) return new Color(1f, 0.7f, 0.5f);
            if (hasBanana && hasPineapple) return new Color(1f, 1f, 0.3f);
            if (hasBanana && hasLemon) return new Color(1f, 1f, 0.6f);
            if (hasBanana && hasCoconut) return new Color(1f, 1f, 0.8f);
            if (hasBanana && hasPear) return new Color(0.9f, 1f, 0.6f);
            if (hasBanana && hasWatermelon) return new Color(1f, 0.6f, 0.2f);

            if (hasKiwi && hasStrawberry) return new Color(1f, 0.6f, 0.5f);
            if (hasKiwi && hasPineapple) return new Color(0.8f, 1f, 0.3f);
            if (hasKiwi && hasLemon) return new Color(0.7f, 1f, 0.4f);
            if (hasKiwi && hasCoconut) return new Color(0.8f, 1f, 0.8f);
            if (hasKiwi && hasPear) return new Color(0.5f, 1f, 0.3f);
            if (hasKiwi && hasWatermelon) return new Color(1f, 0.4f, 0.5f);

            if (hasStrawberry && hasPineapple) return new Color(1f, 0.7f, 0.5f);
            if (hasStrawberry && hasLemon) return new Color(1f, 0.7f, 0.6f);
            if (hasStrawberry && hasCoconut) return new Color(1f, 0.8f, 0.7f);
            if (hasStrawberry && hasPear) return new Color(1f, 0.5f, 0.6f);
            if (hasStrawberry && hasWatermelon) return new Color(1f, 0.3f, 0.3f);

            if (hasPineapple && hasLemon) return new Color(1f, 1f, 0.5f);
            if (hasPineapple && hasCoconut) return new Color(1f, 1f, 0.8f);
            if (hasPineapple && hasPear) return new Color(0.9f, 1f, 0.6f);
            if (hasPineapple && hasWatermelon) return new Color(1f, 0.6f, 0.3f);

            if (hasLemon && hasCoconut) return new Color(1f, 1f, 0.8f);
            if (hasLemon && hasPear) return new Color(0.9f, 1f, 0.6f);
            if (hasLemon && hasWatermelon) return new Color(1f, 0.7f, 0.6f);

            if (hasCoconut && hasPear) return new Color(0.9f, 1f, 0.8f);
            if (hasCoconut && hasWatermelon) return new Color(1f, 0.7f, 0.7f);

            if (hasPear && hasWatermelon) return new Color(1f, 0.5f, 0.5f);
        }

        // 3 or more fruits default color
        if (fruitCount >= 3)
        {
            return new Color(0.6f, 0.4f, 0.4f); // Brownish
        }

        // Default fallback color
        return Color.gray;
    }
}
