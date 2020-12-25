using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointClickPicker : MonoBehaviour
{
    [SerializeField]
    private Vector3 clickedPosition;

    public static event Action<Vector3> OnNewPointAvailable;

    // Layer that enemy is on - remember to set this in inspector.
    public LayerMask _layerMask;

    // Distance to cast ray
    public float _raycastDistance = 10000f;

    private void OnMouseDown()
    {
         Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // Create ray from camera into the world through the mouse cursor position.

        if (Physics.Raycast(ray.origin, ray.direction, out RaycastHit hitInfo, _raycastDistance, _layerMask)) // If raycast hit a collider...
        {
            Debug.Log(hitInfo.collider.name); // Print name of enemy it hit.
            clickedPosition = hitInfo.point;
            OnNewPointAvailable?.Invoke(clickedPosition);
        }       
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(clickedPosition, 0.5f);
    }
}
