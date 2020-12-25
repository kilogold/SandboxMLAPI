using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using System;

/// <summary>
/// Central class to delegate game & network events to other classes.
/// 
/// The host needs to initally spawn the actors so they can be replicated accross the network.
/// </summary>
public class Gamestate : MonoBehaviour
{
    [Header("Actor Prefabs")]
    public GameObject MainPrefab;
    public GameObject CompanionPrefab;

    [Header("Spawn Points")]
    public Transform MainSpawn;
    public Transform CompanionSpawn;

    private InputFocus inputFocuse;

    //Server-Only Vars (Consider making separate scripts: ClientGameState, ServerGameState)
    NetworkedObject companionNetObj = null;

    void Start()
    {
        inputFocuse = GetComponent<InputFocus>();
        DontDestroyOnLoad(gameObject);

        NetworkingManager.Singleton.OnClientConnectedCallback += ServerAndConnectingClientOnly_OnClientConnectedCallback;
        NetworkingManager.Singleton.OnClientDisconnectCallback += ServerAndDisconnectingClientOnly_OnClientDisconnectCallback;
        NetworkingManager.Singleton.OnServerStarted += Singleton_OnServerStarted;
    }

    /// <summary>
    /// Only the Host should be invoking this callback. A second player can only join if Server has started.
    /// </summary>
    private void Singleton_OnServerStarted()
    {
        var mainInstance = Instantiate(MainPrefab, MainSpawn.position, Quaternion.identity).GetComponent<NetworkedObject>();
        mainInstance.Spawn();

        companionNetObj = Instantiate(CompanionPrefab, CompanionSpawn.position, Quaternion.identity).GetComponent<NetworkedObject>();
        companionNetObj.Spawn();

        // Consider:
        // What if a Host simply calls the "OnClientConnected" methods, right after server init?
        // This way we preserve Server/Host separations in order to easily run a dedicated server in the future,
        // instead of a server/client hybrid.
        inputFocuse.RegisterFocusTransform(ActorRole.Main, mainInstance.transform);
        inputFocuse.RegisterFocusTransform(ActorRole.Companion, companionNetObj.transform);
        inputFocuse.SetFocusTarget(ActorRole.Main);
    }

    private void ServerAndDisconnectingClientOnly_OnClientDisconnectCallback(ulong clientId)
    {
        //companionNetObj.ChangeOwnership(NetworkingManager.Singleton.LocalClientId);
        inputFocuse.locked = false;
    }

    private void ServerAndConnectingClientOnly_OnClientConnectedCallback(ulong clientId)
    {
        inputFocuse.locked = true;

        if (NetworkingManager.Singleton.IsHost)
        {
            inputFocuse.SetFocusTarget(ActorRole.Main);
            //companionNetObj.ChangeOwnership(clientId);
            return;
        }

        var actorMovs = GameObject.FindObjectsOfType<ActorMovement>();

        foreach (var item in actorMovs)
        {
            inputFocuse.RegisterFocusTransform(item.SinglePlayerInputMode, item.transform);
        }

        inputFocuse.SetFocusTarget(ActorRole.Companion);
    }
}
