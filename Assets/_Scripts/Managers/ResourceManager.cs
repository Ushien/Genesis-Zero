using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;

public class ResourceManager : MonoBehaviour
{
    public ResourceManager Instance;
    private List<ScriptableSpell> spellList;
    private List<ScriptablePassive> passiveList;
    private List<ScriptableUnit> enemyUnitList;
    private List<ScriptableComposition> enemyCompositionList;

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
    }

    public void LoadResources()
    {
        spellList = Resources.LoadAll("Spells", typeof(ScriptableSpell)).Cast<ScriptableSpell>().ToList();
        passiveList = Resources.LoadAll("Passives", typeof(ScriptablePassive)).Cast<ScriptablePassive>().ToList();
        enemyUnitList = Resources.LoadAll<ScriptableUnit>("Units/Enemies").ToList();
        enemyCompositionList = Resources.LoadAll<ScriptableComposition>("Teams/Enemies").ToList();
        PreloadAllLocalization();
    }

    public static void PreloadAllLocalization()
    {
        LocalizationSettings.StringDatabase.GetTable("Spells");
        Debug.Log("Chargement des tables effectué");
    }

    public List<ScriptableSpell> GetSpells(bool lootable = true)
    {
        if (lootable)
        {
            return spellList.Where(spell => spell.lootable == lootable).ToList();
        }
        return spellList;
    }

    public List<ScriptablePassive> GetPassives(bool lootable = true){
        if(lootable){
            return passiveList.Where(passive => passive.lootable == lootable).ToList();
        }
        return passiveList;
    }

    public List<ScriptableUnit> GetEnemyUnit(){
        return enemyUnitList;
    }

    public List<ScriptableComposition> GetEnemyCompositions(bool lootable = true){
        return enemyCompositionList;
    }
}
