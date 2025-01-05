using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class PickPhaseManager : MonoBehaviour
{
    public static PickPhaseManager Instance;
    private ResourceManager resourceManager;
    private Camera cam;
    
    [SerializeField]
    private GameObject choiceCell;
    public enum RewardType{EMPTY, PASSIVE, SPELL}

    // Start is called before the first frame update
    void Start()
    {
        cam = GlobalManager.Instance.GetCam();
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
        int x_pos = 0;
        foreach (Reward reward in rewards)
        {
            GameObject _object = Instantiate(choiceCell);
            _object.transform.name = reward.GetTitle();
            x_pos = x_pos + Screen.width/(rewards.Count+1);
            _object.transform.position = cam.ScreenToWorldPoint(new Vector3(x_pos, Screen.height/2, 1));
            if(reward is SpellReward){
                SpellReward spellReward = (SpellReward)reward;
                _object.GetComponent<SpriteRenderer>().sprite = spellReward.GetSpell().artwork;
            }

        }

    }
}
