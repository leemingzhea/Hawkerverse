using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
///     A MonoBehaviour for collecting analytics about player activity.
///     It tracks user inputs like key presses, mouse clicks, and mouse movement
///     to distinguish between active and idle time for different activities.
/// </summary>
public class AnalyticsCollectionPlayground : MonoBehaviour
{
    /// <summary>
    ///     The time in seconds of inactivity before the mouse is considered stopped.
    /// </summary>
    private const float MouseMovementThreshold = 1.0f; // Time in seconds before considering mouse stopped

    /// <summary>
    ///     The minimum distance the mouse must move to be considered a movement.
    /// </summary>
    private const float MousePositionThreshold = 1.0f; // Minimum pixel distance to consider as movement

#if UNITY_EDITOR
    [HelpBox(
        "This is the key that is used as the current activity name, " +
        "please change it by setting the CurrentActivityName property programatically!",
        HelpBoxMessageType.Error)]
    [Space(10)]
#endif
    [SerializeField]
    private string activityName = "Default";

    /// <summary>
    ///     A dictionary holding all activity statistics, with activity names as keys.
    /// </summary>
    private Dictionary<string, ActivityStatistics> _allActivityStatistics;

    /// <summary>
    ///     A private field to hold the name of the current activity
    ///     for use as a dictionary key when accessing the current activity
    ///     statistics data struct.
    /// </summary>
    private string _currentActivityName;

    /// <summary>
    ///     Flag indicating if the mouse is currently considered to be moving.
    /// </summary>
    private bool _isCurrentlyMouseMoving;

    /// <summary>
    ///     Flag to control whether analytics collection is paused.
    /// </summary>
    private bool _isCollectionPaused = true;

    /// <summary>
    ///     The time when the mouse was last moved.
    /// </summary>
    private float _lastMouseMoveTime;

    // --- mouse movement tracking ---

    /// <summary>
    ///     Stores the last recorded mouse position to calculate movement delta.
    /// </summary>
    private Vector2 _lastMousePosition;

    /// <summary>
    ///     Tracks if a key was pressed in the last frame to detect new presses.
    /// </summary>
    private bool _wasKeyPressedLastFrame;

    /// <summary>
    ///     Tracks if the mouse button was pressed in the last frame to detect new clicks.
    /// </summary>
    private bool _wasMouseButtonPressedLastFrame;

    /// <summary>
    ///     A public property to set the activity name. When set, if the activity
    ///     did not previously exist in the dictionary, a new
    ///     <see cref="ActivityStatistics" /> entry is created for it.
    /// </summary>
    public string CurrentActivityName
    {
        get => _currentActivityName;

        set
        {
            if (string.IsNullOrEmpty(value))
            {
                Debug.LogError("CurrentActivityName cannot be set to null or empty.");
                return;
            }

            _currentActivityName = value;
            _allActivityStatistics ??= new Dictionary<string, ActivityStatistics>();

            // make a new entry in the dictionary
            if (!_allActivityStatistics.ContainsKey(value))
            {
                Debug.Log($"current activity name set to new value: '{value}', new entry made");

                _allActivityStatistics[value] = new ActivityStatistics
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

    /// <summary>
    ///     Initializes the component by setting the initial activity name.
    /// </summary>
    private void Awake()
    {
        CurrentActivityName = activityName;
    }

    /// <summary>
    ///     Called every frame to update activity and idle times based on player input.
    /// </summary>
    private void Update()
    {
        // If collection is paused, do nothing.
        if (_isCollectionPaused)
        {
            return;
        }

        // Check for current input states.
        var isKeyPressed = Keyboard.current.anyKey.isPressed;
        var isMouseButtonPressed = Mouse.current.leftButton.isPressed;

        // Count individual actions that just started
        var currentActivity = _allActivityStatistics[CurrentActivityName];
        var newActionsThisFrame = 0;

        // Count new key press action
        if (isKeyPressed && !_wasKeyPressedLastFrame) newActionsThisFrame++;

        // Count new mouse button press action
        if (isMouseButtonPressed && !_wasMouseButtonPressedLastFrame) newActionsThisFrame++;

        // Mouse movement detection with time-based threshold
        var currentMousePosition = Mouse.current.position.ReadValue();
        var mouseDelta = currentMousePosition - _lastMousePosition;

        // Check if mouse moved significantly
        if (mouseDelta.magnitude > MousePositionThreshold)
        {
            _lastMouseMoveTime = Time.time;

            // Count as new action only when movement starts (not currently moving)
            if (!_isCurrentlyMouseMoving)
            {
                newActionsThisFrame++;
                _isCurrentlyMouseMoving = true;
            }
        }

        // Check if mouse has stopped moving based on time threshold
        if (Time.time - _lastMouseMoveTime > MouseMovementThreshold) _isCurrentlyMouseMoving = false;

        // Update active or idle time based on whether any input was detected.
        if (isKeyPressed || isMouseButtonPressed || _isCurrentlyMouseMoving)
        {
            currentActivity.runningTimeActive += Time.deltaTime;
            currentActivity.totalActionsTaken += newActionsThisFrame;
        }
        else
        {
            currentActivity.runningTimeIdle += Time.deltaTime;
        }

        // Save the updated statistics and input states for the next frame.
        _allActivityStatistics[CurrentActivityName] = currentActivity;
        _wasKeyPressedLastFrame = isKeyPressed;
        _wasMouseButtonPressedLastFrame = isMouseButtonPressed;
        _lastMousePosition = currentMousePosition;
    }

    /// <summary>
    ///   Starts or resumes the collection of analytics data.
    /// </summary>
    public void StartCollection()
    {
        _isCollectionPaused = false;
        Debug.Log("Analytics collection started.");
    }

    /// <summary>
    ///   Pauses the collection of analytics data.
    /// </summary>
    public void StopCollection()
    {
        _isCollectionPaused = true;
        Debug.Log("Analytics collection stopped.");
    }

    /// <summary>
    ///   Tries to retrieve an <see cref="ActivityStatistics"/> struct from the dictionary.
    /// </summary>
    /// <param name="key">The name of the activity to retrieve.</param>
    /// <returns>A tuple containing a boolean indicating success and the retrieved <see cref="ActivityStatistics" /> struct.</returns>
    public (bool, ActivityStatistics) TryGetActivityStatistics(string key)
    {
        if (_allActivityStatistics != null && _allActivityStatistics.TryGetValue(key, out var stats)) return (true, stats);

        return (false, new ActivityStatistics());
    }

    // reference code for debugging
    // private void FixedUpdate()
    // {
    //     var currentStats = _allActivityStats[CurrentActivityName];
    //     Debug.Log($"\nTAT={currentStats.totalActionsTaken} RIT={currentStats.runningTimeIdle} RAT={currentStats.runningTimeActive}");
    // }
}

/// <summary>
///     data structure to hold player activity statistics
///     for the hr dashboard
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