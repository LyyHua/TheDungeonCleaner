using System;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;

    private void Awake()
    {
        instance = this;
    }
}
