using System;
using UnityEngine;

/// <summary>
///     hello whoever is reading this
///     use this as a starter to how to use the Backend class
///     you should keep the backend in a singleton GameManager script,
///     because the backend should be initialised once
/// </summary>
internal class ConnectionDemo : MonoBehaviour
{
    private AnalyticsCollectionPlayground _analyticsCollector;

    private Backend _backend;

    private void Start()
    {
        // --- just copy this wholesale lol ---
        _backend = new Backend();
        _backend.Init(status =>
        {
            Debug.Log("initialised backend");
            Debug.Log(
                status switch
                {
                    Backend.FirebaseConnectionStatus.Connected => "Status: Connected",
                    Backend.FirebaseConnectionStatus.Updating => "Status: Updating... (Retrying in a bit!)",
                    Backend.FirebaseConnectionStatus.NotConnected => "Status: Disconnected",
                    Backend.FirebaseConnectionStatus.UpdateRequired =>
                        "Status: Disconnected (Device Component Update Required)",
                    Backend.FirebaseConnectionStatus.ExternalError => "Status: Disconnected (External/Device Error)",
                    Backend.FirebaseConnectionStatus.InternalError => "Status: Disconnected (Internal Error)",
                    _ => "Status: Disconnected (unknown fcs state, this is unreachable and a bug)"
                }
            );

            if (status == Backend.FirebaseConnectionStatus.Connected) return;
        });

        _backend.RegisterOnConnectionStatusChangedCallback(status =>
        {
            Debug.Log("this is the sign in callback, this code is called when the connection status changes");
            Debug.Log($"the current game-to-firebase connection status is: {status}");
        });

        // register a callback to refresh the ui when the player signs in.
        _backend.RegisterOnSignInCallback(user =>
        {
            Debug.Log("this is the sign in callback, this code is called when the player is signed in");
            Debug.Log($"the current user is: {user}");
        });

        // --- cut here ---

        // then, somewhere else in UI logic or something, show a text box for the user to enter their name
        // for the demo we'll use the inspector serialized field
        _backend.SetName(demoUserName);

        // and then set the current activity name
        // (the AnalyticsCollector, or whatever is closest called to that, should be attached to the same GameObject)
        _analyticsCollector = GetComponent<AnalyticsCollectionPlayground>();
        if (_analyticsCollector)
        {
            _analyticsCollector.CurrentActivityName = demoActivityName;
            Debug.Log($"Set current activity name to: {_analyticsCollector.CurrentActivityName}");
        }
        else
        {
            Debug.LogError("analytics collector component was not found on this GameObject!");
            throw new Exception("analytics collector component was not found on this GameObject!");
        }

        // after this, we should be a-okay to start collecting analytics and send them to the backend
        _analyticsCollector.StartCollection();

        InvokeRepeating(nameof(LogAnalytics), 1f, 1f);
        // kill this either with _analyticsCollector.StopCollection() or with CancelInvoke(nameof(LogAnalytics));
        // https://docs.unity3d.com/ScriptReference/MonoBehaviour.CancelInvoke.html
    }

    private void OnDestroy()
    {
        // when the game object is destroyed, we should stop collecting analytics
        _analyticsCollector?.StopCollection();

        // and then we can safely dispose of the backend
        _backend?.Deinit();
    }

    private void LogAnalytics()
    {
        // var currentStats = _allActivityStats[CurrentActivityName];
        var (getResult, currentStats) = _analyticsCollector.TryGetActivityStatistics(demoActivityName);
        if (!getResult)
        {
            Debug.LogError($"Failed to retrieve activity statistics for {demoActivityName}");
            return;
        }

        Debug.Log(
            $"\nTAT={currentStats.totalActionsTaken} RIT={currentStats.runningTimeIdle} RAT={currentStats.runningTimeActive}");

        _backend.UpdateUserActivityStatistics(
            demoActivityName,
            currentStats,
            setResult =>
            {
                if (setResult == Backend.TransactionResult.Ok)
                    Debug.Log($"Successfully updated activity statistics for {demoActivityName}");
                else
                    Debug.LogError($"Failed to update activity statistics for {demoActivityName}: {setResult}");
            }
        );
    }
#if UNITY_EDITOR
    [Header("Connection Demo")]
    [Space(10)]
    [HelpBox("Current Activity Name", HelpBoxMessageType.Warning)]
    [Space(10)]
    [SerializeField]
    private string demoActivityName = "ConnectionDemo";

    [HelpBox("User Name", HelpBoxMessageType.Warning)] [Space(10)] [SerializeField]
    private string demoUserName = "Mikkonen";
#endif
}