using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class TestScript : MonoBehaviour
{
    public ScriptableComposition enemy_composition;
    public ScriptableComposition ally_composition;

    void Start()
    {
        BattleManager.Instance.LaunchBattle(ally_composition.GetTuples(), enemy_composition.GetTuples());
        GridManager.Instance.GetRandomTile(Team.Enemy).Select();

        //UnitManager.Instance.GetRandomUnit().CastSpell(0);
    }
    
}
