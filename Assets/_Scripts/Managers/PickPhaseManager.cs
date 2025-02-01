using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;
using System.Linq;

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
    [SerializeField]
    private Canvas informationPanelPrefab;
    private Canvas informationPanel;
    private BaseUnit selectedUnit;

    private Vector3 currentSelectorPosition;
    private Vector3 targetSelectorPosition;

    private static System.Random rng;

    [SerializeField]
    int howManyRewards = 3;

    // Start is called before the first frame update
    void Start()
    {
        cam = GlobalManager.Instance.GetCam();
        rewardParent = new GameObject("Rewards");
        currentSelectionIndex = 0;
        rewardSelector = Instantiate(rewardSelectorPrefab);
        rewardSelector.gameObject.SetActive(false);
        informationPanel = Instantiate(informationPanelPrefab);
        informationPanel.worldCamera = GlobalManager.Instance.GetCam();
        ResetDisplay();
        selectedUnit = GlobalManager.Instance.GetAllies()[0];

        List<Reward> rewardsToSpawn = new List<Reward>();
        //TODO Génération de Rewards
        for (int i = 0; i < howManyRewards; i++)
        {
            if(GlobalManager.Instance.debug){
                if(TestScript.Instance.rewardsToSpawn.Count > 0){
                    rewardsToSpawn.Add(GenerateReward(TestScript.Instance.rewardsToSpawn[0]));
                    TestScript.Instance.rewardsToSpawn.RemoveAt(0);
                }
                else{
                    rewardsToSpawn.Add(GenerateReward(new List<RewardType>{RewardType.SPELL, RewardType.PASSIVE}[rng.Next(2)]));
                }
            }
            else{
                rewardsToSpawn.Add(GenerateReward(new List<RewardType>{RewardType.SPELL, RewardType.PASSIVE}[rng.Next(2)]));
            }
        }
        SetCurrentRewards(rewardsToSpawn);

        DisplayRewards();
    }

    public void End(){
        currentRewards = new List<Reward>();
        currentSelectionIndex = 0;
        Destroy(rewardParent);
        Destroy(rewardSelector.gameObject);
        Destroy(informationPanel.gameObject);
    }

    void Awake(){
        Instance = this;
        rng = new System.Random();
    }

    // Update is called once per frame
    void Update()
    {
        currentSelectorPosition = Vector3.Lerp(currentSelectorPosition, targetSelectorPosition, Time.deltaTime*6);
        rewardSelector.position = currentSelectorPosition;
        if(currentRewards != null){
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
    }

    public void SetResourceManager(ResourceManager _resourceManager){
        resourceManager = _resourceManager;
    }

    public Reward GenerateReward(RewardType rewardType){
        //ADD Reward here
        if(rewardType == RewardType.SPELL){
            List<ScriptableSpell> spellList = resourceManager.GetSpells(lootable:true);
            spellList = spellList.Where(_spell => !(selectedUnit.HasSpell(_spell))).OrderBy(_ => rng.Next()).ToList();
            ScriptableSpell spell = spellList[0];
            return new SpellReward(spell);
        }
        if(rewardType == RewardType.PASSIVE){
            List<ScriptablePassive> passiveList = resourceManager.GetPassives(lootable:true);
            passiveList = passiveList.Where(_passive => !(selectedUnit.HasPassive(_passive))).OrderBy(_ => rng.Next()).ToList();
            ScriptablePassive passive = passiveList[0];
            return new PassiveReward(passive);
        }
        
        return new Reward();
    }

    public Reward GenerateReward(ScriptableObject scriptableObject){
        if(scriptableObject is ScriptablePassive){
            return new PassiveReward((ScriptablePassive)scriptableObject);
        }
        if(scriptableObject is ScriptableSpell){
            return new SpellReward((ScriptableSpell)scriptableObject);
        }
        return null;
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

            //ADD Reward here
            if(reward is SpellReward){
                SpellReward spellReward = (SpellReward)reward;
                _object.GetComponent<SpriteRenderer>().sprite = spellReward.GetSpell().artwork;
            }
            else if(reward is PassiveReward){
                PassiveReward passiveReward = (PassiveReward)reward;
                _object.GetComponent<SpriteRenderer>().sprite = passiveReward.GetPassive().artwork;
            }

        }
        rewardSelector.gameObject.SetActive(true);
        currentSelectorPosition = new Vector3(rewards[0].GetCell().transform.position.x, rewards[0].GetCell().transform.position.y, (float)-9.5);
        targetSelectorPosition = currentSelectorPosition;
        UpdateDisplay();
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
        UpdateDisplay();
    }


    public void SelectReward(Reward reward){
        if(reward.GetCell() != null){
            targetSelectorPosition = new Vector3(reward.GetCell().transform.position.x, reward.GetCell().transform.position.y, (float)-9.5);
        }
        else{
            Debug.Log("Pas censé arriver ici");
        }
        
    }

    public void PickReward(Reward reward){
        if(selectedUnit.GetAvailableSpellIndex() == -1){
            // 
        }
        else{
            reward.Pick(selectedUnit);
        }
    }

    private void UnselectReward(Reward reward){
        //
    }

    public void UpdateDisplay(){
        ResetDisplay();
        if(currentRewards.Count > currentSelectionIndex){
            //ADD Reward here
            if(currentRewards[currentSelectionIndex] is SpellReward){
                // Devrait idéalement fonctionner avec un BaseSpell et non un ScriptableSpell
                ScriptableSpell spell = ((SpellReward)currentRewards[currentSelectionIndex]).GetSpell();
                informationPanel.gameObject.SetActive(true);
                Transform spellPanel = informationPanel.transform.Find("SpellPanel");
                spellPanel.gameObject.SetActive(true);
                spellPanel.Find("Name").GetComponent<TextMeshProUGUI>().text = spell.name;
                spellPanel.Find("Description").GetComponent<TextMeshProUGUI>().text = spell.fight_description;
                spellPanel.Find("Cooldown").GetComponent<TextMeshProUGUI>().text = spell.cooldown.ToString();
                spellPanel.Find("SpellPanelIcon").GetComponent<Image>().sprite = spell.artwork;
            }
            else if(currentRewards[currentSelectionIndex] is PassiveReward){
                // Devrait idéalement fonctionner avec un Passive et non un ScriptablePassive
                ScriptablePassive passive = ((PassiveReward)currentRewards[currentSelectionIndex]).GetPassive();
                informationPanel.gameObject.SetActive(true);
                Transform spellPanel = informationPanel.transform.Find("SpellPanel");
                spellPanel.gameObject.SetActive(true);
                spellPanel.Find("Name").GetComponent<TextMeshProUGUI>().text = passive.name;
                spellPanel.Find("Description").GetComponent<TextMeshProUGUI>().text = passive.fight_description;
                spellPanel.Find("Cooldown").GetComponent<TextMeshProUGUI>().text = "";
                spellPanel.Find("SpellPanelIcon").GetComponent<Image>().sprite = passive.artwork;
            }
            else{
                informationPanel.gameObject.SetActive(false);
            }
        }
        else{
            informationPanel.gameObject.SetActive(false);
        }
    }

    private void ResetDisplay(){
            informationPanel.transform.Find("InfosPanel").gameObject.SetActive(false);
            informationPanel.transform.Find("SpellPanel").gameObject.SetActive(false);
            informationPanel.transform.Find("SpellSelector").gameObject.SetActive(false);
    }
}
