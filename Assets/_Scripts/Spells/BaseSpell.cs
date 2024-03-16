using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseSpell : MonoBehaviour
{
    public ScriptableSpell scriptableSpell;

    public string spell_name = "Name";
    [TextArea(5,10)]
    public string description = "Description";

    public void Setup(ScriptableSpell originSpell){
        scriptableSpell = originSpell;
        spell_name = scriptableSpell.spell_name;
        description = scriptableSpell.description;
    }
}
