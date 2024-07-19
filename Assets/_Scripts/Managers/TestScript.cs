using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

/// <summary>
/// Méthodes de débug et de test
/// </summary>

public class TestScript : MonoBehaviour
{
    public ScriptableComposition enemy_composition;
    public ScriptableComposition ally_composition;

    void Start()
    {
        BattleManager.Instance.LaunchBattle(ally_composition.GetTuples(), enemy_composition.GetTuples());

        BattleManager.Instance.DebugSetState();

        BattleManager.Instance.ChangeState(BattleManager.Machine.PLAYERACTIONCHOICESTATE, BattleManager.Trigger.FORWARD);

        
        foreach (BaseUnit unit in UnitManager.Instance.GetUnits(Team.Ally)){
            //unit.SetHP(20);
        }
        UnitManager.Instance.GetRandomUnit(Team.Enemy).ModifyArmor(+50);

    }

    void Update(){
        if(UnitManager.Instance.GetUnits(Team.Enemy).Count == 0){
            UnitManager.Instance.SpawnEnemies(enemy_composition.GetTuples());
        }
    }
    /*
    void OnGUI()
    {
        if (GUI.Button(new Rect(Screen.width / 2 - 50, 5, 100, 30), "Click"))
        {
            EventManager.Instance.Click();
        }
    }¨
    */
}
