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

    public void LaunchDebug()
    {
        BaseUnit randomUnit = UnitManager.Instance.GetRandomUnit(Team.Enemy);
        randomUnit.ModifyArmor(+10, false);

        foreach (BaseUnit unit in UnitManager.Instance.GetUnits())
        {
            InterfaceManager.Instance.UpdateLifebar(unit);
        }

    }

    void Update(){
        //if(UnitManager.Instance.GetUnits(Team.Enemy).Count == 0){
            //UnitManager.Instance.SpawnEnemies(enemy_composition.GetTuples());
        //}
    }
    void OnGUI()
    {
        if(GlobalManager.Instance.GetRunPhase() == GlobalManager.RunPhase.PICKPHASE){
            if (GUI.Button(new Rect(Screen.width / 2 - 50, 5, 100, 30), "Easy Battle"))
            {
                GlobalManager.Instance.ChangeState(GlobalManager.RunPhase.BATTLEPHASE);
            }
        }
    }
}
