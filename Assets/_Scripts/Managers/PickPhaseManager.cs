using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;
using System.Linq;
using Unity.VisualScripting;

public class PickPhaseManager : MonoBehaviour
{
    public static PickPhaseManager Instance;
    private ResourceManager resourceManager;
    private Camera cam;
    private GameObject rewardParent;
    [SerializeField]
    private Transform rewardSelectorPrefab;
    private Transform rewardSelector;
    private Transform unitSelector;
    
    [SerializeField]
    private GameObject choiceCell;
    private List<Reward> currentRewards;
    private List<GameObject> alliesCells;
    [SerializeField]
    private List<BaseUnit> allies;
    private int currentSelectionIndex;
    private int currentUnitIndex;
    public enum RewardType{EMPTY, PASSIVE, SPELL}
    public enum Directions{NONE, LEFT, RIGHT,UP, DOWN}
    [SerializeField]
    private Canvas informationPanelPrefab;
    private Canvas informationPanel;
    private Vector3 currentRewardSelectorPosition;
    private Vector3 targetSelectorPosition;
    private Vector3 currentUnitSelectorPosition;
    private Vector3 targetUnitPosition;

    [SerializeField]
    int howManyRewards = 3;

    // Start is called before the first frame update
    void Start()
    {
        cam = GlobalManager.Instance.GetCam();
        cam.transform.GetChild(0).gameObject.SetActive(true);
        rewardParent = new GameObject("Rewards");
        allies = GlobalManager.Instance.GetAllies();
        alliesCells = new List<GameObject>();

        currentSelectionIndex = 0;
        rewardSelector = Instantiate(rewardSelectorPrefab);
        rewardSelector.gameObject.SetActive(false);

        currentUnitIndex = 0;
        unitSelector = Instantiate(rewardSelectorPrefab);
        unitSelector.gameObject.SetActive(false);

        informationPanel = Instantiate(informationPanelPrefab);
        informationPanel.worldCamera = GlobalManager.Instance.GetCam();
        ResetDisplay();

        List<Reward> rewardsToSpawn = new List<Reward>();
        Reward rewardToAdd;
        //TODO Génération de Rewards
        for (int i = 0; i < howManyRewards; i++)
        {
            if(GlobalManager.Instance.debug){
                if(TestScript.Instance.rewardsToSpawn.Count > 0){
                    rewardToAdd = GenerateReward(TestScript.Instance.rewardsToSpawn[0]);
                    TestScript.Instance.rewardsToSpawn.RemoveAt(0);
                }
                else{
                    rewardToAdd = GenerateReward(new List<RewardType>{RewardType.SPELL, RewardType.PASSIVE}[UnityEngine.Random.Range(0, 2)]);
                }
            }
            else{
                rewardToAdd = GenerateReward(new List<RewardType>{RewardType.SPELL, RewardType.PASSIVE}[UnityEngine.Random.Range(0, 2)]);
            }
            rewardsToSpawn.Add(rewardToAdd);            
        }
        SetCurrentRewards(rewardsToSpawn);

        int y_pos = 0;
        for (int i = allies.Count-1; i >= 0; i--)
        {
            GameObject _object = Instantiate(choiceCell);
            alliesCells.Insert(0, _object);
            
            _object.transform.name = allies[i].GetName();
            _object.GetComponent<SpriteRenderer>().sprite = allies[i].GetComponent<SpriteRenderer>().sprite;

            y_pos = y_pos + Screen.height/2/(allies.Count+1);
            _object.transform.position = cam.ScreenToWorldPoint(new Vector3(Screen.width/7, y_pos, 1));
            //_object.transform.SetParent(rewardParent.transform);
        }
        unitSelector.gameObject.SetActive(true);
        currentUnitSelectorPosition = alliesCells[currentUnitIndex].transform.position;
        targetUnitPosition = currentUnitSelectorPosition;

        DisplayRewards();

        if(GlobalManager.Instance.debug && TestScript.Instance.AreThereScriptedPicks()){
            Vector2 pick = TestScript.Instance.GetScriptedPicks();
            if(pick != new Vector2(-1, -1)){
                PickReward((int) pick[0], (int) pick[1]);
                GlobalManager.Instance.ChangeState(GlobalManager.RunPhase.BATTLEPHASE);
            }
        }
    }

    public void End(){
        currentRewards = new List<Reward>();
        currentSelectionIndex = 0;
        Destroy(rewardParent);
        Destroy(rewardSelector.gameObject);
        Destroy(informationPanel.gameObject);
        Destroy(unitSelector.gameObject);
        foreach (GameObject unitCell in alliesCells)
        {
            Destroy(unitCell);
        }
        // Active blank background
        // TODO il faut changer ce fonctionnement
        cam.transform.GetChild(0).gameObject.SetActive(false);
    }

