using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cup : MonoBehaviour
{
    public Renderer liquidRenderer;
    public Color smoothieColor;
    public bool isFilled = false;

    public void SetColor(Color color)
    {
        if (liquidRenderer != null)
        {
            liquidRenderer.material.color = color;
            smoothieColor = color; // store it here
            isFilled = true;
        }
    }

    public void Empty()
    {
        if (liquidRenderer != null)
        {
            liquidRenderer.material.color = Color.clear;
            isFilled = false;
        }
    }
}

