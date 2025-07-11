using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleSlot : MonoBehaviour
{
    public string expectedType; // e.g., "email", "profile", etc.
    public PuzzlePiece placedPiece;

    public bool IsCorrect()
    {
        if (placedPiece == null)
        {
            Debug.Log($"Slot '{expectedType}' is empty.");
            return false;
        }

        bool match = placedPiece.itemType == expectedType;

        Debug.Log($"Slot '{expectedType}' has piece '{placedPiece.itemType}' → Match: {match}");
        return match;
    }
}
