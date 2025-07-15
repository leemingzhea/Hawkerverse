using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cup : MonoBehaviour
{
    public Renderer liquidRenderer;

    public void SetColor(Color color)
    {
        if (liquidRenderer != null)
        {
            liquidRenderer.material.color = color;
        }
    }
}
