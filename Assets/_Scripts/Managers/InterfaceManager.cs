using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using System.Linq;
using UnityEngine.Assertions;

/// <summary>
/// Gestion de l'interface de jeu
/// </summary>

public class InterfaceManager : MonoBehaviour
{
    #region Fields
    public static InterfaceManager Instance;

    // UI elements
    [SerializeField]
    private Canvas UIPrefab;
    private Canvas UI;

    private GameObject infosPanel;
    private GameObject spellPanel;
    private TextMeshProUGUI unitNamePanel;
    private TextMeshProUGUI unitPowerPanel;
    private TextMeshProUGUI unitHealthPanel;
    private TextMeshProUGUI unitArmorPanel;
    private TextMeshProUGUI unitLevelPanel;
    private TextMeshProUGUI unitPassiveNamePanel;
    private TextMeshProUGUI unitPassiveDescriptionPanel;
    private TextMeshProUGUI spellNamePanel;
    private TextMeshProUGUI spellCooldownPanel;
    private TextMeshProUGUI spellDescriptionPanel;
    public RectTransform spellPanelLine;
    public RectTransform spellSelectorLine;
    private Image spellPanelIcon;
    private GameObject spellSelector;
    public GameObject shade;
    public Canvas UIWorldSpace;
    private Canvas lifeBarsUI;
    public GameObject lifeBarPrefab;
    public Material grayscaleShader;
    public Sprite emptySpellSelectorSquare;
   

    private GameObject tileSelector;
    private Vector3 tileSelector_targetPos;
    private Vector3 tileSelector_currentPos;

    public Vector3 lifeBarOffset;

    [SerializeField]
    private float selectorSpeed;

    [SerializeField]
    private Material tileOutliner;
    public Camera mainCamera; // Utile pour convertir des position in game à des positions en pixels sur l'écran

    public float tileSize = 250f; // A bit ugly but still good for now

    // La Tile contenant la source du spell, lorsqu'un spell est lancé (lorsqu'on revient en arrière pendant la sélection de cible)
    private Tile sourceTile;

    private enum SpellChoice{CHARACTER, LEFT, RIGHT, UP, DOWN}
    private SpellChoice spellChoice;

    [SerializeField]
    private bool overloaded = false;

    // Le spell pour lequel on va sélectionner une cible
    private BaseSpell selectedSpell;

    // La Tile contenant la cible du spell, lorsqu'un spell est lancé
    public Tile targetTile;

    private Dictionary<BattleManager.PlayerActionChoiceState, bool> activated_states;

    #endregion

    void Awake(){
        Instance = this;

        // On initialise la UI
        UI = Instantiate(UIPrefab);
        lifeBarsUI = GlobalManager.Instance.GetUIWorldSpace();
        UI.worldCamera = GlobalManager.Instance.GetCam();

        infosPanel = UI.transform.Find("InfosPanel").gameObject;
        spellPanel = UI.transform.Find("SpellPanel").gameObject;
        spellSelector = UI.transform.Find("SpellSelector").gameObject;
        shade = UI.transform.Find("Shade").gameObject;

        unitNamePanel = infosPanel.transform.Find("InformationPanel").transform.Find("UnitName").GetComponent<TextMeshProUGUI>();
        unitPowerPanel = infosPanel.transform.Find("InformationPanel").transform.Find("UnitPower").GetComponent<TextMeshProUGUI>();
        unitHealthPanel = infosPanel.transform.Find("InformationPanel").transform.Find("UnitHealth").GetComponent<TextMeshProUGUI>();
        unitArmorPanel = infosPanel.transform.Find("InformationPanel").transform.Find("UnitArmor").GetComponent<TextMeshProUGUI>();
        unitLevelPanel = infosPanel.transform.Find("InformationPanel").transform.Find("UnitLevel").GetComponent<TextMeshProUGUI>();

        unitPassiveNamePanel = infosPanel.transform.Find("PassivePanel").transform.Find("Name").GetComponent<TextMeshProUGUI>();
        unitPassiveDescriptionPanel = infosPanel.transform.Find("PassivePanel").transform.Find("Description").GetComponent<TextMeshProUGUI>();

        spellNamePanel = spellPanel.transform.Find("Name").GetComponent<TextMeshProUGUI>();
        spellCooldownPanel = spellPanel.transform.Find("Cooldown").GetComponent<TextMeshProUGUI>();
        spellDescriptionPanel = spellPanel.transform.Find("Description").GetComponent<TextMeshProUGUI>();
        spellPanelLine = spellPanel.transform.Find("SpellPanelLine").GetComponent<RectTransform>();
        spellPanelIcon = spellPanel.transform.Find("SpellPanelIcon").GetComponent<Image>();

        spellSelectorLine = spellSelector.transform.Find("SpellSelectorLine").GetComponent<RectTransform>();

        mainCamera = GlobalManager.Instance.GetCam();

        activated_states = new Dictionary<BattleManager.PlayerActionChoiceState, bool>();
        foreach (BattleManager.PlayerActionChoiceState state in System.Enum.GetValues(typeof(BattleManager.PlayerActionChoiceState)))
        {
            activated_states[state] = false;
        }
    }

