using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackData : MonoBehaviour
{
    public static TrackData Instance { get; private set; }

    public int LanesCount { get; private set; }
    public float LanesSpacing { get; private set; }
    private void Awake()
    {
        Instance = this;

        LanesCount = 5;
        LanesSpacing = 2;
    }
}
