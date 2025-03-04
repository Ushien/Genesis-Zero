using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contient toutes les méthodes relatives à l'intelligence artificielle du jeu
/// </summary>

public class AIManager : MonoBehaviour
{
    public static AIManager Instance;
    public Instruction emptyInstruction;

    void Awake(){
        Instance = this;
    }

    /*
    Renvoie une liste d'instructions aléatoires pour une équipe donnée
    */
    private List<Instruction> GetDummyAIOrders(Team team){
        List<Instruction> instructions = new List<Instruction>();

        foreach (BaseUnit unit in UnitManager.Instance.GetUnits(team)){
            Instruction newInstruction = BattleManager.Instance.CreateInstruction(unit, unit.GetRandomSpell(includingAttack : true, availableOnly : true), UnitManager.Instance.GetRandomUnit(BattleManager.Instance.InvertTeam(team)).GetTile());
            instructions.Add(newInstruction);
        }

        return instructions;
    }

    /*
    Renvoie une liste d'instructions pour une équipe donnée
    */
    public List<Instruction> GetAIOrders(Team team){
        return GetDummyAIOrders(team);
    }
}
