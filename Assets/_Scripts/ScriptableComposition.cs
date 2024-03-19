using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Composition", menuName = "Composition")]

public class ScriptableComposition : ScriptableObject
{
    [SerializeField] private List<ScriptableUnit> units;
    [SerializeField] private List<Vector2> positions;
    [SerializeField] private List<int> levels;
}
