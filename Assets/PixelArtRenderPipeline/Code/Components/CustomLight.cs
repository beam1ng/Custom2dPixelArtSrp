using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomLight : MonoBehaviour
{
    public enum LightType
    {
        Directional,
        Point
    };

    [field: SerializeField]
    public LightType Type { get; private set; } = LightType.Directional;

    [field: SerializeField]
    public float Intensity { get; private set; } = 1f;

    [field: SerializeField]
    public Color Color { get; private set; } = Color.white;

    [field: SerializeField]
    public float Range { get; private set; } = 1f;
}
