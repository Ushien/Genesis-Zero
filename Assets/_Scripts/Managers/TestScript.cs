using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TestScript : MonoBehaviour
{
    public ScriptableComposition enemy_composition;

    void Start()
    {
        var selected_spell = SpellManager.Instance.GetRandomSpell();
        SpellManager.Instance.SetupSpell(selected_spell);
        BattleManager.Instance.LaunchBattle();
    }
    
}
