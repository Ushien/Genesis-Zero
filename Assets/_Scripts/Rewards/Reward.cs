using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reward
{
        virtual public string GetTitle(){
                Debug.Log("Doit être overridé");
                return "";
        }
}

