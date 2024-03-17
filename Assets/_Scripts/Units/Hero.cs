using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : BaseUnit
{
    public Job job;
    public ScriptableSpell spell1;
    public ScriptableSpell spell2;
    //TODO public Weapon weapon;
    void Awake(){
        Team = Team.Ally;
    }
}
