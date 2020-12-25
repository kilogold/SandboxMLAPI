using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using UnityEngine.AI;
using MLAPI.Messaging;

public enum ActorRole
{
    Main,
    Companion,
    MAX_ACTORS
}

public class ActorMovement : NetworkedBehaviour
{
    private CharacterController controller;
    private NavMeshAgent navMeshAgent;
    private Vector3 playerVelocity;
    private float gravityValue = -9.81f;

    [Tooltip("Disable to prevent user input, while still honoring gravity.")]
    public bool steering;

    [Tooltip("Don't change after init. Wont' respond.")]
    public ActorRole SinglePlayerInputMode;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        navMeshAgent = GetComponent<NavMeshAgent>();

        PointClickPicker.OnNewPointAvailable += PointClickPicker_OnNewPointAvailable;
    }

    private void PointClickPicker_OnNewPointAvailable(Vector3 obj)
    {
        if (!steering)
            return;

        InvokeServerRpc(Net_ReplicateDestination, obj);
    }

    [ServerRPC(RequireOwnership =false)]
    private void Net_ReplicateDestination(Vector3 dst)
    {
        InvokeClientRpcOnEveryone(Net_UpdateDestination, dst);

        // This is a special case condition due to monolithic Host/Server script.
        // Ideally, we should have a 'server' & 'client' variant of these scripts.
        // We can then neatly run MLAPI on either Server mode (server component) or Host mode (server + client components).
        if(IsServer && !IsHost)
        {
            Net_UpdateDestination(dst);
        }
    }

    [ClientRPC]
    private void Net_UpdateDestination(Vector3 dst)
    {
        navMeshAgent.isStopped = false;
        navMeshAgent.SetDestination(dst);
    }

    void Update()
    {
        //if (!steering)
        //{
        //    GetComponent<NavMeshAgent>().isStopped = true;
        //}

        if (controller.isGrounded)
        {
            if (playerVelocity.y < 0)
                playerVelocity.y = 0f;
        }
        else
        {
            playerVelocity.y += gravityValue * Time.deltaTime;
            controller.Move(playerVelocity * Time.deltaTime);
        }
    }
}