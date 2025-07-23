using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerOrder : MonoBehaviour
{
    public List<string> requestedFruits = new List<string>();
    public Color desiredDrinkColor;
    public bool IsWaitingForDrink = true;

    void Start()
    {
        GenerateRandomOrder();
    }

    void GenerateRandomOrder()
    {
        // Example: pick 2 random fruits
        string[] fruitPool = { "banana", "kiwi", "strawberry", "apple", "watermelon", "pineapple", "lemon", "coconut", "pear" };
        requestedFruits.Clear();

        while (requestedFruits.Count < 2)
        {
            string fruit = fruitPool[Random.Range(0, fruitPool.Length)];
            if (!requestedFruits.Contains(fruit))
                requestedFruits.Add(fruit);
        }

        desiredDrinkColor = JuiceColorManager.GetBlendedColor(requestedFruits);
        Debug.Log($"{gameObject.name} ordered: {string.Join(" + ", requestedFruits)} -> {desiredDrinkColor}");
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

        public void Interact(GameObject heldObject)
    {
        if (!IsWaitingForDrink) return;

        Cup cup = heldObject.GetComponent<Cup>();
        if (cup != null && cup.isFilled)
        {
            if (CheckDrink(cup.smoothieColor))
            {
                ReceiveDrink();
                cup.Empty(); // empty cup after delivery
                Debug.Log("Correct drink delivered!");
            }
            else
            {
                Debug.Log("Wrong drink! Try again.");
            }
        }
        else
        {
            Debug.Log("You are not holding a drink!");
        }
    }

}