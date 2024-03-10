using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Unit", menuName = "Unit")]

public class Unit : ScriptableObject
{
    public string unit_name;
    public int original_power;
    public int original_health;
    public string description;
    public BaseUnit UnitPrefab;
}