    void Start(){

        // On crée le tileSelector qui va naviguer pour la sélection des cases

        tileSelector = Instantiate(GridManager.Instance.GetTilePrefab()).gameObject;
        Material material = Instantiate(tileOutliner);

        tileSelector.transform.GetComponent<UnityEngine.SpriteRenderer>().material = material;
        tileSelector.transform.GetComponent<UnityEngine.SpriteRenderer>().sortingOrder = 2;
        tileSelector.transform.parent = UI.transform;
        tileSelector.name = "Tile Selector";
        tileSelector.transform.DetachChildren();

        tileSelector_targetPos = tileSelector.transform.position;
        tileSelector_currentPos = tileSelector.transform.position;
    }

    void Update()
    {   
        if(BattleManager.Instance != null){
            switch (BattleManager.Instance.GetPlayerActionChoiceState())
            {
                case BattleManager.PlayerActionChoiceState.CHARACTER_SELECTION:
                    SourceSelectionDisplay();
                    break;
                case BattleManager.PlayerActionChoiceState.SPELL_SELECTION:
                    SpellSelectionDisplay();
                    break;
                case BattleManager.PlayerActionChoiceState.TARGET_SELECTION:
                    TargetSelectionDisplay();
                    break;
                default:
                    break;
            }   
        }

        
    }

