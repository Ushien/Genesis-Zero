using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class PickPhaseManager : MonoBehaviour
{
    public static PickPhaseManager Instance;
    private ResourceManager resourceManager;
    
    [SerializeField]
    private GameObject choiceCell;
    public enum RewardType{EMPTY, PASSIVE, SPELL}

    // Start is called before the first frame update
    void Start()
    {

    }

    void Awake(){
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetResourceManager(ResourceManager _resourceManager){
        resourceManager = _resourceManager;
    }

    public Reward GenerateReward(RewardType rewardType){
        if(rewardType == RewardType.SPELL){
            ScriptableSpell spell = (ScriptableSpell)resourceManager.GetSpells()[UnityEngine.Random.Range(0, resourceManager.Instance.GetSpells().Length)];
            return new SpellReward(spell);
        }
        
        return new Reward();
    }


    public void DisplayRewards(List<Reward> rewards){
        foreach (Reward reward in rewards)
        {
            GameObject _object = Instantiate(choiceCell);
            _object.transform.name = reward.GetTitle();
        }

    }
}
