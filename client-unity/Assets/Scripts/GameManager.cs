using System;
using System.Collections;
using System.Collections.Generic;
using SpacetimeDB;
using SpacetimeDB.Types;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    const string SERVER_URL = "http://http://10.0.0.68:3001"; // For Self-Host Playable Version (LAN - Must Reget IP For New Networks): Use "http://10.0.0.68:3001" For Solo Deploy Testing & Use Cloudflare Quicktunnel URL; For Maincloud Milestone Playable Version Release: "https://maincloud.spacetimedb.com"; For Local Playable Version: "http://127.0.0.1:3000";
    const string MODULE_NAME = "bash";

    public static event Action OnConnected;
    public static event Action OnSubscriptionApplied;

	public static GameManager Instance { get; private set; }
    public static Player Player { get; private set; }
    public static Identity LocalIdentity { get; private set; }
    public static DbConnection Conn { get; private set; }
    public static int PlayerCount { get; set; } = 0;
    
    private void Start()
    {
        PlayerPrefs.DeleteAll();
        Instance = this;
        Application.targetFrameRate = 60;
        Application.runInBackground = true;

        // In order to build a connection to SpacetimeDB we need to register
        // our callbacks and specify a SpacetimeDB server URI and module name.
        var builder = DbConnection.Builder()
            .OnConnect(HandleConnect)
            .OnConnectError(HandleConnectError)
            .OnDisconnect(HandleDisconnect)
            .WithUri(SERVER_URL)
            .WithModuleName(MODULE_NAME);

        // If the user has a SpacetimeDB auth token stored in the Unity PlayerPrefs,
        // we can use it to authenticate the connection.
        if (AuthToken.Token != "")
        {
            builder = builder.WithToken(AuthToken.Token);
        }

        // Building the connection will establish a connection to the SpacetimeDB
        // server.
        Conn = builder.Build();
    }

    // Called when we connect to SpacetimeDB and receive our client identity
    void HandleConnect(DbConnection conn, Identity identity, string token)
    {
        Debug.Log("Connected.");
        AuthToken.SaveToken(token);
        LocalIdentity = identity;
        OnConnected?.Invoke();

        // Request all tables
        Conn.SubscriptionBuilder()
            .OnApplied(HandleSubscriptionApplied)
            .SubscribeToAllTables();

        Conn.Db.LoggedInPlayers.OnInsert += HandlePlayerLoggedIn;
        Conn.Db.LoggedOutPlayers.OnInsert += HandlePlayerLoggedOut;
    }

    void HandleConnectError(Exception ex)
    {
        Debug.LogError($"Connection error: {ex}");
    }

    void HandleDisconnect(DbConnection _conn, Exception ex)
    {
        Debug.Log("Disconnected.");
        if (ex != null)
        {
            Debug.LogException(ex);
        }
    }

    private void HandleSubscriptionApplied(SubscriptionEventContext ctx)
    {
        Debug.Log("Subscription applied!");
        OnSubscriptionApplied?.Invoke();
    }

    public static bool IsConnected()
    {
        return Conn != null && Conn.IsActive;
    }

    public void Disconnect()
    {
        Conn.Disconnect();
        Conn = null;
        Player = null;
        PlayerCount -= 1;
    }

    public void HandlePlayerLoggedIn(EventContext ctx, Player insertedPlayer)
    {
        if (insertedPlayer.Identity == LocalIdentity) {
            Player = insertedPlayer;
            MatchManager.Instance.InitializeMatchManager(Player);
        }

        PlayerCount += 1;
    }

    public void HandlePlayerLoggedOut(EventContext ctx, Player disconnectedPlayer)
    {
        PlayerCount -= 1;
    }
}


namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit { }
}