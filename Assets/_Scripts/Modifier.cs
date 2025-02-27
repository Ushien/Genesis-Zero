using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// S'applique sur une unité ou un spell pour lui conférer une modification, temporaire ou non.
/// </summary>
public class Modifier : MonoBehaviour
{
    public float powerBonus = 0f;
    public float healthBonus = 0f;
    private int turns = 0;
    public List<Properties> properties = new List<Properties>();
    private int howManyActivations = 0;
    public enum Duration {Battle,Permanent, Finished};
    private Duration duration = Duration.Battle;

    public void Setup(GameObject origin, float _powerBonus = 0f, float _healthBonus = 0f, List<Properties> _properties = null, Duration _duration = Duration.Battle, int _turns = -1, int _howManyActivations = -1){
        powerBonus = _powerBonus;
        healthBonus = _healthBonus;
        if(_properties != null){
            properties = _properties;
        }
        turns = _turns;
        duration = _duration;
        howManyActivations = _howManyActivations;

        name = "Origin: " + origin.name;
    }

    public float GetNewAmount(float amount){
        return (powerBonus + 1)* amount;
    }

    public List<Properties> GetProperties(){
        return properties;
    }

    public void ReduceTurns(){
        if(turns != -1){
            turns -= 1;
        }
        if(turns == 0){
            duration = Duration.Finished;
        }
    }

    public void ReduceActivations(){
        if(howManyActivations != -1){
            howManyActivations -= 1;
        }
        if(howManyActivations == 0){
            duration = Duration.Finished;
        }
    }

    public bool IsEnded(){
        if(duration == Duration.Finished){
            return true;
        }
        else{
            return false;
        }
    }

    public void EndBattle(){
        if(duration == Duration.Battle){
            duration = Duration.Finished;
        }
    }

    public string GetSummary(){

        string propertiesToDisplay = " - ";
        foreach (Properties prop in properties)
        {
            propertiesToDisplay = propertiesToDisplay + prop + " - ";
        }

        return "Modifier : \nPower bonus: " + powerBonus + "\nHealth bonus: " + healthBonus + "\nTurns: " + turns + "\nDuration: " + duration + "\nProperties: " + propertiesToDisplay + "\nHowManyActivations: " + howManyActivations;
    }
}
