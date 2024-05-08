using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Modifier : MonoBehaviour
{
    public float powerBonus = 0f;
    public Properties property = Properties.Empty;

    public void Setup(float _powerBonus = 0f, Properties _property = Properties.Empty){
        powerBonus = _powerBonus;
        property = Properties.Empty;
    }

    public float GetNewAmount(float amount){
        return (powerBonus + 1)* amount;
    }
}
