using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;
using UnityEngine.Serialization;

public class Backend
{
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
}