using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour
{
    [SerializeField] private MeshRenderer meshRenderer;

    public void SetMaterialColor(Color color)
    {
        meshRenderer.materials[0].color = color;
    }

    public void SetMaterialAlphaColor(float alphaValue)
    {
        var materials = meshRenderer.materials;
        var curColor = materials[0].color;
        curColor.a = alphaValue;
        materials[0].color = curColor;
        meshRenderer.materials = materials;
    }
}