    void SourceSelectionDisplay(){
        if(!activated_states[BattleManager.PlayerActionChoiceState.CHARACTER_SELECTION]){
            // Just changed from another state
            // Reset view
            ResetDisplay();

            // Activate the needed interface
            infosPanel.SetActive(true);
            tileSelector.SetActive(true);

            if(UnitManager.Instance.GetUnits(Team.Ally).Count > 0){
                sourceTile = UnitManager.Instance.GetUnits(Team.Ally).Where(_unit => !_unit.HasGivenInstruction()).First().GetTile();
            }
            else{
                // On prend une case au hasard je ne comprends pas pourquoi il crash si je ne lui en donne pas
                sourceTile = GridManager.Instance.GetRandomTile(Team.Both);
            }

            sourceTile.Select();
            GridManager.Instance.SetSelectionMode(GridManager.Selection_mode.Single_selection);

            ActivateState(BattleManager.PlayerActionChoiceState.CHARACTER_SELECTION);

        }

        // Change la position du sélector de case à l'aide d'un joli lerp
        tileSelector_targetPos = sourceTile.transform.position;
        tileSelector_currentPos = Vector3.Lerp(tileSelector_currentPos, tileSelector_targetPos, Time.deltaTime*selectorSpeed);
        tileSelector.transform.position = tileSelector_currentPos;

        //GridManager.Instance.DisplayHighlights();

        if (Input.GetKeyDown(KeyCode.B)){
            if(sourceTile.GetUnit()!= null){
                if(sourceTile.GetUnit().GetTeam() == Team.Ally && !sourceTile.GetUnit().HasGivenInstruction()){
                    SourceSelectionTrigger(BattleManager.Trigger.VALIDATE);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.N)){
            SourceSelectionTrigger(BattleManager.Trigger.CANCEL);
        }
        if (Input.GetKeyDown(KeyCode.UpArrow)){
            if(sourceTile.GetNextTile(Directions.UP) != null){
                sourceTile.GetNextTile(Directions.UP).Select();
                sourceTile.Unselect();
                sourceTile = sourceTile.GetNextTile(Directions.UP);
            }
        }
        if (Input.GetKeyDown(KeyCode.DownArrow)){
            if(sourceTile.GetNextTile(Directions.DOWN) != null){
                sourceTile.GetNextTile(Directions.DOWN).Select();
                sourceTile.Unselect();
                sourceTile = sourceTile.GetNextTile(Directions.DOWN);
            }
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow)){
            if(sourceTile.GetNextTile(Directions.LEFT) != null){
                sourceTile.GetNextTile(Directions.LEFT).Select();
                sourceTile.Unselect();
                sourceTile = sourceTile.GetNextTile(Directions.LEFT);
            }
        }
        if (Input.GetKeyDown(KeyCode.RightArrow)){
            if(sourceTile.GetNextTile(Directions.RIGHT) != null){
                sourceTile.GetNextTile(Directions.RIGHT).Select();
                sourceTile.Unselect();
                sourceTile = sourceTile.GetNextTile(Directions.RIGHT);
            }
        }

        BaseUnit currentUnit = sourceTile.GetUnit();
        if(currentUnit != null){
            infosPanel.SetActive(true);
            DisplayUnit(currentUnit);
            //DrawPanelLine(infosPanelLine, sourceTile);
        }
        else{
            infosPanel.SetActive(false);
        }
    }
    void SourceSelectionTrigger(BattleManager.Trigger trigger){
        // On sort de la sélection de la source pour aller vers un autre état
        BattleManager.Instance.ChangeState(BattleManager.Machine.PLAYERACTIONCHOICESTATE, trigger);
    }

