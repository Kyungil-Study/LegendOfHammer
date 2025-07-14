using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.SceneManagement;


public class DebugScreen : SingletonBase<DebugScreen> 
{
    [Header("Debug Screen")]
    public Font debugFont;
    public int fontSize = 20;
    public Color textColor = Color.white;
    public TextAnchor alignment = TextAnchor.UpperLeft;
    public Vector2 offset = Vector2.zero;

    public List<string> debugText = new List<string>();

    public override void OnInitialize()
    {
        base.OnInitialize();
        SceneManager.sceneLoaded += (Scene scene, LoadSceneMode mode) =>
        {
            // Clear the debug text when a new scene is loaded
            debugText.Clear();
            AddLog($"Scene Loaded: {scene.name}");
        };
    }

    private void AddLog(string text)
    {
        if (debugText.Count == 5)
        {
            debugText.RemoveAt(0);
        }
        
        debugText.Add(text);
    }
    
    public static void Log(string text)
    {
        Instance.AddLog(text);
        Debug.Log(text);
        
    }
    
    public static void LogWarning(string text)
    {
        Instance.AddLog(text);
        Debug.LogWarning(text);
    }
    
    public static void LogError(string text)
    {
        Instance.AddLog(text);
        Debug.LogError(text);
    }

    private void OnGUI()
    {
        if (debugFont != null)
        {
            GUI.skin.font = debugFont;
        }
        GUIStyle style = new GUIStyle();
        style.fontSize = fontSize;
        style.normal.textColor = textColor;
        style.alignment = alignment;

        // Calculate the position based on offset and screen size
        Vector2 position = new Vector2(offset.x + 10, offset.y + 10); // Add a small margin

        if (alignment == TextAnchor.UpperCenter || alignment == TextAnchor.MiddleCenter || alignment == TextAnchor.LowerCenter)
        {
            position.x = Screen.width / 2;
        }
        else if (alignment == TextAnchor.UpperRight || alignment == TextAnchor.MiddleRight || alignment == TextAnchor.LowerRight)
        {
            position.x = Screen.width - 10; // Add a small margin
        }

        if (alignment == TextAnchor.LowerLeft || alignment == TextAnchor.LowerCenter || alignment == TextAnchor.LowerRight)
        {
            position.y = Screen.height - 10; // Add a small margin
        }
        else if (alignment == TextAnchor.MiddleLeft || alignment == TextAnchor.MiddleCenter || alignment == TextAnchor.MiddleRight)
        {
            position.y = Screen.height / 2;
        }
        
        for(int i = 0; i < debugText.Count; i++)
        {
            string debugMessage = debugText[i];
            GUI.Label(new Rect(position.x, position.y + (i * fontSize), 200, 50), debugMessage, style);
        }
    }
}

