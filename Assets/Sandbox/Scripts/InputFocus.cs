using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

/// <summary>
/// Controller class to assign player control to a spacific acter.
/// </summary>
public class InputFocus : MonoBehaviour
{
    public CinemachineTargetGroup targetGroup;
    public Transform[] focusTransforms;
    private ActorRole currentFocus;
    public bool locked;

    // Start is called before the first frame update
    void Start()
    {
        focusTransforms = new Transform[(int)ActorRole.MAX_ACTORS];
    }

    public void RegisterFocusTransform(ActorRole role, Transform transform)
    {
        ref Transform arraySlot = ref focusTransforms[(int)role];

        if (arraySlot != null)
            Debug.LogWarning("Overwritting preexisting registration");

        arraySlot = transform;
    }
    // Update is called once per frame
    void Update()
    {
        if (!locked && Input.GetKeyDown(KeyCode.E))
        {
            ToggleFocusTarget();
        }
    }

    private void ToggleFocusTarget()
    {
        switch (currentFocus)
        {
            case ActorRole.Main:
                SetFocusTarget(ActorRole.Companion);
                break;
            case ActorRole.Companion:
                SetFocusTarget(ActorRole.Main);
                break;
            default:
                break;
        }
    }

    public void SetFocusTarget(ActorRole role)
    {
        Transform oldAnchor = focusTransforms[(int)currentFocus];
        oldAnchor.GetComponent<ActorMovement>().steering = false;
        targetGroup.RemoveMember(oldAnchor);

        currentFocus = role;

        Transform newAnchor = focusTransforms[(int)currentFocus];
        newAnchor.GetComponent<ActorMovement>().steering = true;
        targetGroup.AddMember(newAnchor, 1, 2);
    }
}
