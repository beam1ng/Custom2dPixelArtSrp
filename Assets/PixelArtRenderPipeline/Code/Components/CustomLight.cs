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
    public LightType Type { get; set; } = LightType.Directional;

    [field: SerializeField]
    public float Intensity { get; set; } = 1f;

    [field: SerializeField]
    public Color Color { get; set; } = Color.white;

    [field: SerializeField]
    public float Range { get; set; } = 1f;
}
