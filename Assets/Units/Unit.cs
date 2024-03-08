using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Unit", menuName = "Scriptable Unit")]

public class Unit : ScriptableObject
{
    public Team Team;
    public BaseUnit UnitPrefab;
}

public enum Team {
    Ally = 0,
    Enemy = 1
}