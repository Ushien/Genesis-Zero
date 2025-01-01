using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public ResourceManager Instance;
    private UnityEngine.Object[] spellList;

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
    }

    public void LoadResources(){
        spellList = Resources.LoadAll("Spells", typeof(ScriptableSpell));
    }

    public UnityEngine.Object[] GetSpells(){
        return spellList;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
