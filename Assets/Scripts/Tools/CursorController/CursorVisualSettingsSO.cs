using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CursorVisualSettings", menuName = "Cursor/CursorVisualSettings")]
public class CursorVisualSettingsSO : ScriptableObject
{
    public Texture2D[] Textures;
    public Vector2 HotSpot = Vector2.zero;
    public CursorMode Mode = CursorMode.Auto;

    [Header("Animation")]
    public bool IsAnimated = false;
    public float FrameDelay = 0.05f;
    public int CurrentFrame = 0;
}
