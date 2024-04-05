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
        var enemyComposition = enemy_composition.GetTuples();

        BattleManager.Instance.LaunchBattle(new List<Tuple<Vector2, ScriptableUnit, int>>(),enemyComposition);
        GridManager.Instance.GetRandomTile(Team.Enemy).Select();

        //UnitManager.Instance.GetRandomUnit().CastSpell(0);
    }
    
}
