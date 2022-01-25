using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GlobalCanvas : MonoBehaviour
{
    public Canvas canvas;
    public static GlobalCanvas globalCanvas;
    public Transform PlayerCanvas;
    void Awake()
    {
        globalCanvas = this;
    }

}
