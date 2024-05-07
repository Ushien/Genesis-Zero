using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Modifier : MonoBehaviour
{
    public int powerBonus = 0;
    public Properties property = Properties.Empty;

    public void Setup(int _powerBonus = 0, Properties _property = Properties.Empty){
        powerBonus = _powerBonus;
        property = Properties.Empty;
    }

    public int GetNewAmount(int amount){
        return (int)(((float) powerBonus + 100)/100 * amount);
    }
}
