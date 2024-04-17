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

        BattleManager.Instance.DebugSetState();
        GridManager.Instance.GetRandomTile(Team.Enemy).Select();

        //UnitManager.Instance.GetRandomUnit().CastSpell(0);
    }
    void OnGUI()
    {
        if (GUI.Button(new Rect(Screen.width / 2 - 50, 5, 100, 30), "Click"))
        {
            EventManager.Instance.Click();
        }
    }
}
