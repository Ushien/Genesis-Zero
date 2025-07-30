using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using System.Linq;
using UnityEngine.Assertions;
using System.Data.Common;
using UnityEngine.InputSystem;
using System;
using UnityEngine.Localization.Settings;

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
    [SerializeField]
    private Canvas RewardUIPrefab;
    private Canvas UI;
    public Canvas RewardUI;

    // UI Lines
    public GameObject UILine;
    public GameObject UILinePrefab;
    public Transform UILineVertical;
    public Transform UILineHorizontal;

    // UI Panel Lines
    public GameObject UIPanelLine;
    public GameObject UIPanelLinePrefab;
    public Transform UIPanelLineVertical;
    public Transform UIPanelLineHorizontal;

    // UI elements
    private Transform unitPanel;
    public Transform spellPanel;
    private Transform passivePanel;
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
    public TextMeshProUGUI rewardSpellName;
    public TextMeshProUGUI rewardSpellDescription;
    public TextMeshProUGUI rewardSpellCooldown;
    private Image spellPanelIcon;
    public Transform spellSelector;
    public Transform shade;
    public Canvas UIWorldSpace;
    private Canvas lifeBarsUI;
    public GameObject lifeBarPrefab;
    public Material grayscaleShader;
    public Color blueColor;
    public Sprite emptySpellSelectorSquare;



    private Transform tileSelector;
    private Vector3 tileSelector_targetPos;
    private Vector3 tileSelector_currentPos;
    public float errorShakeIntensity;
    public float errorShakeDuration;
    public float driftAmount = 0f;
    public float driftIntensity = 0f;
    public float driftSmoothing = 0f;
    public float xLineOffset = 0f;
    public float yLineOffset = 0f;
    public float yLinePanelOffset = 0f;
    public float UILinePanelVerticalOffset = 0f;
    public float xZoomOffset = 0f;
    public float yZoomOffset = 0f;
    public float zoomSpeed = 0.1f;
    public float rewardYPos = 0.52f;

    public Vector3 lifeBarOffset;

    [SerializeField]
    private PlayerInput playerInput;
    [SerializeField]
    private float selectorSpeed;

    [SerializeField]
    private Material tileOutliner;
    public Camera mainCamera; // Utile pour convertir des position in game à des positions en pixels sur l'écran

    public float tileSize = 250f; // A bit ugly but still good for now

    private enum SpellChoice { CHARACTER, LEFT, RIGHT, UP, DOWN }
    private SpellChoice spellChoice;
    private SpellChoice currentSpellChoice = SpellChoice.DOWN;
    private SpellChoice currentSpellChoiceAnim = SpellChoice.DOWN;
    private SpellChoice currentSpellChoiceHighlight = SpellChoice.DOWN;

    [SerializeField]
    public bool overloaded = false;
    // La Tile contenant la source du spell, lorsqu'un spell est lancé (lorsqu'on revient en arrière pendant la sélection de cible)
    [SerializeField]
    private Tile sourceTile;

    // Le spell pour lequel on va sélectionner une cible
    [SerializeField]
    private BaseSpell selectedSpell;

    // La Tile contenant la cible du spell, lorsqu'un spell est lancé
    [SerializeField]
    public Tile targetTile;
    private BaseUnit selectedUnit;

    private Dictionary<BattleManager.PlayerActionChoiceState, bool> activated_states;

    private int selectedPassiveIndex = 0;

    #endregion

    void Awake()
    {
        Instance = this;

        try
        {
            // On initialise la UI
            UI = Instantiate(UIPrefab);
            lifeBarsUI = GlobalManager.Instance.GetUIWorldSpace();
            RewardUI = Instantiate(RewardUIPrefab);
            RewardUI.worldCamera = GlobalManager.Instance.GetCam();
            UI.worldCamera = GlobalManager.Instance.GetCam();


            unitPanel = UI.transform.Find("UnitPanelEncapsulator").Find("UnitPanel");
            spellPanel = UI.transform.Find("SpellPanelEncapsulator").Find("SpellPanel");
            passivePanel = UI.transform.Find("PassivePanelEncapsulator").Find("PassivePanel");
            spellSelector = UI.transform.Find("SpellSelectorEncapsulator").Find("SpellSelector");
            shade = UI.transform.Find("Shade");

            rewardSpellName = RewardUI.transform.Find("DescriptionPanel").transform.Find("Name").GetComponent<TextMeshProUGUI>();
            rewardSpellDescription = RewardUI.transform.Find("DescriptionPanel").transform.Find("Description").GetComponent<TextMeshProUGUI>();
            rewardSpellCooldown = RewardUI.transform.Find("DescriptionPanel").transform.Find("Cooldown").GetComponent<TextMeshProUGUI>();

            unitNamePanel = unitPanel.Find("UnitName").GetComponent<TextMeshProUGUI>();
            unitPowerPanel = unitPanel.Find("UnitPower").GetComponent<TextMeshProUGUI>();
            unitHealthPanel = unitPanel.Find("UnitHealth").GetComponent<TextMeshProUGUI>();
            unitArmorPanel = unitPanel.Find("UnitArmor").GetComponent<TextMeshProUGUI>();
            unitLevelPanel = unitPanel.Find("UnitLevel").GetComponent<TextMeshProUGUI>();

            unitPassiveNamePanel = passivePanel.Find("Name").GetComponent<TextMeshProUGUI>();
            unitPassiveDescriptionPanel = passivePanel.Find("Description").GetComponent<TextMeshProUGUI>();

            spellNamePanel = spellPanel.transform.Find("Name").GetComponent<TextMeshProUGUI>();
            spellCooldownPanel = spellPanel.transform.Find("Cooldown").GetComponent<TextMeshProUGUI>();
            spellDescriptionPanel = spellPanel.transform.Find("Description").GetComponent<TextMeshProUGUI>();
            spellPanelIcon = spellPanel.transform.Find("Sprite").GetComponent<Image>();

            mainCamera = GlobalManager.Instance.GetCam();

            activated_states = new Dictionary<BattleManager.PlayerActionChoiceState, bool>();
            foreach (BattleManager.PlayerActionChoiceState state in System.Enum.GetValues(typeof(BattleManager.PlayerActionChoiceState)))
            {
                activated_states[state] = false;
            }

            PlayerInput playerInput = InputManager.Instance.GetPlayerInput();
            playerInput.actions["Validate"].performed += ValidateInput;
            playerInput.actions["Cancel"].performed += CancelInput;
            playerInput.actions["Movement"].performed += MovementInput;
            playerInput.actions["Switch"].performed += SwitchInput;
        }
        catch (Exception)
        {
            Warning.Info("Il y a eu un problème lors de l'initialisation de l'interface");
        }
    }

    void Start()
    {

        // On crée le tileSelector qui va naviguer pour la sélection des cases

        tileSelector = Instantiate(GridManager.Instance.GetTilePrefab()).transform;
        Material material = Instantiate(tileOutliner);
        tileSelector.gameObject.SetActive(false);
        tileSelector.GetComponent<SpriteRenderer>().enabled = false;

        tileSelector.transform.GetComponent<UnityEngine.SpriteRenderer>().material = material;
        tileSelector.transform.GetComponent<UnityEngine.SpriteRenderer>().sortingOrder = 2;
        tileSelector.transform.parent = UI.transform;
        tileSelector.name = "Tile Selector";
        tileSelector.transform.DetachChildren();

        tileSelector_targetPos = tileSelector.transform.position;
        tileSelector_currentPos = tileSelector.transform.position;

        // On initialise la ligne des UI
        UILine = Instantiate(UILinePrefab);
        UIPanelLine = Instantiate(UIPanelLinePrefab);
        UILineVertical = UILine.transform.Find("vertical");
        UILineHorizontal = UILine.transform.Find("horizontal");
        UIPanelLineVertical = UIPanelLine.transform.Find("vertical");
        UIPanelLineHorizontal = UIPanelLine.transform.Find("horizontal");

    }

    void Update()
    {
        // Super moche mais flemme de débug plus finement maintenant
        if (!overloaded && spellPanel.transform.Find("overloaded").gameObject.activeSelf)
            spellPanel.transform.Find("overloaded").gameObject.SetActive(false);

        if (BattleManager.Instance != null)
        {
            // TODO retirer tout ça
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

    void SourceSelectionDisplay()
    {
        if (!activated_states[BattleManager.PlayerActionChoiceState.CHARACTER_SELECTION])
        {
            // Reset those flags
            currentSpellChoice = SpellChoice.DOWN;
            currentSpellChoiceAnim = SpellChoice.DOWN;
            currentSpellChoiceHighlight = SpellChoice.DOWN;

            // Just changed from another state
            // Reset view
            ResetDisplay();

            // Activate the needed interface
            unitPanel.gameObject.SetActive(true);
            passivePanel.gameObject.SetActive(true);
            tileSelector.gameObject.SetActive(true);

            if (UnitManager.Instance.GetUnits(Team.Ally).Count > 0)
            {
                sourceTile = UnitManager.Instance.GetUnits(Team.Ally).Where(_unit => !_unit.HasGivenInstruction()).First().GetTile();
            }
            else
            {
                // On prend une case au hasard je ne comprends pas pourquoi il crash si je ne lui en donne pas
                sourceTile = GridManager.Instance.GetRandomTile(Team.Both);
            }

            if (sourceTile.GetUnit() != null)
            {
                selectedUnit = sourceTile.GetUnit();
                DisplayUnit();
                DisplayPassives();
            }

            sourceTile.Select();
            GridManager.Instance.SetSelectionMode(GridManager.Selection_mode.Single_selection);

            tileSelector.transform.position = sourceTile.transform.position;
            tileSelector_currentPos = sourceTile.transform.position;

            ActivateState(BattleManager.PlayerActionChoiceState.CHARACTER_SELECTION);

        }

        /*
        // Change la position du sélector de case à l'aide d'un joli lerp
        tileSelector_targetPos = sourceTile.transform.position;
        tileSelector_currentPos = Vector3.Lerp(tileSelector_currentPos, tileSelector_targetPos, Time.deltaTime * selectorSpeed);
        tileSelector.transform.position = tileSelector_currentPos;
        */
    }
    void SourceSelectionTrigger(BattleManager.Trigger trigger)
    {
        // On sort de la sélection de la source pour aller vers un autre état
        BattleManager.Instance.ChangeState(BattleManager.Machine.PLAYERACTIONCHOICESTATE, trigger);
    }

    void SpellSelectionDisplay()
    {
        // TODO Ne pas afficher les 4 cases si le personnage n'a pas 4 spells
        BaseUnit sourceUnit = sourceTile.GetUnit();
        BaseSpell[] currentSpells = sourceUnit.GetFourSpells();

        if (!activated_states[BattleManager.PlayerActionChoiceState.SPELL_SELECTION])
        {
            // Just changed from another state

            // Reset view
            ResetDisplay();

            // Activate the needed interface
            spellSelector.gameObject.SetActive(true);
            shade.gameObject.SetActive(true);
            unitPanel.gameObject.SetActive(true);
            spellPanel.gameObject.SetActive(true);

            selectedUnit = sourceTile.GetUnit();
            DisplayUnit();

            //spellSelector.transform.position = sourceTile.transform.position;
            DrawUILine(UILine, sourceTile);

            int currentSpellIndex = 0;
            foreach (BaseSpell spell in currentSpells)
            {
                // On instancie le material des cooldown pour les modifier indépendamment
                Material cooldownUnit = spellSelector.transform.GetChild(currentSpellIndex).Find("CoolDownUnit").GetComponent<Image>().material;
                RectTransform cooldownProgress = spellSelector.transform.GetChild(currentSpellIndex).Find("CoolDownProgress").GetComponent<RectTransform>();
                GameObject unavailable = spellSelector.transform.GetChild(currentSpellIndex).Find("unavailable").gameObject;
                spellSelector.transform.GetChild(currentSpellIndex).Find("CoolDownUnit").GetComponent<Image>().material = new Material(cooldownUnit);
                cooldownUnit = spellSelector.transform.GetChild(currentSpellIndex).Find("CoolDownUnit").GetComponent<Image>().material;

                // mettre la bonne image dans le spell selector
                Image spellImage = spellSelector.transform.GetChild(currentSpellIndex).GetComponent<Image>();

                if (spell != null)
                {
                    spellImage.sprite = spell.GetArtwork();
                    spellImage.material = null;
                    cooldownUnit.SetVector("_Tiling", new Vector2(spell.base_cooldown, 1f));
                    if (!spell.IsAvailable())
                    {
                        Material material = Instantiate(grayscaleShader);
                        spellImage.material = material;
                        cooldownUnit.SetColor("_Color", new Color(1, 0, 0, 1));
                        unavailable.SetActive(true);
                        //Grey
                    }
                    else
                    {
                        cooldownUnit.SetColor("_Color", blueColor);
                        unavailable.SetActive(false);
                    }
                    cooldownProgress.localScale = new Vector3(1f - ((float)spell.cooldown / (float)spell.base_cooldown), 1f, 1f);
                }

                else
                {
                    cooldownUnit.SetColor("_Color", blueColor);
                    spellImage.sprite = emptySpellSelectorSquare;
                    spellImage.material = null;
                    cooldownUnit.SetVector("_Tiling", new Vector2(0f, 1f));
                    cooldownProgress.localScale = new Vector3(0f, 1f, 1f);
                    unavailable.SetActive(false);
                }
                currentSpellIndex += 1;
            }

            // Change aAttack image
            spellSelector.transform.GetChild(currentSpellIndex).GetComponent<Image>().sprite = sourceUnit.GetAttack().GetArtwork();
            selectedSpell = sourceUnit.GetAttack();
            DisplaySpell();

            GridManager.Instance.SetSelectionMode(GridManager.Selection_mode.Single_selection);

            ActivateState(BattleManager.PlayerActionChoiceState.SPELL_SELECTION);
        }

        //Display highlight
        if (spellChoice != currentSpellChoiceHighlight)
        {
            spellSelector.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(false);
            spellSelector.transform.GetChild(1).transform.GetChild(0).gameObject.SetActive(false);
            spellSelector.transform.GetChild(2).transform.GetChild(0).gameObject.SetActive(false);
            spellSelector.transform.GetChild(3).transform.GetChild(0).gameObject.SetActive(false);
            spellSelector.transform.GetChild(4).transform.GetChild(0).gameObject.SetActive(false);
            currentSpellChoiceHighlight = spellChoice;
        }

        // Si le spellChoice n'a pas changé, on joue pas l'anim
        if (spellChoice != currentSpellChoiceAnim)
        {
            switch (spellChoice)
            {
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
            currentSpellChoiceAnim = spellChoice;
        }

        /////////
        // Affichage du spell dans l'information panel

        currentSpellChoice = spellChoice;
    }

    void SpellSelectionTrigger(BattleManager.Trigger trigger)
    {
        // On sort de la sélection des spells pour aller vers un autre état
        BattleManager.Instance.ChangeState(BattleManager.Machine.PLAYERACTIONCHOICESTATE, trigger);
    }

    void TargetSelectionDisplay()
    {
        if (!activated_states[BattleManager.PlayerActionChoiceState.TARGET_SELECTION])
        {
            // Just changed from another state

            // Reset view
            ResetDisplay();

            // Activate the needed interface
            unitPanel.gameObject.SetActive(true);
            tileSelector.gameObject.SetActive(true);
            //spellSelector.gameObject.SetActive(true);
            shade.gameObject.SetActive(true);
            spellPanel.gameObject.SetActive(true);

            // TODO Définir un emplacement par défaut plus intelligent
            targetTile = UnitManager.Instance.GetUnits(Team.Enemy)[0].GetTile();
            targetTile.Select();
            selectedUnit = targetTile.GetUnit();
            DisplayUnit();
            DrawUILine(UIPanelLine, targetTile);

            ActivateState(BattleManager.PlayerActionChoiceState.TARGET_SELECTION);
        }

        // Change la position du sélector de case à l'aide d'un joli lerp
        targetTile.transform.position = targetTile.transform.position;
        tileSelector_currentPos = Vector3.Lerp(tileSelector_currentPos, tileSelector_targetPos, Time.deltaTime * selectorSpeed);
        tileSelector.transform.position = tileSelector_currentPos;
        tileSelector.transform.position = targetTile.transform.position;
    }

    void TargetSelectionTrigger(BattleManager.Trigger trigger)
    {
        // On sort de la sélection de la cible pour aller vers un autre état
        if (trigger == BattleManager.Trigger.VALIDATE)
        {
            // Ajouter l'instruction dans la liste d'instructions
            if (selectedSpell == null)
            {

            }
            ResetDisplay();
            Instruction instruction = BattleManager.Instance.CreateInstruction(sourceTile.GetUnit(), selectedSpell, targetTile, hyper: overloaded);
            BattleManager.Instance.AssignInstruction(instruction);
        }
        BattleManager.Instance.ChangeState(BattleManager.Machine.PLAYERACTIONCHOICESTATE, trigger);
    }

    private void DisplaySpell()
    {
        // manière cheap de rejouer les anims de texte et d'éviter des bugs
        ResetPanel(spellPanel);
        BaseSpell spell = selectedSpell;
        bool hyper = overloaded;
        if (spell != null)
        {
            spellPanel.gameObject.SetActive(true);
            spellNamePanel.text = spell.GetName();
            spellDescriptionPanel.text = spell.GetDescription(hyper);

            if (spell.IsAAttack())
            {
                spellCooldownPanel.text = "";
            }
            else
            {
                spellCooldownPanel.text = spell.GetCooldown().ToString() + " / " + spell.GetBaseCooldown().ToString();
            }
            spellPanelIcon.sprite = spell.GetArtwork();
        }
        else
        {
            spellPanel.gameObject.SetActive(false);
            spellNamePanel.text = "";
            spellDescriptionPanel.text = "";
            spellCooldownPanel.text = "";
        }
    }

    private void DisplayUnit()
    {
        ResetPanel(unitPanel);
        if (selectedUnit != null)
        {
            BaseUnit unit = selectedUnit;
            unitNamePanel.text = unit.GetName();
            unitPowerPanel.text = LocalizationSettings.StringDatabase.GetLocalizedString("Interface", "Power") + " : " + unit.GetFinalPower().ToString();
            unitHealthPanel.text = LocalizationSettings.StringDatabase.GetLocalizedString("Interface", "HP") + " : " + unit.GetFinalHealth().ToString() + "/" + unit.GetTotalHealth().ToString();
            if (unit.GetArmor() > 0)
            {
                unitArmorPanel.text = LocalizationSettings.StringDatabase.GetLocalizedString("Interface", "Armor") + " : " + unit.GetArmor().ToString();
            }
            else
            {
                unitArmorPanel.text = " ";
            }
            unitLevelPanel.text = LocalizationSettings.StringDatabase.GetLocalizedString("Interface", "Level") + " : " + unit.GetLevel().ToString();    
        }
    }

    private void DisplayPassives()
    {
        BaseUnit unit = selectedUnit;
        ResetPanel(passivePanel);
        if (unit.GetPassives().Count >= 1)
        {
            unitPassiveNamePanel.text = unit.GetPassives()[selectedPassiveIndex].GetName();
            unitPassiveDescriptionPanel.text = unit.GetPassives()[selectedPassiveIndex].GetDescription();
            passivePanel.Find("Sprite").GetComponent<Image>().sprite = unit.GetPassives()[selectedPassiveIndex].GetScriptablePassive().artwork;
            if (unit.GetPassives().Count >= 2)
            {
                passivePanel.Find("NextSprite").gameObject.SetActive(true);
                if (selectedPassiveIndex == unit.GetPassives().Count - 1)
                {
                    passivePanel.Find("NextSprite").GetComponent<Image>().sprite = unit.GetPassives()[0].GetScriptablePassive().artwork;
                }
                else
                {
                    passivePanel.Find("NextSprite").GetComponent<Image>().sprite = unit.GetPassives()[selectedPassiveIndex + 1].GetScriptablePassive().artwork;
                }
            }
            else
            {
                passivePanel.Find("NextSprite").gameObject.SetActive(false);
            }
        }
        else
        {
            passivePanel.gameObject.SetActive(false);
            unitPassiveNamePanel.text = "";
            unitPassiveDescriptionPanel.text = "";
        }
    }

    public void ResetDisplay()
    {
        UILine.SetActive(false);
        UIPanelLine.SetActive(false);
        RewardUI.gameObject.SetActive(false);
        shade.gameObject.SetActive(false);
        unitPanel.gameObject.SetActive(false);
        passivePanel.gameObject.SetActive(false);
        tileSelector.gameObject.SetActive(false);
        spellPanel.gameObject.SetActive(false);
        spellSelector.gameObject.SetActive(false);
    }

    void ResetPanel(Transform panel)
    {
        panel.gameObject.SetActive(false);
        panel.gameObject.SetActive(true);
    }

    void ActivateState(BattleManager.PlayerActionChoiceState stateToActivate)
    {
        Dictionary<BattleManager.PlayerActionChoiceState, bool> new_states = new Dictionary<BattleManager.PlayerActionChoiceState, bool>(activated_states);
        foreach (var state in activated_states)
        {
            if (state.Key == stateToActivate)
            {
                new_states[state.Key] = true;
            }
            else
            {
                new_states[state.Key] = false;
            }
        }
        activated_states = new_states;
    }

    void NavigateSource(Directions direction)
    {
        if (sourceTile.GetNextTile(direction) != null)
        {
            sourceTile.GetNextTile(direction).Select();
            sourceTile.Unselect();
            sourceTile = sourceTile.GetNextTile(direction);
            Vector2 vectorDirection = Tools.ConvertDirectionsToVector2(direction);
            //Petit coup de caméra dans la direction
            CameraEffects.Instance.TriggerDrift(driftIntensity, driftSmoothing, new Vector3(driftAmount * vectorDirection.x, driftAmount * vectorDirection.y, 0f));
            // Reset le slider de passifs
            selectedPassiveIndex = 0;

            selectedUnit = sourceTile.GetUnit();
            if (selectedUnit != null)
            {
                DisplayUnit();
                DisplayPassives();
            }
            else
            {
                unitPanel.gameObject.SetActive(false);
                passivePanel.gameObject.SetActive(false);
            }
        }
    }

    void NavigateTarget(Directions direction)
    {
        // S'il y a une case dans la direction indiquée
        if (targetTile.GetNextTile(direction) != null)
        {
            targetTile.Unselect();
            targetTile = targetTile.GetNextTile(direction);
            targetTile.Select();
            selectedUnit = targetTile.GetUnit();
            DrawUILine(UIPanelLine, targetTile);
            Vector2 vectorDirection = Tools.ConvertDirectionsToVector2(direction);
            //Petit coup de caméra dans la direction
            CameraEffects.Instance.TriggerDrift(driftIntensity, driftSmoothing, new Vector3(driftAmount * vectorDirection.x, driftAmount * vectorDirection.y, 0f));

            selectedUnit = targetTile.GetUnit();
            if (selectedUnit != null)
            {
                DisplayUnit();
            }
            else
            {
                unitPanel.gameObject.SetActive(false);
                passivePanel.gameObject.SetActive(false);
            }
        }
    }

    void NavigatePassives(Directions direction)
    {
        BaseUnit currentUnit = sourceTile.GetUnit();
        if (currentUnit != null)
        {
            List<Passive> passives = currentUnit.GetPassives();
            if (passives.Count > 0)
            {
                if (direction == Directions.LEFT)
                {
                    if (selectedPassiveIndex > 0)
                    {
                        selectedPassiveIndex -= 1;
                    }
                    else
                    {
                        selectedPassiveIndex = passives.Count - 1;
                    }
                }
                if (direction == Directions.RIGHT)
                {
                    if (selectedPassiveIndex < passives.Count - 1)
                    {
                        selectedPassiveIndex += 1;
                    }
                    else
                    {
                        selectedPassiveIndex = 0;
                    }
                }
            }
            DisplayPassives();
        }
        if (GlobalManager.Instance.debug)
        {
            Assert.IsTrue(selectedPassiveIndex >= 0);
        }
    }

    public Tile GetMainSelection()
    {
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


    // Fonction pour gérer la ligne d'UI nouvelle
    // ------------------------------------------
    private void DrawUILine(GameObject Line, Tile tile)
    {
        Line.SetActive(true);
        Vector3 targetPosition = tile.transform.position;
        if (Line == UILine)
        {
            UILine.transform.position = new Vector3(tile.transform.position.x + xLineOffset, tile.transform.position.y + yLineOffset, 0f);
            UILineVertical.position = new Vector3(-UILinePanelVerticalOffset, 0f, 0f);
        }
        if (Line == UIPanelLine)
        {
            UIPanelLineHorizontal.position = new Vector3(UIPanelLineHorizontal.position.x, targetPosition.y + yLinePanelOffset, 0f);
        }
    }


    // Setup la barre de vie d'un perso
    public LifeBar SetupLifebar(BaseUnit unit)
    {

        // GameObject instanciation
        LifeBar lifeBarPanel = Instantiate(lifeBarPrefab).GetComponent<LifeBar>();

        lifeBarPanel.transform.SetParent((unit.GetTeam() == Team.Ally) ? lifeBarsUI.transform.Find("Allies").transform : lifeBarsUI.transform.Find("Ennemies").transform);
        lifeBarPanel.transform.localScale = Vector3.one;
        lifeBarPanel.transform.position = unit.transform.position + lifeBarOffset;
        lifeBarPanel.name = $"{unit.GetName()}_LifeBar";

        lifeBarPanel.Setup(unit);

        return lifeBarPanel;
    }

    // Update la barre de vie d'un perso
    public void UpdateLifebar(BaseUnit unit, int finalHPChange, int totalHPChange, int armorChange)
    {
        // Child components access and modification, very ugly
        unit.lifeBar.GetComponent<LifeBar>().UpdateHP(finalHPChange);
        unit.lifeBar.GetComponent<LifeBar>().UpdateTotalHP(totalHPChange);
        unit.lifeBar.GetComponent<LifeBar>().UpdateArmor(armorChange);
    }

    public void UpdateLifeBarPosition(BaseUnit unit)
    {
        unit.lifeBar.transform.position = unit.transform.position + lifeBarOffset;
    }

    // Fonction pour gérer les anims de la croix de sorts. La solution actuelles est dégeu, il faudra la rebosser intelligemment.
    // Notamment on peut réduire le nombre d'animations requises par 5 si j'apprends à faire des anims paramétrables.
    private void AnimateSpellChoice(int index)
    {
        //Liste contenant le nom des animations disponibles pour le spellChoicePanel
        List<string> spellChoiceAnims = new List<string>
        {"LeftSelected", "RightSelected", "TopSelected",
         "DownSelected", "AttackSelected",};

        // On récupère l'Animator de chaque choix de spell
        for (int i = 0; i < 5; i++)
        {
            Animator spellChoiceAnimator = spellSelector.transform.GetChild(i).GetComponent<Animator>();

            // Si c'est le spell sélectionné
            if (i == index)
            {
                spellChoiceAnimator.Play(spellChoiceAnims[i]);
            }

            // S'il était précédemment sélectionné, on le déselctionne
            var stateInfo = spellChoiceAnimator.GetCurrentAnimatorStateInfo(0);
            if (!stateInfo.IsName($"Not{spellChoiceAnims[i]}"))
            {
                spellChoiceAnimator.Play($"Not{spellChoiceAnims[i]}");

                // enlever le overload
                if (i != 4)
                    spellSelector.transform.GetChild(i).Find("overloaded").gameObject.SetActive(false);
            }
            // + déselectionner le overload
        }
    }

    public Canvas GetUI()
    {
        return UI;
    }

    public void ShakeElement(RectTransform uiElement, float intensity, float shakeTime)
    {
        Vector3 originalPosition = uiElement.anchoredPosition;
        void UpdateShake()
        {
            if (shakeTime > 0f)
            {
                Vector2 randomOffset = UnityEngine.Random.insideUnitCircle * intensity;
                uiElement.anchoredPosition = originalPosition + (Vector3)randomOffset;
                shakeTime -= Time.deltaTime;
            }
            else
            {
                uiElement.anchoredPosition = originalPosition;
            }
            if (shakeTime > 0)
                UpdateShake();
        }

    }

    public void DisableSpellOverload()
    {
        for (int i = 0; i < 4; i++)
            spellSelector.transform.GetChild(i).transform.Find("overloaded").gameObject.SetActive(false);
        spellPanel.transform.Find("overloaded").gameObject.SetActive(false);

        // tant qu'on y est on reset le spell sélectionné       
        spellChoice = SpellChoice.CHARACTER;
        overloaded = false;
    }

    private void ValidateInput(InputAction.CallbackContext context)
    {
        AudioManager.Instance.UIBip2.Play();
        switch (BattleManager.Instance.GetPlayerActionChoiceState())
        {
            case BattleManager.PlayerActionChoiceState.CHARACTER_SELECTION:
                SourceSelectionValidateInput(context);
                break;
            case BattleManager.PlayerActionChoiceState.SPELL_SELECTION:
                SpellValidateInput(context);
                break;
            case BattleManager.PlayerActionChoiceState.TARGET_SELECTION:
                TargetSelectionValidateInput(context);
                break;
            default:
                break;
        }
    }

    private void CancelInput(InputAction.CallbackContext context)
    {
        AudioManager.Instance.UIBip2.Play();
        switch (BattleManager.Instance.GetPlayerActionChoiceState())
        {
            case BattleManager.PlayerActionChoiceState.CHARACTER_SELECTION:
                SourceSelectionCancelInput(context);
                break;
            case BattleManager.PlayerActionChoiceState.SPELL_SELECTION:
                SpellCancelInput(context);
                break;
            case BattleManager.PlayerActionChoiceState.TARGET_SELECTION:
                TargetSelectionCancelInput(context);
                break;
            default:
                break;
        }
    }

    private void MovementInput(InputAction.CallbackContext context)
    {
        switch (BattleManager.Instance.GetPlayerActionChoiceState())
        {
            case BattleManager.PlayerActionChoiceState.CHARACTER_SELECTION:
                AudioManager.Instance.UIBip1.Play();
                SourceSelectionMovementInput(context);
                break;
            case BattleManager.PlayerActionChoiceState.SPELL_SELECTION:
                SpellMovementInput(context);
                break;
            case BattleManager.PlayerActionChoiceState.TARGET_SELECTION:
                AudioManager.Instance.UIBip1.Play();
                TargetSelectionMovementInput(context);
                break;
            default:
                break;
        }
    }

    private void SwitchInput(InputAction.CallbackContext context)
    {
        NavigatePassives(Directions.RIGHT);
    }
    private void SourceSelectionValidateInput(InputAction.CallbackContext context)
    {
        if (sourceTile.GetUnit() != null)
        {
            if (sourceTile.GetUnit().GetTeam() == Team.Ally && !sourceTile.GetUnit().HasGivenInstruction())
            {
                SourceSelectionTrigger(BattleManager.Trigger.VALIDATE);
                CameraEffects.Instance.TriggerZoom(mainCamera.transform.position, 2.8f, zoomSpeed);
            }
        }
    }

    private void SourceSelectionCancelInput(InputAction.CallbackContext context)
    {
        SourceSelectionTrigger(BattleManager.Trigger.CANCEL);
    }

    private void SourceSelectionMovementInput(InputAction.CallbackContext context)
    {
        Directions direction = Tools.ConvertVector2ToDirections(context.ReadValue<Vector2>());
        NavigateSource(direction);
    }

    private void SpellValidateInput(InputAction.CallbackContext context)
    {
        if (selectedSpell == null)
        {
            AudioManager.Instance.errorSound.Play();
            CameraEffects.Instance.TriggerShake(errorShakeIntensity, errorShakeDuration);
        }

        if (selectedSpell.IsAvailable())
        {
            sourceTile.Unselect();
            SpellSelectionTrigger(BattleManager.Trigger.VALIDATE);
        }
    }

    private void SpellCancelInput(InputAction.CallbackContext context)
    {
        if (spellChoice == SpellChoice.CHARACTER)
        {
            // Retour à la sélection de personnages
            sourceTile.Unselect();
            SpellSelectionTrigger(BattleManager.Trigger.CANCEL);
            CameraEffects.Instance.TriggerZoom(mainCamera.transform.position, 3f, zoomSpeed);
        }
        else
        {
            // Retour au centre
            spellChoice = SpellChoice.CHARACTER;
            overloaded = false;
            DisableSpellOverload();
        }
    }

    private void SpellMovementInput(InputAction.CallbackContext context)
    {
        BaseSpell[] currentSpells = sourceTile.GetUnit().GetFourSpells();

        switch (Tools.ConvertVector2ToDirections(context.ReadValue<Vector2>()))
        {
            //TODO Moyen de refactor tout ça
            case Directions.UP:
                // Passage en mode surchargé / Retour en mode normal
                if (spellChoice == SpellChoice.UP)
                {
                    if (!selectedSpell.IsAvailable())
                        break;
                    overloaded = !overloaded;
                    // On joue le FX d'overload
                    if (spellSelector.transform.GetChild(2).transform.Find("SpecialFX").gameObject.activeSelf)
                        spellSelector.transform.GetChild(2).transform.Find("SpecialFX").gameObject.SetActive(false);
                    spellSelector.transform.GetChild(2).transform.Find("SpecialFX").gameObject.SetActive(true);
                    spellSelector.transform.GetChild(2).GetComponent<Animator>().Play("up overloaded");
                    if (overloaded)
                    {
                        spellSelector.transform.GetChild(2).transform.Find("overloaded").gameObject.SetActive(true);
                        CameraEffects.Instance.TriggerShake(0.05f, 0.2f);
                        AudioManager.Instance.overloadSound.Play();
                        spellPanel.transform.Find("overloaded").gameObject.SetActive(true);
                    }
                    else
                    {
                        spellSelector.transform.GetChild(2).transform.Find("overloaded").gameObject.SetActive(false);
                        spellPanel.transform.Find("overloaded").gameObject.SetActive(false);
                    }
                }
                else
                {
                    overloaded = false;
                    spellChoice = SpellChoice.UP;
                    selectedSpell = currentSpells[2];
                }
                DisplaySpell();
                break;
            case Directions.DOWN:
                // Passage en mode surchargé / Retour en mode normal
                if (spellChoice == SpellChoice.DOWN)
                {
                    if (!selectedSpell.IsAvailable())
                        break;
                    overloaded = !overloaded;
                    // On joue le FX d'overload
                    if (spellSelector.transform.GetChild(3).transform.Find("SpecialFX").gameObject.activeSelf)
                        spellSelector.transform.GetChild(3).transform.Find("SpecialFX").gameObject.SetActive(false);
                    spellSelector.transform.GetChild(3).transform.Find("SpecialFX").gameObject.SetActive(true);
                    spellSelector.transform.GetChild(3).GetComponent<Animator>().Play("down overloaded");
                    if (overloaded)
                    {
                        spellSelector.transform.GetChild(3).transform.Find("overloaded").gameObject.SetActive(true);
                        CameraEffects.Instance.TriggerShake(0.05f, 0.2f);
                        AudioManager.Instance.overloadSound.Play();
                        spellPanel.transform.Find("overloaded").gameObject.SetActive(true);
                    }
                    else
                    {
                        spellSelector.transform.GetChild(3).transform.Find("overloaded").gameObject.SetActive(false);
                        spellPanel.transform.Find("overloaded").gameObject.SetActive(false);
                    }
                }
                else
                {
                    overloaded = false;
                    spellChoice = SpellChoice.DOWN;
                    selectedSpell = currentSpells[3];
                }
                DisplaySpell();
                break;
            case Directions.LEFT:
                if (spellChoice == SpellChoice.LEFT)
                {
                    // Passage en mode surchargé / Retour en mode normal
                    if (!selectedSpell.IsAvailable())
                        break;
                    overloaded = !overloaded;
                    // On joue le FX d'overload
                    if (spellSelector.transform.GetChild(0).transform.Find("SpecialFX").gameObject.activeSelf)
                        spellSelector.transform.GetChild(0).transform.Find("SpecialFX").gameObject.SetActive(false);
                    spellSelector.transform.GetChild(0).transform.Find("SpecialFX").gameObject.SetActive(true);
                    spellSelector.transform.GetChild(0).GetComponent<Animator>().Play("left overloaded");
                    if (overloaded)
                    {
                        spellSelector.transform.GetChild(0).transform.Find("overloaded").gameObject.SetActive(true);
                        CameraEffects.Instance.TriggerShake(0.05f, 0.2f);
                        AudioManager.Instance.overloadSound.Play();
                        spellPanel.transform.Find("overloaded").gameObject.SetActive(true);
                    }
                    else
                    {
                        spellSelector.transform.GetChild(0).transform.Find("overloaded").gameObject.SetActive(false);
                        spellPanel.transform.Find("overloaded").gameObject.SetActive(false);
                    }
                }
                else
                {
                    overloaded = false;
                    spellChoice = SpellChoice.LEFT;
                    selectedSpell = currentSpells[0];
                }
                DisplaySpell();
                break;
            case Directions.RIGHT:
                if (spellChoice == SpellChoice.RIGHT)
                {
                    // Passage en mode surchargé / Retour en mode normal
                    if (!selectedSpell.IsAvailable())
                        break;
                    overloaded = !overloaded;
                    // On joue le FX d'overload
                    if (spellSelector.transform.GetChild(1).transform.Find("SpecialFX").gameObject.activeSelf)
                        spellSelector.transform.GetChild(1).transform.Find("SpecialFX").gameObject.SetActive(false);
                    spellSelector.transform.GetChild(1).transform.Find("SpecialFX").gameObject.SetActive(true);
                    spellSelector.transform.GetChild(1).GetComponent<Animator>().Play("right overloaded");
                    if (overloaded)
                    {
                        spellSelector.transform.GetChild(1).transform.Find("overloaded").gameObject.SetActive(true);
                        CameraEffects.Instance.TriggerShake(0.05f, 0.2f);
                        AudioManager.Instance.overloadSound.Play();
                        spellPanel.transform.Find("overloaded").gameObject.SetActive(true);
                    }
                    else
                    {
                        spellSelector.transform.GetChild(1).transform.Find("overloaded").gameObject.SetActive(false);
                        spellPanel.transform.Find("overloaded").gameObject.SetActive(false);
                    }
                }
                else
                {
                    overloaded = false;
                    spellChoice = SpellChoice.RIGHT;
                    selectedSpell = currentSpells[1];
                }
                DisplaySpell();
                break;
            default:
                break;
        }
    }

    private void TargetSelectionValidateInput(InputAction.CallbackContext context)
    {
        if (targetTile.GetUnit() != null)
        {
            targetTile.Unselect();
            UIPanelLine.SetActive(false);
            TargetSelectionTrigger(BattleManager.Trigger.VALIDATE);
            DisableSpellOverload();
            CameraEffects.Instance.TriggerZoom(mainCamera.transform.position, 3f, zoomSpeed);
        }
    }

    private void TargetSelectionCancelInput(InputAction.CallbackContext context)
    {
        targetTile.Unselect();
        sourceTile.Select();
        TargetSelectionTrigger(BattleManager.Trigger.CANCEL);
    }

    private void TargetSelectionMovementInput(InputAction.CallbackContext context)
    {
        Directions direction = Tools.ConvertVector2ToDirections(context.ReadValue<Vector2>());
        NavigateTarget(direction);
    }

    internal void RefreshDisplays()
    {
        if (unitPanel.gameObject.activeSelf)
        {
            DisplayUnit();
        }
        if (spellPanel.gameObject.activeSelf)
        {
            DisplaySpell();
        }
        if (passivePanel.gameObject.activeSelf)
        {
            DisplayPassives();
        }
    }
}

public enum Directions { RIGHT, LEFT, UP, DOWN , UP_R, UP_L, DOWN_R, DOWN_L, NONE}
