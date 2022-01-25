using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour
{
    public static event Action Targets;
    public static CursorController Instance;

    public CursorVisualSettingsSO VisualSettings;
    private float _counter = 0;

    public static bool IsActive => Targets != null;

    public void SetVisualSettings (CursorVisualSettingsSO cursorVisualSettings)
    {
        if(cursorVisualSettings != null)
        {
            if (cursorVisualSettings.Textures != null && cursorVisualSettings.Textures.Length > 0)
            {
                VisualSettings = cursorVisualSettings;
                Cursor.SetCursor(VisualSettings.Textures[0], VisualSettings.HotSpot, VisualSettings.Mode);
            }
            else
            {
                Debug.LogError("visual settings is broken");
            }
        }
        else
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
    }

    private void Awake ()
    {
        Instance = this;

        SetVisualSettings(VisualSettings);
    }

    private void Update()
    {
        if (Targets != null)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            if (VisualSettings != null)
            {
                if(VisualSettings.IsAnimated)
                {
                    if (_counter > VisualSettings.FrameDelay)
                    {
                        _counter = 0;

                        VisualSettings.CurrentFrame++;

                        if (VisualSettings.CurrentFrame >= VisualSettings.Textures.Length)
                            VisualSettings.CurrentFrame = 0;

                        Cursor.SetCursor(VisualSettings.Textures[VisualSettings.CurrentFrame], VisualSettings.HotSpot, VisualSettings.Mode);
                    }
                    _counter += Time.deltaTime;
                }
            }
        }
        else
        {
            //if (_Graphics.ScreenMode < 2)
                //Cursor.lockState = CursorLockMode.Locked;
            //else
                //Cursor.lockState = CursorLockMode.Confined;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
