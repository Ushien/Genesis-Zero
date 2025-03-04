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
    private string scriptedInstructions = "A00-0-E02/A10-0-E12/A20-0-E22";

    public void Awake(){
        Instance = this;
    }

    public void LaunchDebug()
    {
        BaseUnit randomEnemy = UnitManager.Instance.GetRandomUnit(Team.Enemy);

        BaseUnit randomAlly = UnitManager.Instance.GetRandomUnit(Team.Ally);

        AnimationManager.Instance.ForceAnimation();
    }

    public List<Instruction> GetScriptedInstructions(){

        string[] stringInstructions = scriptedInstructions.Split("/");
        List<Instruction> instructions = new List<Instruction>();
        foreach (string stringInstruction in stringInstructions)
        {
            string[] subInstructions = stringInstruction.Split("-");
            Instruction newInstruction = Instantiate(AIManager.Instance.emptyInstruction);
            Team team = Team.Enemy;
            if(subInstructions[0][0] == 'E'){
                team = Team.Enemy;
            }
            else if(subInstructions[0][0] == 'A'){
                team = Team.Ally;
            }
            Assert.AreEqual(team, BattleManager.Instance.ConvertTeamTurn(BattleManager.Instance.teamTurn));
            BaseUnit sourceUnit = GridManager.Instance.GetTileAtPosition(team, new Vector2(Int32.Parse(subInstructions[0][1].ToString()), Int32.Parse(subInstructions[0][2].ToString()))).GetUnit();
            
            BaseSpell spellToCast = sourceUnit.GetSpells(includingAttack : true)[Int32.Parse(subInstructions[1])];
            
            if(subInstructions[2][0] == 'E'){
                team = Team.Enemy;
            }
            else if(subInstructions[2][0] == 'A'){
                team = Team.Ally;
            }
            Tile targetTile = GridManager.Instance.GetTileAtPosition(team, new Vector2(Int32.Parse(subInstructions[2][1].ToString()), Int32.Parse(subInstructions[2][2].ToString())));
            newInstruction.Setup(sourceUnit,spellToCast, targetTile);
            instructions.Add(newInstruction);
            Debug.Log(newInstruction.GetSummary());
        }

        return instructions;
    }

    public bool AreThereScriptedInstructions(){
        return true;
    }

    public void TestAssertions(){
        
    }

    void Update(){

    }
    void OnGUI()
    {

    }
}
