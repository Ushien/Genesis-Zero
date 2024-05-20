using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIManager : MonoBehaviour
{
    public static AIManager Instance;

    void Awake(){
        Instance = this;
    }
    public List<Instruction> GetAIOrders(Team team){
        List<Instruction> instructions = new List<Instruction>();
        BaseUnit randomUnit = UnitManager.Instance.GetRandomUnit(Team.Enemy);

        instructions.Add(new Instruction(randomUnit, randomUnit.GetSpells()[0], UnitManager.Instance.GetRandomUnit(Team.Ally).GetTile()));

        return instructions;
    }
}
