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
    private string logFile;
    private List<string> scriptedInstructions = new List<string>(){"A00-0-E02-_/A10-0-E12-_/A20-0-E22-_","A00-0-E02-_/A10-0-E12-_/A20-0-E22-_", "A00-0-E02-_/A10-0-E12-_/A20-0-E22-_", "A00-0-E02-_/A10-0-E12-_/A20-0-E22-_","A00-0-E02-_/A10-0-E12-_/A20-0-E22-_","A00-0-E02-_/A10-0-E12-_/A20-0-E22-_"};

    public void Awake(){
        Instance = this;
    }

    public void Log(string text){
        if(GlobalManager.Instance.debug){
            if (!File.Exists(logFile)){
                File.WriteAllText(logFile, string.Empty);
            }
            using (var writer = new StreamWriter(logFile, true)){
                writer.WriteLine(text);
            }
            }
    }

    [ContextMenu("Clear log directory")]
    public void ClearLogs(){
        Array.ForEach(Directory.GetFiles(Application.dataPath + "/logs"), File.Delete);
    }

    public void LaunchDebug()
    {
        BaseUnit randomEnemy = UnitManager.Instance.GetRandomUnit(Team.Enemy);

        BaseUnit randomAlly = UnitManager.Instance.GetRandomUnit(Team.Ally);

        AnimationManager.Instance.ForceAnimation();
    }

    public List<Instruction> GetScriptedInstructions(){

        //return AIManager.Instance.GetAIOrders(Team.Ally);

        if(AreThereScriptedInstructions()){

            string[] stringInstructions = scriptedInstructions[0].Split("/");
            scriptedInstructions.RemoveAt(0);
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
                
                bool _hyper = false;
                
                if(subInstructions[3][0] == 'H'){
                    _hyper = true;
                }
                
                newInstruction.Setup(sourceUnit,spellToCast, targetTile, hyper : _hyper);
                instructions.Add(newInstruction);
            }

            return instructions;
        }
        else{
            return null;
        }
        
    }

    public bool AreThereScriptedInstructions(){
        return scriptedInstructions.Count > 0;
    }

    public void Start(){
        if(GlobalManager.Instance.debug){
            int i = 1;
            while(File.Exists(Application.dataPath + "/logs/log" + i + ".txt"))
            {
                i++;
            }
            logFile = Application.dataPath + "/logs/log" + i + ".txt";
        }
    }

    public void TestAssertions(){
        
    }

    void Update(){

    }
    void OnGUI()
    {

    }
}
