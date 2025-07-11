using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;
using UnityEngine.Serialization;
using System.Security.Cryptography;
using System.Text;

public class Backend
{
    // --- minimal class implementation from sota staircase projects and ColourMeOK ---
    // https://forge.joshwel.co/mark/colourmeok/src/branch/main/ColourMeOKGame/Assets/Scripts/Backend.cs
    
    /// <summary>
    ///     enum for the result of the authentication process
    /// </summary>
    public enum AuthenticationResult
    {
        Ok,
        AlreadyAuthenticated,
        NonExistentUser,
        AlreadyExistingUser,
        UsernameAlreadyTaken,
        InvalidEmail,
        InvalidCredentials,
        GenericError
    }

    /// <summary>
    ///     enum for the connection status of the firebase back-end
    /// </summary>
    public enum FirebaseConnectionStatus
    {
        NotConnected,
        Connected,

        // "a required system component is out of date"
        UpdateRequired,

        // "a required system component is updating, retrying in a bit..."
        Updating,

        // "a system component is disabled, invalid, missing, or permissions are insufficient"
        ExternalError,

        // "an unknown error occurred"
        InternalError
    }

    /// <summary>
    ///     generic enum for the result of a database transaction
    /// </summary>
    public enum TransactionResult
    {
        Ok,
        Unauthenticated,
        Error
    }

    /// <summary>
    ///     callback functions to be invoked when the connection status changes
    /// </summary>
    /// <returns></returns>
    private readonly List<Action<FirebaseConnectionStatus>> _onConnectionStatusChangedCallbacks = new();

    /// <summary>
    ///     callback functions to be invoked when the user signs in
    /// </summary>
    private readonly List<Action<FirebaseUser>> _onSignInCallbacks = new();

    /// <summary>
    ///     callback functions to be invoked when the user signs out
    /// </summary>
    private readonly List<Action> _onSignOutCallbacks = new();


    /// <summary>
    ///     the firebase authentication object
    /// </summary>
    private FirebaseAuth _auth;

    /// <summary>
    ///     the firebase database reference
    /// </summary>
    private DatabaseReference _db;

    /// <summary>
    ///     the current user object, if authenticated
    /// </summary>
    private FirebaseUser _user;

    // /// <summary>
    // ///     the current user's username, if authenticated
    // /// </summary>
    // private string _username;

    /// <summary>
    ///     whether the user is signed in
    /// </summary>
    [FormerlySerializedAs("IsSignedIn")] public bool isSignedIn;

    /// <summary>
    ///     whether the backend is connected to the firebase backend
    /// </summary>
    [FormerlySerializedAs("Status")] public FirebaseConnectionStatus status = FirebaseConnectionStatus.NotConnected;

    /// <summary>
    ///     variable initialisation function
    /// </summary>
    public void Init(Action<FirebaseConnectionStatus> callback)
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            switch (task.Result)
            {
                case DependencyStatus.Available:
                    _auth = FirebaseAuth.GetAuth(FirebaseApp.DefaultInstance);
                    _auth.StateChanged += AuthStateChanged;
                    _db = FirebaseDatabase.DefaultInstance.RootReference;
                    status = FirebaseConnectionStatus.Connected;
                    callback(status);
                    FireOnConnectionStatusChangedCallbacks();
                    break;

                case DependencyStatus.UnavailableDisabled:
                case DependencyStatus.UnavailableInvalid:
                case DependencyStatus.UnavilableMissing:
                case DependencyStatus.UnavailablePermission:
                    status = FirebaseConnectionStatus.ExternalError;
                    callback(status);
                    FireOnConnectionStatusChangedCallbacks();
                    break;

                case DependencyStatus.UnavailableUpdating:
                    status = FirebaseConnectionStatus.Updating;
                    callback(status);
                    FireOnConnectionStatusChangedCallbacks();
                    RetryInitialiseAfterDelay(callback);
                    break;

                case DependencyStatus.UnavailableUpdaterequired:
                    status = FirebaseConnectionStatus.UpdateRequired;
                    FireOnConnectionStatusChangedCallbacks();
                    callback(status);
                    break;

                case DependencyStatus.UnavailableOther:
                default:
                    status = FirebaseConnectionStatus.InternalError;
                    Debug.LogError("firebase ??? blew up or something," + task.Result);
                    FireOnConnectionStatusChangedCallbacks();
                    callback(status);
                    break;
            }