    void SpellSelectionDisplay(){
        // TODO Ne pas afficher les 4 cases si le personnage n'a pas 4 spells
        BaseUnit sourceUnit = sourceTile.GetUnit();
        BaseSpell[] currentSpells = sourceUnit.GetSpells();

        if(!activated_states[BattleManager.PlayerActionChoiceState.SPELL_SELECTION]){
            // Just changed from another state

            // Reset view
            ResetDisplay();

            // Activate the needed interface
            spellSelector.SetActive(true);
            shade.SetActive(true);
            infosPanel.SetActive(true);
            spellPanel.SetActive(true);
            spellPanelLine.gameObject.SetActive(false);

            //spellSelector.transform.position = sourceTile.transform.position;
            DrawPanelLine(spellSelectorLine, sourceTile);

            int currentSpellIndex = 0;

            foreach (BaseSpell spell in currentSpells)
            {
                if(spell != null){
                    spellSelector.transform.GetChild(currentSpellIndex).GetComponent<UnityEngine.UI.Image>().sprite = spell.GetArtwork();
                    spellSelector.transform.GetChild(currentSpellIndex).GetComponent<UnityEngine.UI.Image>().material = null;
                    if(!spell.IsAvailable()){
                        Material material = Instantiate(grayscaleShader);
                        spellSelector.transform.GetChild(currentSpellIndex).GetComponent<UnityEngine.UI.Image>().material = material;
                        //Grey
                    }              
                }
                else{
                    spellSelector.transform.GetChild(currentSpellIndex).GetComponent<UnityEngine.UI.Image>().sprite = emptySpellSelectorSquare;
                    spellSelector.transform.GetChild(currentSpellIndex).GetComponent<UnityEngine.UI.Image>().material = null;
                }
                currentSpellIndex += 1;
            }

            GridManager.Instance.SetSelectionMode(GridManager.Selection_mode.Single_selection);
            
            ActivateState(BattleManager.PlayerActionChoiceState.SPELL_SELECTION);
        }

        //Display highlight
        spellSelector.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(false);
        spellSelector.transform.GetChild(1).transform.GetChild(0).gameObject.SetActive(false);
        spellSelector.transform.GetChild(2).transform.GetChild(0).gameObject.SetActive(false);
        spellSelector.transform.GetChild(3).transform.GetChild(0).gameObject.SetActive(false);
        spellSelector.transform.GetChild(4).transform.GetChild(0).gameObject.SetActive(false);

        switch(spellChoice){
            case SpellChoice.CHARACTER:
                spellSelector.transform.GetChild(4).transform.GetChild(0).gameObject.SetActive(true);
                AnimateSpellChoice(4);
                break;
            case SpellChoice.LEFT:
                spellSelector.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(true);
                AnimateSpellChoice(0);
                break;
            case SpellChoice.RIGHT:
                spellSelector.transform.GetChild(1).transform.GetChild(0).gameObject.SetActive(true);
                AnimateSpellChoice(1);
                break;
            case SpellChoice.UP:
                spellSelector.transform.GetChild(2).transform.GetChild(0).gameObject.SetActive(true);
                AnimateSpellChoice(2);
                break;
            case SpellChoice.DOWN:
                spellSelector.transform.GetChild(3).transform.GetChild(0).gameObject.SetActive(true);
                AnimateSpellChoice(3);
                break;
            default:
                break;
        }

        // Navigation au sein de la sélection
        switch(spellChoice){
            case SpellChoice.CHARACTER:
                overloaded = false;
                selectedSpell = sourceUnit.GetAttack();
                if (Input.GetKeyDown(KeyCode.B)){
                    // Sélectionner attaque
                    sourceTile.Unselect();
                    SpellSelectionTrigger(BattleManager.Trigger.VALIDATE);
                    break;
                }
                if (Input.GetKeyDown(KeyCode.N)){
                    // Retour à la sélection de personnages
                    sourceTile.Unselect();
                    SpellSelectionTrigger(BattleManager.Trigger.CANCEL);
                    break;
                }
                if (Input.GetKeyDown(KeyCode.UpArrow)){
                    // Aller en haut
                    spellChoice = SpellChoice.UP;
                    break;
                }
                if (Input.GetKeyDown(KeyCode.DownArrow)){
                    // Aller en bas
                    spellChoice = SpellChoice.DOWN;
                    break;
                }
                if (Input.GetKeyDown(KeyCode.LeftArrow)){
                    // Aller à gauche
                    spellChoice = SpellChoice.LEFT;
                    break;
                }
                if (Input.GetKeyDown(KeyCode.RightArrow)){
                    // Aller à droite
                    spellChoice = SpellChoice.RIGHT;
                    break;
                }
                break;
            case SpellChoice.LEFT:
                selectedSpell = currentSpells[0];
                //TODO Ce code se répète 4 fois, il y a moyen de refactor
                if (Input.GetKeyDown(KeyCode.B) && selectedSpell != null){
                    if(selectedSpell.IsAvailable()){
                        sourceTile.Unselect();
                        SpellSelectionTrigger(BattleManager.Trigger.VALIDATE);
                    }
                    break;
                }
                if (Input.GetKeyDown(KeyCode.N)){
                    // Retour au centre
                    spellChoice = SpellChoice.CHARACTER;
                    overloaded = false;
                    break;
                }
                if (Input.GetKeyDown(KeyCode.UpArrow)){
                    // Aller en haut
                    spellChoice = SpellChoice.UP;
                    overloaded = false;
                    break;
                }
                if (Input.GetKeyDown(KeyCode.DownArrow)){
                    // Aller en bas
                    spellChoice = SpellChoice.DOWN;
                    overloaded = false;
                    break;
                }
                if (Input.GetKeyDown(KeyCode.RightArrow)){
                    // Aller à droite
                    spellChoice = SpellChoice.RIGHT;
                    overloaded = false;
                    break;
                }
                if (Input.GetKeyDown(KeyCode.LeftArrow)){
                    // Passer en mode surcharge / Revenir au mode non surchargé
                    spellChoice = SpellChoice.LEFT;
                    overloaded = !overloaded;
                    break;
                }
                break;
            case SpellChoice.RIGHT:
                selectedSpell = currentSpells[1];
                if (Input.GetKeyDown(KeyCode.B) && selectedSpell != null){
                    if(selectedSpell.IsAvailable()){
                        sourceTile.Unselect();
                        SpellSelectionTrigger(BattleManager.Trigger.VALIDATE);
                    }
                    break;
                }
                if (Input.GetKeyDown(KeyCode.N)){
                    // Retour au centre
                    spellChoice = SpellChoice.CHARACTER;
                    break;
                }
                if (Input.GetKeyDown(KeyCode.UpArrow)){
                    // Aller en haut
                    spellChoice = SpellChoice.UP;
                    overloaded = false;
                    break;
                }
                if (Input.GetKeyDown(KeyCode.DownArrow)){
                    // Aller en bas
                    spellChoice = SpellChoice.DOWN;
                    overloaded = false;
                    break;
                }
                if (Input.GetKeyDown(KeyCode.LeftArrow)){
                    // Aller à gauche
                    spellChoice = SpellChoice.LEFT;
                    overloaded = false;
                    break;
                }
                if (Input.GetKeyDown(KeyCode.RightArrow)){
                    // Passer en mode surcharge / Revenir au mode non surchargé
                    spellChoice = SpellChoice.RIGHT;
                    overloaded = !overloaded;
                    break;
                }
                break;
            case SpellChoice.UP:
                selectedSpell = currentSpells[2];
                if (Input.GetKeyDown(KeyCode.B) && selectedSpell != null){
                    if(selectedSpell.IsAvailable()){
                        sourceTile.Unselect();
                        SpellSelectionTrigger(BattleManager.Trigger.VALIDATE);
                    }
                    break;
                }
                if (Input.GetKeyDown(KeyCode.N)){
                    // Retour au centre
                    spellChoice = SpellChoice.CHARACTER;
                    break;
                }
                if (Input.GetKeyDown(KeyCode.RightArrow)){
                    // Aller à droite
                    spellChoice = SpellChoice.RIGHT;
                    overloaded = false;
                    break;
                }
                if (Input.GetKeyDown(KeyCode.DownArrow)){
                    // Aller en bas
                    spellChoice = SpellChoice.DOWN;
                    overloaded = false;
                    break;
                }
                if (Input.GetKeyDown(KeyCode.LeftArrow)){
                    // Aller à gauche
                    spellChoice = SpellChoice.LEFT;
                    overloaded = false;
                    break;
                }
                if (Input.GetKeyDown(KeyCode.UpArrow)){
                    // Passer en mode surcharge / Revenir au mode non surchargé
                    spellChoice = SpellChoice.UP;
                    overloaded = !overloaded;
                    break;
                }
                break;
            case SpellChoice.DOWN:
                selectedSpell = currentSpells[3];
                if (Input.GetKeyDown(KeyCode.B) && selectedSpell != null){
                    if(selectedSpell.IsAvailable()){
                        sourceTile.Unselect();
                        SpellSelectionTrigger(BattleManager.Trigger.VALIDATE);
                    }
                    break;
                }
                if (Input.GetKeyDown(KeyCode.N)){
                    // Retour au centre
                    spellChoice = SpellChoice.CHARACTER;
                    break;
                }
                if (Input.GetKeyDown(KeyCode.RightArrow)){
                    // Aller à droite
                    spellChoice = SpellChoice.RIGHT;
                    overloaded = false;
                    break;
                }
                if (Input.GetKeyDown(KeyCode.UpArrow)){
                    // Aller en haut
                    spellChoice = SpellChoice.UP;
                    overloaded = false;
                    break;
                }
                if (Input.GetKeyDown(KeyCode.LeftArrow)){
                    // Aller à gauche
                    spellChoice = SpellChoice.LEFT;
                    overloaded = false;
                    break;
                }
                if (Input.GetKeyDown(KeyCode.DownArrow)){
                    // Passer en mode surcharge / Revenir au mode non surchargé
                    spellChoice = SpellChoice.DOWN;
                    overloaded = !overloaded;
                    break;
                }
                break;

            default:
                break;
        }

        // Affichage du spell dans l'information panel
        switch(spellChoice){
            case SpellChoice.CHARACTER:
                DisplayUnit(sourceUnit);
                DisplaySpell(null, hyper : overloaded);
                break;
            case SpellChoice.LEFT:
                DisplaySpell(currentSpells[0], hyper : overloaded);
                break;
            case SpellChoice.RIGHT:
                DisplaySpell(currentSpells[1], hyper : overloaded);
                break;
            case SpellChoice.UP:
                DisplaySpell(currentSpells[2], hyper : overloaded);
                break;
            case SpellChoice.DOWN:
                DisplaySpell(currentSpells[3], hyper : overloaded);
                break;
            default:
                break;
        }
    }
    
