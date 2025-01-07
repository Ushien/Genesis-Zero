using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reward
{
        public GameObject cell;

        virtual public void Pick(BaseUnit unit){
                Debug.Log("Doit être overridé");
        }

        virtual public string GetTitle(){
                Debug.Log("Doit être overridé");
                return "";
        }

        public void SetCell(GameObject _cell){
                cell = _cell;
        }

        public GameObject GetCell(){
                return cell;
        }
}

