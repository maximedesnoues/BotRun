using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointAssigner : MonoBehaviour
{
    [SerializeField] private List<Zone> zones;

    private void Start()
    {
        foreach (Zone zone in zones)
        {
            AssignWaypointsToCars(zone.forwardWaypoints, zone.forwardCars);
            // AssignWaypointsToCars(zone.reverseWaypoints, zone.reverseCars);
        }
    }

    private void AssignWaypointsToCars(List<Transform> waypoints, List<GameObject> cars)
    {
        foreach (GameObject car in cars)
        {
            CarController carController = car.GetComponent<CarController>();
            if (carController != null)
            {
                carController.waypoints = waypoints;
            }
        }
    }
}

[System.Serializable]
public class Zone
{
    public List<Transform> forwardWaypoints;
    // public List<Transform> reverseWaypoints;
    public List<GameObject> forwardCars;
    // public List<GameObject> reverseCars;
}