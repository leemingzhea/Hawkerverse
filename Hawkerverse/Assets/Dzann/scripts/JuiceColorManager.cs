using System.Collections;
using System.Collections.Generic;
using System.Linq; // ✅ Needed for .Select
using UnityEngine;

public static class JuiceColorManager
{
    public static Color GetBlendedColor(List<string> fruits)
    {
        Debug.Log("Fruits passed in: " + string.Join(", ", fruits));
        fruits = fruits.Select(f => f.ToLower().Trim()).ToList();

        bool hasApple = fruits.Contains("apple");
        bool hasBanana = fruits.Contains("banana");
        bool hasKiwi = fruits.Contains("kiwi");
        bool hasStrawberry = fruits.Contains("strawberry");
        bool hasPineapple = fruits.Contains("pineapple");
        bool hasLemon = fruits.Contains("lemon");
        bool hasCoconut = fruits.Contains("coconut");
        bool hasPear = fruits.Contains("pear");
        bool hasWatermelon = fruits.Contains("watermelon");

        int fruitCount = fruits.Count;

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

        if (fruitCount >= 3)
        {
            return new Color(0.6f, 0.4f, 0.4f);
        }

        return Color.gray; // default fallback
    }
}
