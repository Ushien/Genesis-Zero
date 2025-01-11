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
    private GameObject rewardParent;
    [SerializeField]
    private Transform rewardSelectorPrefab;
    private Transform rewardSelector;
    
    [SerializeField]
    private GameObject choiceCell;
    private List<Reward> currentRewards;
    private int currentSelectionIndex;
    public enum RewardType{EMPTY, PASSIVE, SPELL}
    public enum Directions{NONE, LEFT, RIGHT}

    private Vector3 currentSelectorPosition;
    private Vector3 targetSelectorPosition;

    // Start is called before the first frame update
    void Start()
    {
        cam = GlobalManager.Instance.GetCam();
        rewardParent = new GameObject("Rewards");
        currentSelectionIndex = 0;
        rewardSelector = Instantiate(rewardSelectorPrefab);
        rewardSelector.gameObject.SetActive(false);
    }

    public void End(){
        currentRewards = new List<Reward>();
        currentSelectionIndex = 0;
        Destroy(rewardParent);
        Destroy(rewardSelector.gameObject);
    }

    void Awake(){
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        currentSelectorPosition = Vector3.Lerp(currentSelectorPosition, targetSelectorPosition, Time.deltaTime*6);
        rewardSelector.position = currentSelectorPosition;
        if(Input.GetKeyDown(KeyCode.LeftArrow)){
            Move(Directions.LEFT);
        }
        if(Input.GetKeyDown(KeyCode.RightArrow)){
            Move(Directions.RIGHT);
        }
        if(Input.GetKeyDown(KeyCode.B)){
            if(currentRewards.Count > 0){
                PickReward(currentRewards[currentSelectionIndex]);
                GlobalManager.Instance.ChangeState(GlobalManager.RunPhase.BATTLEPHASE);
            }
        }
    }

    public void SetResourceManager(ResourceManager _resourceManager){
        resourceManager = _resourceManager;
    }

    public Reward GenerateReward(RewardType rewardType){
        if(rewardType == RewardType.SPELL){
            List<ScriptableSpell> spellList = resourceManager.GetSpells(lootable:true);
            ScriptableSpell spell = spellList[UnityEngine.Random.Range(0, spellList.Count)];
            return new SpellReward(spell);
        }
        
        return new Reward();
    }

    public void SetCurrentRewards(List<Reward> rewards){
        currentRewards = rewards;
    }

    public List<Reward> GetCurrentRewards(){
        return currentRewards;
    }

    public void DisplayRewards(){
        List<Reward> rewards = GetCurrentRewards();
        int x_pos = 0;
        foreach (Reward reward in rewards)
        {
            GameObject _object = Instantiate(choiceCell);
            reward.SetCell(_object);
            _object.transform.name = reward.GetTitle();
            x_pos = x_pos + Screen.width/(rewards.Count+1);
            _object.transform.position = cam.ScreenToWorldPoint(new Vector3(x_pos, Screen.height/2, 1));
            _object.transform.SetParent(rewardParent.transform);
            if(reward is SpellReward){
                SpellReward spellReward = (SpellReward)reward;
                _object.GetComponent<SpriteRenderer>().sprite = spellReward.GetSpell().artwork;
            }

        }
        rewardSelector.gameObject.SetActive(true);
        currentSelectorPosition = rewards[0].GetCell().transform.position;
        targetSelectorPosition = rewards[0].GetCell().transform.position;

    }

    public void Move(Directions direction){
        switch (direction)
        {
            case Directions.LEFT:
                if(currentSelectionIndex != 0){
                    UnselectReward(GetCurrentRewards()[currentSelectionIndex]);
                    currentSelectionIndex = currentSelectionIndex-1;
                    SelectReward(GetCurrentRewards()[currentSelectionIndex]);
                }
                break;
            case Directions.RIGHT:
                if(currentSelectionIndex != GetCurrentRewards().Count-1){
                    UnselectReward(GetCurrentRewards()[currentSelectionIndex]);
                    currentSelectionIndex = currentSelectionIndex + 1;
                    SelectReward(GetCurrentRewards()[currentSelectionIndex]);
                }
                break;
            default:
                break;
        }
    }


    public void SelectReward(Reward reward){
        if(reward.GetCell() != null){
            targetSelectorPosition = reward.GetCell().transform.position;
        }
        else{
            Debug.Log("Pas censé arriver ici");
        }
        
    }

    public void PickReward(Reward reward){
        reward.Pick(GlobalManager.Instance.GetAllies()[0]);
    }

    private void UnselectReward(Reward reward){
        //
    }
}
