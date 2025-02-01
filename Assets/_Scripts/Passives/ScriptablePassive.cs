using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Scriptable Passive", menuName = "ScriptablePassive")]
public class ScriptablePassive : ScriptableObject
{
    public string passive_name;
    [TextArea(5,10)]
    public string fight_description;
    public float[] ratios = {1f, 1f, 1f};
    public Sprite artwork;
    public Passive passivePrefab;
    public bool lootable;

    public void SetupPassive(BaseUnit unit){
        Passive passive = Instantiate(passivePrefab);
        passive.Setup(unit);
    }
}
