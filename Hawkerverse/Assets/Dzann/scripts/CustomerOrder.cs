using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerOrder : MonoBehaviour
{
    public List<string> requestedFruits = new List<string>();
    public Color desiredDrinkColor;
    public bool IsWaitingForDrink = true;
    public bool HasOrdered = false; // ✅ Now exists

    void Start()
    {
        // Don't generate order on Start anymore — wait for player interaction
    }

    public void StartOrder()
    {
        if (HasOrdered) return;

        GenerateRandomOrder(); // generates the drink request
        HasOrdered = true;
        IsWaitingForDrink = true; // tells system customer is ready to receive drink
        Debug.Log($"{gameObject.name} ordered: {string.Join(" + ", requestedFruits)} -> {desiredDrinkColor}");
    }


    void GenerateRandomOrder()
    {
        string[] fruitPool = { "banana", "kiwi", "strawberry", "apple", "watermelon", "lemon", "coconut", "pear" };
        requestedFruits.Clear();

        while (requestedFruits.Count < 2)
        {
            string fruit = fruitPool[Random.Range(0, fruitPool.Length)];
            if (!requestedFruits.Contains(fruit))
                requestedFruits.Add(fruit);
        }

        desiredDrinkColor = JuiceColorManager.GetBlendedColor(requestedFruits);
    }

    public bool TryReceiveDrink(GameObject heldCup)
    {
        Cup cupScript = heldCup.GetComponent<Cup>();
        if (cupScript == null)
        {
            Debug.Log("No Cup script on heldCup");
            return false;
        }

        if (!IsWaitingForDrink)
        {
            Debug.Log("Customer is not waiting for a drink");
            return false;
        }

        Color givenColor = cupScript.GetSmoothieColor();
        Debug.Log($"Given color: {givenColor}, Expected: {desiredDrinkColor}");

        if (CheckDrink(givenColor))
        {
            ReceiveDrink();
            return true;
        }

        Debug.Log("Wrong color");
        return false;
    }

    public void ReceiveDrink()
    {
        IsWaitingForDrink = false;
        Debug.Log("Customer received correct drink!");
    }

    public bool CheckDrink(Color deliveredColor)
    {
        float tolerance = 0.1f;
        return Mathf.Abs(desiredDrinkColor.r - deliveredColor.r) < tolerance &&
               Mathf.Abs(desiredDrinkColor.g - deliveredColor.g) < tolerance &&
               Mathf.Abs(desiredDrinkColor.b - deliveredColor.b) < tolerance;
    }

    public string GetDrinkName()
    {
        return string.Join(" + ", requestedFruits);
    }
}
