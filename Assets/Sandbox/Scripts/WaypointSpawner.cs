using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointSpawner : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        PointClickPicker.OnNewPointAvailable += PointClickPicker_OnNewPointAvailable;
    }

    private void PointClickPicker_OnNewPointAvailable(Vector3 obj)
    {
        transform.position = obj;
        GetComponent<ParticleSystem>().Play(true);
    }
}
