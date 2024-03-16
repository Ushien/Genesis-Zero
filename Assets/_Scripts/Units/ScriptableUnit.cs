using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Unit", menuName = "Unit")]

public class ScriptableUnit : ScriptableObject
{
    public string unit_name;

    public Sprite sprite;
    public int original_power;
    public int original_health;
    [TextArea(5,10)]
    public string description;
}