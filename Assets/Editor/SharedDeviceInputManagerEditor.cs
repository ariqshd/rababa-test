using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Editor;

[CustomEditor(typeof(SharedPlayerInputManager))]
public class SharedDeviceInputManagerEditor : UnityEditor.Editor
{
    private SharedPlayerInputManager _inputManager;
    private UnityEditor.Editor _defaultEditor;

    private void OnEnable()
    {
        _inputManager = target as SharedPlayerInputManager;
        _defaultEditor = UnityEditor.Editor.CreateEditor(_inputManager as PlayerInputManager, typeof(PlayerInputManagerEditor));
    }

    public override void OnInspectorGUI()
    {
        _defaultEditor.OnInspectorGUI();
    }
}
