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
    private List<string> scriptedInstructions = new List<string>();
    private List<string> scriptedPicks = new List<string>();

    public void Awake(){
        Instance = this;
    }

    public void Log(string text){
        if(GlobalManager.Instance.debug){
            if(!Directory.Exists(Application.dataPath + "/logs")){
                Directory.CreateDirectory(Application.dataPath + "/logs");
            }
            if(!File.Exists(logFile)){
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
            string[] stringInstructions = scriptedInstructions[0].Substring(2).Split("/");

            scriptedInstructions.RemoveAt(0);
            List<Instruction> instructions = new List<Instruction>();
            foreach (string stringInstruction in stringInstructions)
            {
                string[] subInstructions = stringInstruction.Split("-");

                Team team = Team.Enemy;
                if(subInstructions[0][0] == 'E'){
                    team = Team.Enemy;
                }
                else if(subInstructions[0][0] == 'A'){
                    team = Team.Ally;
                }
                Assert.AreEqual(team, BattleManager.Instance.ConvertTeamTurn(BattleManager.Instance.teamTurn));
                BaseUnit sourceUnit = GridManager.Instance.GetTileAtPosition(team, new Vector2(Int32.Parse(subInstructions[0][1].ToString()), Int32.Parse(subInstructions[0][2].ToString()))).GetUnit();
                
                BaseSpell spellToCast = sourceUnit.GetSpellByIndex(Int32.Parse(subInstructions[1]));
                
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
                
                Instruction newInstruction = BattleManager.Instance.CreateInstruction(sourceUnit,spellToCast, targetTile, hyper : _hyper);
                instructions.Add(newInstruction);
            }

            return instructions;
        }
        else{
            return null;
        }
        
    }

    public Vector2 GetScriptedPicks(){
        if(AreThereScriptedPicks()){
            string[] stringPicks = scriptedPicks[0].Split(":")[1].Split("-");
            scriptedPicks.RemoveAt(0);
            return new Vector2(Int32.Parse(stringPicks[0]), Int32.Parse(stringPicks[1]));
        }
        else{
            return new Vector2(-1, -1);
        }
    }

    public bool AreThereScriptedInstructions(){
        return scriptedInstructions.Count > 0;
    }

    public bool AreThereScriptedPicks(){
        return scriptedPicks.Count > 0;
    }

    public void Start(){
        if(GlobalManager.Instance.debug){

            // Vérifie si un log d'instructions scriptées est présent dans le dossier
            if(Directory.Exists(Application.dataPath + "/logs/debug")){
                if(Directory.GetFiles(Application.dataPath + "/logs/debug").Count() > 0){
                    Assert.IsTrue(Directory.GetFiles(Application.dataPath + "/logs/debug").Count() == 2, "Il ne peut y avoir qu'un seul fichier d'instructions dans le dossier logs/debug (et ses métadonnées)");
                    string[] allLines = File.ReadAllLines(Directory.GetFiles(Application.dataPath + "/logs/debug")[0]);
                    GlobalManager.Instance.runSeed = Int32.Parse(allLines.Where(line => line[0] == 'S').First().Substring(2));
                    scriptedInstructions = allLines.Where(line => line[0] == 'I').ToList();
                    scriptedPicks = allLines.Where(line => line[0] == 'P').ToList();
                    File.Delete(Directory.GetFiles(Application.dataPath + "/logs/debug")[0] + ".txt.meta");
                }
            }

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
