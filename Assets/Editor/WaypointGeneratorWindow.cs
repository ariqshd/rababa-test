using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = System.Random;

namespace Editor
{
    public class WaypointGeneratorWindow : EditorWindow
    {
        private Transform _parentTransform;
        private int _numberOfWaypoints;

        private Vector3 _boxCenter = Vector3.zero;
        private Vector3 _boxSize = new Vector3(10, 10, 10);

        private bool _showDebugBox = true;
        private Color _debugBoxColor = Color.yellow;

        private bool _showWaypointGizmos = true;
        private float _waypointGizmoSize = 0.5f;
        private Color _waypointGizmoColor = Color.cyan;

        private List<GameObject> _generatedWaypoints = new List<GameObject>();

        [MenuItem("Tools/Random Waypoint Generator")]
        public static void ShowWindow()
        {
            GetWindow<WaypointGeneratorWindow>("Random Waypoint Generator");
        }

        private void OnGUI()
        {
            _parentTransform = (Transform)EditorGUILayout.ObjectField("Parent Transform", _parentTransform, typeof(Transform), true);
            _numberOfWaypoints = EditorGUILayout.IntField("Number of Waypoints", _numberOfWaypoints);

            EditorGUILayout.Space();
            
            EditorGUILayout.LabelField("Box Settings", EditorStyles.boldLabel);
            _boxCenter = EditorGUILayout.Vector3Field("Box Center (World)", _boxCenter);
            _boxSize = EditorGUILayout.Vector3Field("Box Size", _boxSize);
            
            EditorGUILayout.Space();
            
            EditorGUILayout.LabelField("Debug Visualization", EditorStyles.boldLabel);
            _showDebugBox = EditorGUILayout.Toggle("Show Box in Scene", _showDebugBox);
            _debugBoxColor = EditorGUILayout.ColorField("Box Color", _debugBoxColor);
            
            _showWaypointGizmos = EditorGUILayout.Toggle("Show Waypoint in Scene", _showWaypointGizmos);
            _waypointGizmoColor = EditorGUILayout.ColorField("Waypoint Color", _waypointGizmoColor);
            _waypointGizmoSize = EditorGUILayout.Slider("Waypoint Gizmo Size", _waypointGizmoSize, 0.1f, 2f);
            
            EditorGUILayout.Space();

            if (GUILayout.Button("Generate Waypoints"))
            {
                GenerateRandomWaypoints();
            }

            if (GUILayout.Button("Clear Waypoints"))
            {
                ClearGeneratedWaypoints();
            }
        }

        private void OnEnable()
        {
            SceneView.duringSceneGui += OnSceneGUI;
        }

        private void OnDisable()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
        }

        private Vector3 GetRandomPositionInBox()
        {
            Vector3 halfSize = _boxSize * 0.5f;
            
            float x = UnityEngine.Random.Range(_boxCenter.x - halfSize.x, _boxCenter.x + halfSize.x);
            float y = UnityEngine.Random.Range(_boxCenter.y - halfSize.y, _boxCenter.y + halfSize.y);
            float z = UnityEngine.Random.Range(_boxCenter.z - halfSize.z, _boxCenter.z + halfSize.z);
            
            return new Vector3(x, y, z);
        }

        private void GenerateRandomWaypoints()
        {
            if (_numberOfWaypoints <= 0)
            {
                return;
            }

            for (int i = 0; i < _numberOfWaypoints; i++)
            {
                Vector3 randomPosition = GetRandomPositionInBox();
                
                GameObject waypoint = new GameObject($"Waypoint {i}");
                waypoint.transform.position = randomPosition;

                if (_parentTransform != null)
                {
                    waypoint.transform.SetParent(_parentTransform);
                }
                
                _generatedWaypoints.Add(waypoint);
            }
        }
        
        private void ClearGeneratedWaypoints()
        {
            for (int i = _generatedWaypoints.Count - 1; i >= 0; i--)
            {
                if (_generatedWaypoints[i] != null)
                {
                    Undo.DestroyObjectImmediate(_generatedWaypoints[i]);
                }
            }
            
            _generatedWaypoints.Clear();
        }

        private void OnSceneGUI(SceneView sceneView)
        {
            if (_showDebugBox)
            {
                Handles.color = _debugBoxColor;
                Handles.DrawWireCube(_boxCenter, _boxSize);
            }

            if (_showWaypointGizmos && _generatedWaypoints != null)
            {
                Handles.color = _waypointGizmoColor;
                foreach (var wp in _generatedWaypoints)
                {
                    if(wp == null) continue;
                    Handles.DrawWireDisc(wp.transform.position, Vector3.up, _waypointGizmoSize);
                }
            }
        }
    }
}