    void SpellSelectionTrigger(BattleManager.Trigger trigger){
        // On sort de la sélection des spells pour aller vers un autre état
        BattleManager.Instance.ChangeState(BattleManager.Machine.PLAYERACTIONCHOICESTATE, trigger);
    }
    
    void TargetSelectionDisplay(){
        if(!activated_states[BattleManager.PlayerActionChoiceState.TARGET_SELECTION]){
            // Just changed from another state

            // Reset view
            ResetDisplay();

            // Activate the needed interface
            infosPanel.SetActive(true);
            tileSelector.SetActive(true);
            spellSelector.SetActive(true);
            shade.SetActive(true);
            spellPanel.SetActive(true);
            spellPanelLine.gameObject.SetActive(true);

            // TODO Définir un emplacement par défaut plus intelligent
            targetTile = UnitManager.Instance.GetUnits(Team.Enemy)[0].GetTile();
            targetTile.Select();
            
            ActivateState(BattleManager.PlayerActionChoiceState.TARGET_SELECTION);
        }

        // Change la position du sélector de case à l'aide d'un joli lerp
        targetTile.transform.position = targetTile.transform.position;
        tileSelector_currentPos = Vector3.Lerp(tileSelector_currentPos, tileSelector_targetPos, Time.deltaTime*selectorSpeed);
        tileSelector.transform.position = tileSelector_currentPos;
        tileSelector.transform.position = targetTile.transform.position;
        //GridManager.Instance.DisplayHighlights();

        // Highlight the selected tiles depending on the range of the spell

        // Change the tile or the state depending on the input (same algorithm than the source selection !)

        if (Input.GetKeyDown(KeyCode.B)){
            if(targetTile.GetUnit()!= null){
                targetTile.Unselect();
                TargetSelectionTrigger(BattleManager.Trigger.VALIDATE);
            }
        }

        if (Input.GetKeyDown(KeyCode.N)){
            targetTile.Unselect();
            sourceTile.Select();
            TargetSelectionTrigger(BattleManager.Trigger.CANCEL);
        }
        if (Input.GetKeyDown(KeyCode.UpArrow)){
            if(targetTile.GetNextTile(Directions.UP) != null){
                targetTile.Unselect();
                targetTile = targetTile.GetNextTile(Directions.UP);
                targetTile.Select();
            }
        }
        if (Input.GetKeyDown(KeyCode.DownArrow)){
            if(targetTile.GetNextTile(Directions.DOWN) != null){
                targetTile.Unselect();
                targetTile = targetTile.GetNextTile(Directions.DOWN);
                targetTile.Select();
            }
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow)){
            if(targetTile.GetNextTile(Directions.LEFT) != null){
                targetTile.Unselect();
                targetTile = targetTile.GetNextTile(Directions.LEFT);
                targetTile.Select();
            }
        }
        if (Input.GetKeyDown(KeyCode.RightArrow)){
            if(targetTile.GetNextTile(Directions.RIGHT) != null){
                targetTile.Unselect();
                targetTile = targetTile.GetNextTile(Directions.RIGHT);
                targetTile.Select();
            }
        }