    void Awake(){
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        currentRewardSelectorPosition = Vector3.Lerp(currentRewardSelectorPosition, targetSelectorPosition, Time.deltaTime*6);
        rewardSelector.position = currentRewardSelectorPosition;

        currentUnitSelectorPosition = Vector3.Lerp(currentUnitSelectorPosition, targetUnitPosition, Time.deltaTime*6);
        unitSelector.position = currentUnitSelectorPosition;

        if(currentRewards != null){
            if(Input.GetKeyDown(KeyCode.LeftArrow)){
                Move(Directions.LEFT);
            }
            if(Input.GetKeyDown(KeyCode.RightArrow)){
                Move(Directions.RIGHT);
            }
            if(Input.GetKeyDown(KeyCode.UpArrow)){
                Move(Directions.UP);
            }
            if(Input.GetKeyDown(KeyCode.DownArrow)){
                Move(Directions.DOWN);
            }
            if(Input.GetKeyDown(KeyCode.B)){
                if(currentRewards.Count > 0){
                    PickReward(currentRewards[currentSelectionIndex], allies[currentUnitIndex]);
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
            foreach (BaseUnit ally in allies)
            {
                spellList = spellList.Where(_spell => !(ally.HasSpell(_spell))).OrderBy(_ => UnityEngine.Random.value).ToList();      
            }
            ScriptableSpell spell = spellList[0];
            return new SpellReward(spell);
        }
        if(rewardType == RewardType.PASSIVE){
            List<ScriptablePassive> passiveList = resourceManager.GetPassives(lootable:true);
            // Filtre les passifs déjà possédés par une unité
            foreach (BaseUnit ally in allies)
            {
                passiveList = passiveList.Where(_passive => !(ally.HasPassive(_passive))).OrderBy(_ => UnityEngine.Random.value).ToList();
            }
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
        currentRewardSelectorPosition = new Vector3(rewards[0].GetCell().transform.position.x, rewards[0].GetCell().transform.position.y, (float)-9.5);
        targetSelectorPosition = currentRewardSelectorPosition;
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
                    UpdateDisplay();
                }
                break;
            case Directions.RIGHT:
                if(currentSelectionIndex != GetCurrentRewards().Count-1){
                    UnselectReward(GetCurrentRewards()[currentSelectionIndex]);
                    currentSelectionIndex = currentSelectionIndex + 1;
                    SelectReward(GetCurrentRewards()[currentSelectionIndex]);
                    UpdateDisplay();
                }
                break;
            case Directions.UP:
                if(currentUnitIndex != 0){
                    //UnselectUnit()
                    currentUnitIndex -= 1;
                    SelectNewUnit(currentUnitIndex);
                    UpdateDisplay();
                }
                break;
            case Directions.DOWN:
                if(currentUnitIndex != allies.Count-1){
                    currentUnitIndex += 1;
                    SelectNewUnit(currentUnitIndex);
                    UpdateDisplay();
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

    private void SelectNewUnit(int unitIndex){
        targetUnitPosition = new Vector3(alliesCells[unitIndex].transform.position.x, alliesCells[unitIndex].transform.position.y, (float)-9.5);
    }

    public void PickReward(Reward reward, BaseUnit unit){
        if(reward is SpellReward && allies[currentUnitIndex].GetAvailableSpellIndex() == -1){
            // 
        }
        else{
            TestScript.Instance.Log("P:"+currentSelectionIndex+"-"+currentUnitIndex);
            reward.Pick(unit);
            AnimationManager.Instance.ForceAnimation();
        }
    }

    public void PickReward(int rewardIndex, int unitIndex){
        if(allies[currentUnitIndex].GetAvailableSpellIndex() == -1){
            // 
        }
        else{
            currentRewards[rewardIndex].Pick(allies[unitIndex]);
            AnimationManager.Instance.ForceAnimation();
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
                spellPanel.Find("Description").GetComponent<TextMeshProUGUI>().text = spell.GetFightDescription(allies[currentUnitIndex]);
                spellPanel.Find("Cooldown").GetComponent<TextMeshProUGUI>().text = spell.cooldown.ToString();
                spellPanel.Find("Sprite").GetComponent<Image>().sprite = spell.artwork;
            }
            else if(currentRewards[currentSelectionIndex] is PassiveReward){
                // Devrait idéalement fonctionner avec un Passive et non un ScriptablePassive
                ScriptablePassive passive = ((PassiveReward)currentRewards[currentSelectionIndex]).GetPassive();
                informationPanel.gameObject.SetActive(true);
                Transform passivePanel = informationPanel.transform.Find("PassivePanel");
                passivePanel.gameObject.SetActive(true);
                passivePanel.Find("NextSprite").gameObject.SetActive(false);
                passivePanel.Find("Name").GetComponent<TextMeshProUGUI>().text = passive.name;
                passivePanel.Find("Description").GetComponent<TextMeshProUGUI>().text = passive.GetFightDescription(allies[currentUnitIndex]);
                passivePanel.Find("Sprite").GetComponent<Image>().sprite = passive.artwork;
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
            informationPanel.transform.Find("UnitPanel").gameObject.SetActive(false);
            informationPanel.transform.Find("PassivePanel").gameObject.SetActive(false);
            informationPanel.transform.Find("SpellPanel").gameObject.SetActive(false);
            informationPanel.transform.Find("SpellSelector").gameObject.SetActive(false);
    }
}
