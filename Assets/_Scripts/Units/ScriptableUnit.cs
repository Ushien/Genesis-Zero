using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Unit", menuName = "Unit")]

public class ScriptableUnit : ScriptableObject
{
    public string unit_name;

    public Sprite sprite;
    public int original_power;
    public int original_health;
    [TextArea(5,10)]
    public string lore_description;
    [TextArea(5,10)]
    public string fight_description;

    public Passive passive;
    public List<ScriptableSpell> spells;
    
    public ScriptableJob scriptableJob = null;
}