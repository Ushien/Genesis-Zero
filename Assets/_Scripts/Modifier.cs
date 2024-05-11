using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Modifier : MonoBehaviour
{
    public float powerBonus = 0f;
    private int turns = 0;
    private bool permanent = true;
    public Properties property = Properties.Empty;

    public void Setup(float _powerBonus = 0f, Properties _property = Properties.Empty, bool _permanent = true, int _turns = 0 ){
        powerBonus = _powerBonus;
        property = Properties.Empty;
        turns = _turns;
        permanent = _permanent;
    }

    public float GetNewAmount(float amount){
        return (powerBonus + 1)* amount;
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
        if (turns > 0){
            return false;
        }
        return true;
    }
}
