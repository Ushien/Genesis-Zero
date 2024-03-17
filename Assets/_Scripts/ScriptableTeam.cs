using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Team", menuName = "Team")]

public class ScriptableTeam : ScriptableObject
{
    [SerializeField] private List<ScriptableUnit> units;
    [SerializeField] private List<Vector2> positions;
    [SerializeField] private List<int> levels;
}
