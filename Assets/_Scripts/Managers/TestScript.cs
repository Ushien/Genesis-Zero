using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class TestScript : MonoBehaviour
{
    public ScriptableComposition enemy_composition;

    void Start()
    {
        var selected_spell = SpellManager.Instance.GetRandomSpell();
        SpellManager.Instance.SetupSpell(selected_spell);

        var decomposedComposition = enemy_composition.GetTuples();

        BattleManager.Instance.LaunchBattle(decomposedComposition);
    }

    /*
    public Tuple<Vector2, ScriptableUnit, int> DecomposeScriptableComposition(ScriptableComposition comp){
        return new Tuple<Vector2, ScriptableUnit, int>(new Vector2(0, 0), comp.units[0], 10);
    }
    */
    
}
