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
        var decomposedComposition = enemy_composition.GetTuples();

        BattleManager.Instance.LaunchBattle(decomposedComposition);
        GridManager.Instance.GetRandomTile().Select();

        UnitManager.Instance.GetRandomUnit().CastSpell(0);
    }
    
}