            Debug.Log("firebase status is" + status);
        });
    }

    /// <summary>
    ///     async function to retry initialisation after a delay
    /// </summary>
    private async void RetryInitialiseAfterDelay(Action<FirebaseConnectionStatus> callback)
    {
        try
        {
            await Task.Delay(TimeSpan.FromSeconds(10));
            Init(callback);
        }
        catch (Exception e)
        {
            Debug.LogError("RetryInitialiseAfterDelay");
            Debug.LogException(e);
        }
    }

    /// <summary>
    ///     cleanup function
    /// </summary>
    public void Deinit()
    {
        SignOutUser();
        _auth.StateChanged -= AuthStateChanged;
        _auth = null;
    }

    /// <summary>
    ///     function to handle the authentication state change event
    /// </summary>
    /// <param name="sender">the object that triggered the event</param>
    /// <param name="eventArgs">the event arguments</param>
    private void AuthStateChanged(object sender, EventArgs eventArgs)
    {
        // if the user hasn't changed, do nothing
        if (_auth.CurrentUser == _user) return;

        // if the user has changed, check if they've signed in or out
        isSignedIn = _user != _auth.CurrentUser && _auth.CurrentUser != null;

        // if we're not signed in, but we still hold _user locally, we've signed out
        if (!isSignedIn && _user != null) Debug.Log("moi-moi");

        // they have signed in, update _user
        _user = _auth.CurrentUser;
        if (!isSignedIn) return;

        Debug.Log($"signed in successfully as {_user?.UserId}");
    }

    /// <summary>
    ///     function to register a callback for when the connection status changes
    /// </summary>
    /// <param name="callback">callback function that takes in a <c>FirebaseConnectionStatus</c> enum</param>
    public void RegisterOnConnectionStatusChangedCallback(Action<FirebaseConnectionStatus> callback)
    {
        _onConnectionStatusChangedCallbacks.Add(callback);
        Debug.Log($"registering ConnectionStatusChangedCallback ({_onConnectionStatusChangedCallbacks.Count})");
    }

    /// <summary>
    ///     function to register a callback for when the user signs in
    /// </summary>
    /// <param name="callback">callback function that takes in a <c>FirebaseUser</c> object</param>
    public void RegisterOnSignInCallback(Action<FirebaseUser> callback)
    {
        _onSignInCallbacks.Add(callback);
        Debug.Log($"registering OnSignInCallback ({_onSignInCallbacks.Count})");
    }

    /// <summary>
    ///     function to register a callback for when the user signs out
    /// </summary>
    /// <param name="callback">callback function</param>
    public void RegisterOnSignOutCallback(Action callback)
    {
        _onSignOutCallbacks.Add(callback);
        Debug.Log($"registering OnSignOutCallback ({_onSignOutCallbacks.Count})");
    }


    /// <summary>
    ///     function to fire all on connection status changed callbacks
    /// </summary>
    private void FireOnConnectionStatusChangedCallbacks()
    {
        Debug.Log($"firing OnConnectionStatusChangedCallbacks ({_onConnectionStatusChangedCallbacks.Count})");
        foreach (var callback in _onConnectionStatusChangedCallbacks)
            try
            {
                callback.Invoke(status);
            }
            catch (Exception e)
            {
                Debug.LogError($"error invoking OnConnectionStatusChangedCallback: {e.Message}");
            }
    }

    /// <summary>
    ///     function to fire all on sign in callbacks
    /// </summary>
    private void FireOnSignInCallbacks()
    {
        Debug.Log($"firing OnSignInCallbacks ({_onSignInCallbacks.Count})");
        foreach (var callback in _onSignInCallbacks)
            try
            {
                callback.Invoke(_user);
            }
            catch (Exception e)
            {
                Debug.LogError($"error invoking OnSignInCallback: {e.Message}");
            }
    }

    /// <summary>
    ///     function to fire all on sign-out callbacks
    /// </summary>
    private void FireOnSignOutCallbacks()
    {
        Debug.Log($"firing OnSignOutCallbacks ({_onSignOutCallbacks.Count})");
        foreach (var callback in _onSignOutCallbacks)
            try
            {
                callback.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogError($"error invoking OnSignOutCallback: {e.Message}");
            }
    }

    /// <summary>
    ///     abstraction function to retrieve the user
    /// </summary>
    /// <returns>the firebase user object</returns>
    public FirebaseUser GetUser()
    {
        return _user;
    }

    // public string GetUsername()
    // {
    //     return _username;
    // }

    /// <summary>
    ///     abstraction function to sign out the user
    /// </summary>
    public void SignOutUser()
    {
        _auth.SignOut();
    }
    
    // --- game-specific functions below this line ---
    
    // FOR CALLBACK-BASED DATABASE SETTER FUNCTION REFERENCE:
    // /// <summary>
    // ///     abstraction function to update the user's rating in the database
    // /// </summary>
    // /// <param name="callback">callback function that takes in a <c>TransactionResult</c> enum </param>
    // public void UpdateUserRating(
    //     Action<TransactionResult> callback)
    // {
    //     if (!Status.Equals(FirebaseConnectionStatus.Connected)) return;
    //
    //     if (_user == null)
    //     {
    //         callback(TransactionResult.Unauthenticated);
    //         return;
    //     }
    //
    //     var userRating = GameManager.Instance.Data.CalculateUserRating();
    //
    //     _db.Child("users")
    //         .Child(_user.UserId)
    //         .Child("rating")
    //         .SetValueAsync(userRating)
    //         .ContinueWithOnMainThread(task =>
    //         {
    //             if (task.IsCompletedSuccessfully)
    //             {
    //                 Debug.Log($"updated online user rating to {userRating}");
    //                 callback(TransactionResult.Ok);
    //             }
    //             else
    //             {
    //                 Debug.LogError(task.Exception);
    //                 callback(TransactionResult.Error);
    //             }
    //         });
    // }
    //
    // FOR CALLBACK-BASED DATABASE GETTER FUNCTION REFERENCE:
    // /// <summary>
    // ///     abstraction function to get the leaderboard from the database
    // /// </summary>
    // /// <param name="callback">
    // ///     callback function that takes in a <c>TransactionResult</c> enum and a <c>List&lt;LeaderboardEntry&gt;</c>
    // /// </param>
    // public void GetLeaderboard(
    //     Action<TransactionResult, List<LeaderboardEntry>> callback)
    // {
    //     Debug.Log("getting leaderboard");
    //
    //     _db.Child("users")
    //         .OrderByChild("rating")
    //         .LimitToLast(LeaderboardUI.MaxEntries)
    //         .GetValueAsync()
    //         .ContinueWithOnMainThread(task =>
    //         {
    //             if (!task.IsCompletedSuccessfully)
    //             {
    //                 Debug.LogError(task.Exception);
    //                 callback(TransactionResult.Error, new List<LeaderboardEntry>(0));
    //                 return;
    //             }
    //
    //             var entries = new List<LeaderboardEntry>();
    //             foreach (var child in task.Result.Children)
    //                 try
    //                 {
    //                     var entry = new LeaderboardEntry(child.Value as Dictionary<string, object>);
    //                     entries.Add(entry);
    //                 }
    //                 catch (Exception e)
    //                 {
    //                     Debug.LogError(e);
    //                 }
    //
    //             callback(TransactionResult.Ok, entries);
    //         });
    // }
    
    // NOTE: the below name-based jank is because for the sake of time
    //       we should not implement sign in and auth logic.
    //
    //       because we either get each user to sign in through firebase auth,
    //       or we just have a name input box
    //
    //       so during the prototype showcase, type in the same name
    //       (or modify the SetUserActivityStatistics function to use the
    //       firebase username as the display name and their uid as the sanitised name)
    
