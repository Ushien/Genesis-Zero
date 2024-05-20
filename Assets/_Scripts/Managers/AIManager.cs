using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIManager : MonoBehaviour
{
    public static AIManager Instance;

    void Awake(){
        Instance = this;
    }
    public List<Instruction> GetDummyAIOrders(Team team){
        List<Instruction> instructions = new List<Instruction>();

        foreach (BaseUnit unit in UnitManager.Instance.GetUnits(Team.Enemy)){
            instructions.Add(new Instruction(unit, unit.GetRandomSpell(includingAttack : true), UnitManager.Instance.GetRandomUnit(Team.Ally).GetTile()));
        }

        return instructions;
    }

    public List<Instruction> GetAIOrders(Team team){
        return GetDummyAIOrders(team);
    }
}
