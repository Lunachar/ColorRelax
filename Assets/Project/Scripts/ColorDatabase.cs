using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New ColorDatabase", menuName = "ScriptableObjects/ColorDatabase", order = 1)]
public class ColorDatabase : ScriptableObject
{
    public List<Color> availibleColors = new List<Color>();
}
