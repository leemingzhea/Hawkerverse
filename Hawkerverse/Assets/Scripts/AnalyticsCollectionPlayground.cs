using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AnalyticsCollectionPlayground : MonoBehaviour
{
    /// <summary>
    ///   dictionary of activity statistics for each activity
    /// </summary>
    private Dictionary<string, ActivityStatistics> _allActivityStats;

    /// <summary>
    ///     a private field to hold the name of the current activity
    ///     for use as a dictionary key when accessing the current activity
    ///     statistics data struct
    /// </summary>
    private string _currentActivityName;

    /// <summary>
    ///     a public property to set the activity name, and if the activity
    ///     did not previously exist in the dictionary, create a new
    ///     ActivityStatistics entry for it
    /// </summary>
    public string CurrentActivityName
    {
        get => _currentActivityName;

        set
        {
            _allActivityStats ??= new Dictionary<string, ActivityStatistics>();

            // make a new entry in the dictionary
            if (!_allActivityStats.ContainsKey(value))
            {
                Debug.Log($"current activity name set to new value: '{value}', new entry made");
                
                _allActivityStats[value] = new ActivityStatistics
                {
                    runningTimeIdle = 0f,
                    runningTimeActive = 0f,
                    totalActionsTaken = 0,
                    startedAtEpoch = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
                };
            }
            else
            {
                Debug.Log($"current activity name made set to existing value: '{value}'");
            }
        }
    }

    private void Start()
    {
        CurrentActivityName = "_";
    }
    
    private void Update()
    {
        if (Keyboard.current.anyKey.isPressed)
        {
            Debug.Log("playground key press");
        }
        
        if (Mouse.current.leftButton.isPressed)
        {
            Debug.Log("playground mouse click");
        }

        if (Mouse.current.leftButton.isPressed)
        {
            Debug.Log("playground mouse actuated");
        }

        if (Mouse.current.delta.ReadValue().sqrMagnitude > 0)
        {
            Debug.Log("playground mouse movement");
        }
    }
}

/// <summary>
///     data structure to hold player activity statistics
///     for the hr dashboard
///
///     deliberately simple/generic because this is reused
///     per each game portion (referred to as an activity,
///     chapter, module, etc.)
/// </summary>
[Serializable]
public struct ActivityStatistics
{
    /// <summary>
    ///     total seconds where the player is idle
    /// </summary>
    public float runningTimeIdle;
    
    /// <summary>
    ///     total seconds where the player is actively providing input
    /// </summary>
    public float runningTimeActive;

    /// <summary>
    ///     total actions (key press, mouse click, etc.) taken by the player
    /// </summary>
    public int totalActionsTaken;
    
    /// <summary>
    ///     date time when the player started the activity
    /// </summary>
    public long startedAtEpoch;
}
