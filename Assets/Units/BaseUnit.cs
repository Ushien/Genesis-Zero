using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseUnit : MonoBehaviour
{

    public Tile OccupiedTile;
    public Team Team;
    public int level;

}

public enum Team {
    Ally = 0,
    Enemy = 1
}
