using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "New Composition", menuName = "Composition")]
public class ScriptableComposition : ScriptableObject
{
    [SerializeField] public List<ScriptableUnit> units;
    [SerializeField] public List<Vector2> positions;
    [SerializeField] public List<int> levels;

    public List<Tuple<Vector2, ScriptableUnit, int>> GetTuples(){
        //TODO vérification de la forme
        var decomposed_composition = new List<Tuple<Vector2, ScriptableUnit, int>>();

        for (int x = 0; x < units.Count; x++) {
            decomposed_composition.Add(new Tuple<Vector2, ScriptableUnit, int>(positions[x], units[x], levels[x]));
        }
        return decomposed_composition;
    }
}
