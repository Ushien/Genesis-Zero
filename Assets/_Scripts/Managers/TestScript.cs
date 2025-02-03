using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using System.IO;
using UnityEngine.Assertions;

/// <summary>
/// Méthodes de débug et de test
/// </summary>

public class TestScript : MonoBehaviour
{
    public ScriptableComposition enemy_composition;
    public ScriptableComposition ally_composition;
    public List<ScriptableObject> rewardsToSpawn;
    public static TestScript Instance;

    public void Awake(){
        Instance = this;
    }

    public void LaunchDebug()
    {
        BaseUnit randomUnit = UnitManager.Instance.GetRandomUnit(Team.Enemy);
        randomUnit.ModifyArmor(+10, false);
        BaseUnit randomAlly = UnitManager.Instance.GetRandomUnit(Team.Ally);
        randomAlly.SetHP(300, false);
        InterfaceManager.Instance.UpdateLifebar(randomUnit, 0, 10);
        InterfaceManager.Instance.UpdateLifebar(randomAlly, -(randomAlly.GetTotalHealth()-300), 0);
    }

    void Update(){
        //if(UnitManager.Instance.GetUnits(Team.Enemy).Count == 0){
            //UnitManager.Instance.SpawnEnemies(enemy_composition.GetTuples());
        //}
    }
    void OnGUI()
    {
        /*
        if(GlobalManager.Instance.GetRunPhase() == GlobalManager.RunPhase.PICKPHASE){
            if (GUI.Button(new Rect(Screen.width / 2 - 50, 5, 100, 30), "Easy Battle"))
            {
                GlobalManager.Instance.ChangeState(GlobalManager.RunPhase.BATTLEPHASE);
            }
        }
        */
        /* if(GlobalManager.Instance.GetRunPhase() == GlobalManager.RunPhase.PICKPHASE){
            if (GUI.Button(new Rect(Screen.width / 3 - 50, 5, 100, 30), "Test"))
            {
                PickPhaseManager.Instance.SetCurrentRewards(new List<Reward>{
                    PickPhaseManager.Instance.GenerateReward(PickPhaseManager.RewardType.SPELL),
                    PickPhaseManager.Instance.GenerateReward(PickPhaseManager.RewardType.SPELL),
                    PickPhaseManager.Instance.GenerateReward(PickPhaseManager.RewardType.SPELL)
                });
                PickPhaseManager.Instance.DisplayRewards();
            }
        }*/   
    }
}
