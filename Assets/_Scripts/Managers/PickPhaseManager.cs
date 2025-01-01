using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class PickPhaseManager : MonoBehaviour
{
    public static PickPhaseManager Instance;
    
    [SerializeField]
    private GameObject choiceCell;
    public enum RewardType{EMPTY, PASSIVE, SPELL}
    private UnityEngine.Object[] spellList;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        spellList = Resources.LoadAll("Spells", typeof(ScriptableSpell));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Reward GenerateReward(RewardType rewardType){
        if(rewardType == RewardType.SPELL){
            ScriptableSpell spell = (ScriptableSpell)spellList[UnityEngine.Random.Range(0, spellList.Length)];
        }
        return new Reward();
    }


    private void DisplayRewards(List<Reward> rewards){
        foreach (Reward reward in rewards)
        {
            Instantiate(choiceCell);
        }

    }
}