        DrawPanelLine(spellPanelLine, targetTile);
        GridManager.Instance.SetSelectionMode(selectedSpell.GetRange());

        //Abstraire ce code
        BaseUnit currentUnit = targetTile.GetUnit();

        // N'aidait pas à la lisibilité d'après moi
        // (en gros, le personnage ennemi était toujours sélectionné, je préfère qu'on ait toujours les infos sur le lanceur)
        //if(currentUnit != null){
            //infosPanel.SetActive(true);
            //DisplayUnit(currentUnit);
            //DrawPanelLine(infosPanelLine, targetTile);
        //}
        //else{
            //infosPanel.SetActive(false);
        //}

        // Write into a variable the instruction if validated

        // Go back to the same spell selection if cancel
    }
    
    void TargetSelectionTrigger(BattleManager.Trigger trigger){
        // On sort de la sélection de la cible pour aller vers un autre état
        if(trigger == BattleManager.Trigger.VALIDATE){
            // Ajouter l'instruction dans la liste d'instructions
            if(selectedSpell == null){

            }
            ResetDisplay();
            Instruction instruction = BattleManager.Instance.CreateInstruction(sourceTile.GetUnit(), selectedSpell, targetTile, hyper : overloaded);
            BattleManager.Instance.AssignInstruction(instruction);
        }
        BattleManager.Instance.ChangeState(BattleManager.Machine.PLAYERACTIONCHOICESTATE, trigger);
    }

    private void DisplaySpell(BaseSpell spell, bool hyper = false){
        if(spell != null){
            spellPanel.SetActive(true);
            spellNamePanel.text = spell.GetName();
            spellDescriptionPanel.text = spell.GetFightDescription(hyper);
            spellCooldownPanel.text = spell.GetCooldown().ToString() + " / " + spell.GetBaseCooldown().ToString();  
            spellPanelIcon.sprite = spell.GetArtwork();
        }
        else{
            spellPanel.SetActive(false);
            spellNamePanel.text="";
            spellDescriptionPanel.text = "";
            spellCooldownPanel.text = "";
        }
    }

    private void DisplayUnit(BaseUnit unit){
        unitNamePanel.text = unit.GetName();
        unitPowerPanel.text = "Puissance : " + unit.GetFinalPower().ToString();
        unitHealthPanel.text = "PV : " + unit.GetFinalHealth().ToString() + "/" + unit.GetTotalHealth().ToString();
        if(unit.GetArmor() > 0){
            unitArmorPanel.text = "Armure : " + unit.GetArmor().ToString();
        }
        else{
            unitArmorPanel.text = "";
        }
        unitLevelPanel.text = "Niveau : " + unit.GetLevel().ToString();
        // TODO Il faut changer ça, là on n'affiche que le premier passif de la liste
        if(unit.GetPassives().Count >= 1){
            unitPassiveNamePanel.text = unit.GetPassives()[0].GetName();
            unitPassiveDescriptionPanel.text = unit.GetPassives()[0].GetFightDescription();
        }
        else{
            unitPassiveNamePanel.text = "";
            unitPassiveDescriptionPanel.text = "";
        }
        spellCooldownPanel.text = "";
    }
    
    void ResetDisplay(){
            spellSelector.SetActive(false);
            shade.SetActive(false);
            infosPanel.SetActive(false);
            tileSelector.SetActive(false);
            spellPanel.SetActive(false);
    }

    void ActivateState(BattleManager.PlayerActionChoiceState stateToActivate){
        Dictionary<BattleManager.PlayerActionChoiceState, bool> new_states = new Dictionary<BattleManager.PlayerActionChoiceState, bool>(activated_states);
        foreach (var state in activated_states)
        {
            if(state.Key == stateToActivate){
                new_states[state.Key] = true;
            }
            else{
                new_states[state.Key] = false;
            }
        }
        activated_states = new_states;
    }

    void Navigate(Directions direction){
        //
    }

    public Tile GetMainSelection(){
        switch (BattleManager.Instance.GetPlayerActionChoiceState())
        {
            case BattleManager.PlayerActionChoiceState.CHARACTER_SELECTION:
                return sourceTile;
            case BattleManager.PlayerActionChoiceState.SPELL_SELECTION:
                return sourceTile;
            case BattleManager.PlayerActionChoiceState.TARGET_SELECTION:
                return targetTile;
            default:
                return null;
        }
    }


    // Fonction pour gérer la ligne d'UI qui pointe sur les objets. assez rigide pour l'instant
    private void DrawPanelLine(RectTransform PanelLine, Tile tile){

        // Convertit la position dans le gameworld en position en px sur l'écran.
        Vector3 targetPosition = mainCamera.WorldToScreenPoint(tile.transform.position);

        // Pour l'instant, au cas par cas en fonction de quelle ligne de quel panneau est utilisée.... A voir si on en a beaucoup par la suite
        if(PanelLine == spellSelectorLine)
            PanelLine.sizeDelta = new Vector2(targetPosition.x - tileSize*1.6f,  Screen.height - spellSelector.GetComponent<RectTransform>().rect.height - targetPosition.y-tileSize*0.3f); // a bit ugly but still good      
        
        /// Ces lignes géraient la ligne de l'info panel, qui n'en a plus dans le layout actuel
        //if(PanelLine == infosPanelLine)
        //    PanelLine.sizeDelta = new Vector2(targetPosition.x - tileSize, Screen.height - infosPanel.GetComponent<RectTransform>().rect.height - targetPosition.y); // a bit ugly but still good
            //PanelLine.sizeDelta = new Vector2(targetPosition.x - tileSize, targetPosition.y - infosPanel.GetComponent<RectTransform>().rect.height); // a bit ugly but still good      
        
        // Gère la position de la ligne du sort pendant la sélection de target
        if (PanelLine == spellPanelLine)
            PanelLine.sizeDelta = new Vector2(Screen.width-targetPosition.x - tileSize, targetPosition.y - spellPanel.GetComponent<RectTransform>().rect.height); // Hard coded, needs some update
    } 

    // Setup la barre de vie d'un perso
    public GameObject SetupLifebar(BaseUnit unit){
        
        // GameObject instanciation
        GameObject lifeBarPanel = Instantiate(lifeBarPrefab);

        lifeBarPanel.transform.SetParent((unit.GetTeam() == Team.Ally) ? lifeBarsUI.transform.Find("Allies").transform : lifeBarsUI.transform.Find("Ennemies").transform);
        lifeBarPanel.transform.localScale = Vector3.one;
        lifeBarPanel.transform.position = unit.transform.position + lifeBarOffset;
        lifeBarPanel.name = $"{unit.GetName()}_LifeBar";

        lifeBarPanel.GetComponent<LifeBar>().Setup(unit);

        return lifeBarPanel;
    }
    
    // Update la barre de vie d'un perso
    public void UpdateLifebar(BaseUnit unit, int HPChange, int armorChange){
        // Child components access and modification, very ugly
        unit.lifeBar.GetComponent<LifeBar>().UpdateHP(HPChange);
        unit.lifeBar.GetComponent<LifeBar>().UpdateArmor(armorChange);
    }

    public void UpdateLifeBarPosition(BaseUnit unit){
        unit.lifeBar.transform.position = unit.transform.position + lifeBarOffset;
    }

    // Fonction pour gérer les anims de la croix de sorts. La solution actuelles est dégeu, il faudra la rebosser intelligemment.
    // Notamment on peut réduire le nombre d'animations requises par 5 si j'apprends à faire des anims paramétrables.
    private void AnimateSpellChoice(int index){
        //Liste contenant le nom des animations disponibles pour le spellChoicePanel
        List<string> spellChoiceAnims = new List<string>
        {"LeftSelected", "RightSelected", "TopSelected",
         "DownSelected", "AttackSelected",};

        // On récupère l'Animator de chaque choix de spell
        for (int i=0; i<5; i++){
            Animator spellChoiceAnimator = spellSelector.transform.GetChild(i).GetComponent<Animator>();

            // Si c'est le spell sélectionné
            if (i == index){
                spellChoiceAnimator.Play(spellChoiceAnims[i]);
                continue;
            }

            // S'il était précédemment sélectionné, on le déselctionne
            if (spellChoiceAnimator.GetCurrentAnimatorStateInfo(0).IsName(spellChoiceAnims[i]))
                spellChoiceAnimator.Play($"Not{spellChoiceAnims[i]}");
        }
    }

    public Canvas GetUI(){
        return UI;
    }
}


public enum Directions {RIGHT, LEFT, UP, DOWN}
