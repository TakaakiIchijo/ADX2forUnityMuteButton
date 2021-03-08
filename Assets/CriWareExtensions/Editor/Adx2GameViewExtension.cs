using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

[InitializeOnLoad]
public static class GameViewExtension
{
    private const string EditorPrefsIsAtomMute = "Editor_Prefs_IsAtomMute";

    static GameViewExtension()
    {
        var toolbarToggle = new ToolbarToggle () {text = "Mute ADX2"};

        var asm = Assembly.Load("UnityEditor");

        var typeGameView = asm.GetType("UnityEditor.GameView");
        var gameView = EditorWindow.GetWindow(typeGameView, false, "Game", false);

        var toolbar = new VisualElement();
        var style = toolbar.style;
        var rootVisualElement = gameView.rootVisualElement;

        style.flexDirection = FlexDirection.RowReverse;
        style.height = 20;
        style.marginRight = 106;
        style.paddingLeft = 480;
        
        toolbarToggle.value = EditorPrefs.GetBool(EditorPrefsIsAtomMute);
        
        toolbarToggle.RegisterValueChangedCallback(x =>
        {
            Debug.Log("Editor_Prefs_IsAtomMute "+toolbarToggle.value);
            EditorPrefs.SetBool(EditorPrefsIsAtomMute, toolbarToggle.value);
            SetAdx2Mute(toolbarToggle.value);
        });

        toolbar.Add(toolbarToggle);
        toolbar.BringToFront();

        rootVisualElement.Clear();
        rootVisualElement.Add(toolbar);
        
        EditorApplication.playModeStateChanged += OnChangedPlayMode;
    }

    private static void SetAdx2Mute(bool value)
    {
        if (Application.isPlaying)
        {
            var volume = value ? 0f : 1f;
            CriAtomExAsr.SetBusVolume("MasterOut", volume);

            if (value)
            {
                Debug.Log("<color=red>ADX2 audio is muted</color>"); 
            }
        }
    }

    private static void OnChangedPlayMode(PlayModeStateChange state) {
        
        if (state == PlayModeStateChange.ExitingEditMode)
        {
            
        }
        else if (state == PlayModeStateChange.EnteredPlayMode)
        {
            if (EditorPrefs.HasKey(EditorPrefsIsAtomMute))
            {
                var isMute = EditorPrefs.GetBool(EditorPrefsIsAtomMute, false);
                if (isMute == false) return;

                SetAdx2Mute(true);
            }
            
        }
        else if (state == PlayModeStateChange.ExitingPlayMode) {

        }
        else if (state == PlayModeStateChange.EnteredEditMode) {

        }
    }
}
