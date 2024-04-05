using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackData : MonoBehaviour
{
    public static TrackData Instance { get; private set; }

    public int LanesCount { get; private set; } = 5;
    public float LanesSpacing { get; private set; } = 3;
    private void Awake()
    {
        Instance = this;
    }
}
