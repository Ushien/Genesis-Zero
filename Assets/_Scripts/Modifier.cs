using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// S'applique sur une unité ou un spell pour lui conférer une modification, temporaire ou non.
/// </summary>
public class Modifier : MonoBehaviour
{
    public float powerBonus = 0f;
    private int turns = 0;
    private bool permanent = true;
    public List<Properties> properties = new List<Properties>();
    private int howManyActivations = 0;

    public void Setup(float _powerBonus = 0f, List<Properties> _properties = null, bool _permanent = true, int _turns = 0, int _howManyActivations = 0){
        powerBonus = _powerBonus;
        if(_properties != null){
            properties = _properties;
        }
        turns = _turns;
        permanent = _permanent;
        howManyActivations = _howManyActivations;
    }

    public float GetNewAmount(float amount){
        return (powerBonus + 1)* amount;
    }

    public List<Properties> GetProperties(){
        return properties;
    }

    public void ModifyTurns(int amount = -1){
        if(!IsPermanent()){
            turns += amount;
        }
    }

    public bool IsPermanent(){
        return permanent;
    }

    public bool IsEnded(){
        if (IsPermanent()){
            return false;
        }
        if (turns != 0){
            return false;
        }
        if (IsPermanent() && howManyActivations >= 0){
            return false;
        }
        return true;
    }

    public void ReduceActivations(int howManyReductions = 1){
        howManyActivations -= howManyReductions;
        if(howManyActivations < 0){
            howManyActivations = 0;
        }
    }
}