#if UNITY_EDITOR
    [HelpBox(
        "Set this name to anything but default programatically with Backend.SetName, " +
        "for sake of the game being a prototype just have a text input box in the beginning for them to input! " +
        "(The SetName function helps reasonably reproducibly a sanitised name identifier)",
        HelpBoxMessageType.Warning)]
    [Space(10)]
#endif
    [SerializeField] private string _userDisplayName = "Default";
    
    private string _userPhonyID = ToFirebaseCompatibleId("DEFAULT");

    private static string ToFirebaseCompatibleId(string input)
    {
        // Use SHA-256 for good collision resistance
        using var sha = SHA256.Create();
        var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
        
        // Use Base64 URL-safe encoding (Firebase-safe characters)
        var base64 = Convert.ToBase64String(hash)
            .Replace('+', '-')
            .Replace('/', '_')
            .Replace("=", "");
        
        // Truncate to exactly 28 characters
        return base64[..28];
    }

    /// <summary>
    ///     use this function to set the user's name
    /// </summary>
    /// <param name="name">
    ///     the name to set for the user as their display name, taken from a text input box or similar
    /// </param>
    public void SetName(string name)
    {
        _userDisplayName = name;
        _userPhonyID = ToFirebaseCompatibleId(name.ToUpper().Replace(" ", ""));
    }

    public void UpdateUserActivityStatistics(string activityName, ActivityStatistics statistics, Action<TransactionResult> callback)
    {
        if (!status.Equals(FirebaseConnectionStatus.Connected)) return;

        if (string.IsNullOrEmpty(_userPhonyID))
        {
            callback(TransactionResult.Unauthenticated);
            return;
        }

        var activityStats = new Dictionary<string, object>
        {
            { "running_time_idle", statistics.runningTimeIdle },
            { "running_time_active", statistics.runningTimeActive },
            { "actions_taken", statistics.totalActionsTaken },
            { "started_at_epoch", statistics.startedAtEpoch }
        };

        var userNameTask = _db.Child("users")
            .Child(_userPhonyID)
            .Child("name")
            .SetValueAsync(_userDisplayName);

        var activityStatsTask = _db.Child("activity")
            .Child(activityName)
            .Child(_userPhonyID)
            .SetValueAsync(activityStats);

        Task.WhenAll(userNameTask, activityStatsTask).ContinueWithOnMainThread(task =>
        {
            callback(task.IsCompletedSuccessfully ? TransactionResult.Ok : TransactionResult.Error);
        });
    }
}