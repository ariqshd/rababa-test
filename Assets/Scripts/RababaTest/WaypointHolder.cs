using System;
using System.Collections.Generic;
using UnityEngine;

namespace RababaTest
{
    public class WaypointHolder : MonoBehaviour
    {
        public Transform[] waypoints;

        private void OnValidate()
        {
            RefreshWaypoints();
        }

        public void RefreshWaypoints()
        {
            List<Transform> wayPointList = new List<Transform>();
            foreach (Transform child in transform)
            {
                wayPointList.Add(child);
            }
            
            waypoints = wayPointList.ToArray();
        }
    }
}
