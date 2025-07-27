using UnityEngine;

public class Cup : MonoBehaviour
{
    public Renderer liquidRenderer; // Assign in inspector
    private Color currentColor;
    private bool hasSmoothie = false;

    void Start()
    {
        if (liquidRenderer != null)
            liquidRenderer.material = new Material(liquidRenderer.material); // Ensure unique instance

        if (liquidRenderer != null)
            liquidRenderer.material.color = Color.clear;
    }

    public void SetSmoothie(Color color)
    {
        currentColor = color;
        hasSmoothie = true;

        if (liquidRenderer != null)
            liquidRenderer.material.color = currentColor;
    }

    public Color GetSmoothieColor()
    {
        return hasSmoothie ? currentColor : Color.clear;
    }

    public bool HasSmoothie()
    {
        return hasSmoothie;
    }

    public void EmptyCup()
    {
        hasSmoothie = false;
        currentColor = Color.clear;

        if (liquidRenderer != null)
            liquidRenderer.material.color = Color.clear;
    }
}
