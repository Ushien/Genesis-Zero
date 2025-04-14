using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine.Assertions;
using TMPro;

/// <summary>
/// Représente une unité au sens large
/// </summary>
public class BaseUnit : MonoBehaviour
{
        #region Fields
        #region Fields de setup

    public ScriptableUnit scriptableUnit;
    public Modifier emptyModifier;
        #endregion

        #region Références à d'autres objets

    public Tile OccupiedTile;
    public LifeBar lifeBar;
    private List<Passive> passives;
    public BaseSpell aAttack;
    public BaseSpell[] availableSpells = new BaseSpell[4];
    public Animator animator;

        #endregion

        #region Caractéristiques

    private string unit_name = "Name";
    // Puissance de l'unité
    private int basePower = 1;
    private int finalPower = 1;
    // Points de vie maximaux initiaux
    private int baseTotalHealth = 1;
    // Points de vie maximaux réels
    private int totalHealth = 1;
    // Points de vie actuels
    private int finalHealth = 1;
    // Niveau (influence la vie et la puissance)
    private int level = 1;
    [TextArea(5,10)]
    private string lore_description = "Lore Description";
    [TextArea(5,10)]
    private string fight_description = "Fight Description";
    private Team Team = Team.Enemy;
    // Combien de tours d'étourdissement restants
    private int stunTime = 0;
    // Est-ce que l'unité est étourdie ?
    private bool stun;
    // Est-ce que l'unité est hors-combat ?
    private bool dead = false;
    private int armor = 0;
    private int curatifCount = 0;
    private List<Properties> properties = new List<Properties>();
    private Vector2 position;
        #endregion

        #region Fields relatifs au moteur de jeu

    // Liste des modificateurs associés à l'unité
    private Dictionary<Func<int, BattleEvent>, List<Modifier>> modifiers = new Dictionary<Func<int, BattleEvent>, List<Modifier>>();
    private List<Modifier> globalModifiers = new List<Modifier>();
    // Liste des actions enregistrées. Le tuple est composé de 3 éléments: La méthode qui doit être appelée, le paramètre avec lequel elle doit être appelée, le nombre de tours dans lequel l'action doit être effectuée.
    private List<Tuple<Func<int, BattleEvent>, int, int>> actionQueue = new List<Tuple<Func<int, BattleEvent>, int, int>>();
    // L'unité a-t-elle déjà reçu une instruction ?
    private bool instructionGiven = false;
        #endregion
        #endregion

        #region Fields relatifs à l'interface
    private Vector3 targetLifeBarScale = Vector3.one;
    private Vector3 targetArmorBarScale = new Vector3(0, 1, 1);

        #endregion

    ////////////////////////////////////////////////////////

        #region Méthodes d'initialisation

    /// <summary>
    /// Initialise l'unité
    /// </summary>
    /// <param name="originUnit">Modèle d'unité</param>
    /// <param name="setup_level">Niveau de l'unité</param>
    /// <param name="team">Equipe de l'unité</param>
    public void Setup(ScriptableUnit originUnit, int setup_level, Team team, Vector2 _position, int healthAmount = 0, int powerAmount = 0){
        scriptableUnit = originUnit;
        name = scriptableUnit.unit_name;
        
        GetComponent<SpriteRenderer>().sprite = scriptableUnit.sprite;
        unit_name = scriptableUnit.unit_name;
        if(scriptableUnit.animator != null)
        animator.runtimeAnimatorController = scriptableUnit.animator;


        Team = team;
        position = _position;

        level = setup_level;
        Debug.Log(name + " " + healthAmount);

        if(powerAmount == 0){
            finalPower = GetStatFromLevel(scriptableUnit.original_power, level);
            basePower = finalPower;
            Debug.Log(name + " " + finalPower);
        }
        else{
            finalPower = powerAmount;
            basePower = powerAmount;
        }

        if(healthAmount == 0){
            baseTotalHealth = GetStatFromLevel(scriptableUnit.original_health, level);
            totalHealth = baseTotalHealth;
            finalHealth = baseTotalHealth;
        }
        else{
            baseTotalHealth = healthAmount;
            totalHealth = healthAmount;
            finalHealth = healthAmount;
        }

        lore_description = scriptableUnit.lore_description;
        fight_description = scriptableUnit.fight_description;

        // Il faut ajouter un modifier pour chaque méthode qui pourrait y être sujette
        modifiers[Heal] = new List<Modifier>();
        modifiers[Damage] = new List<Modifier>();

        passives = new List<Passive>();

        foreach(ScriptablePassive _passive in originUnit.passives)
        {
            _passive.SetupPassive(this);
        }

        SpellManager.Instance.SetupAttack(originUnit.aAttack, this);

        availableSpells = new BaseSpell[4];
        int i = 1;
        foreach (ScriptableSpell spell in scriptableUnit.spells)
        { 
            if(spell != null){
                SpellManager.Instance.SetupSpell(spell, this, i);
            }
            i++;
        }

        targetArmorBarScale = new Vector3((float)GetArmor()/GetTotalHealth(), 1, 1);
        lifeBar = InterfaceManager.Instance.SetupLifebar(this);
    }

    #endregion

        #region Méthodes relatives au moteur de jeu

    /// <summary>
    /// Mets à jour visuellement l'unité à chaque frame
    /// </summary>
    public void Update(){
        // Pour l'instant on fait rien si il n'y a pas encore d'animations pour les unit
        if(scriptableUnit.animator != null){
            // Rend l'unité grisée si elle a déjà reçu une instruction
            if(HasGivenInstruction() && animator.GetCurrentAnimatorStateInfo(0).IsName("idle")){
                //gameObject.GetComponent<SpriteRenderer>().color = new Color32( 173, 173, 173, 200);
                animator.Play("prepared");
            }
            //else
            if(!HasGivenInstruction() && !animator.GetCurrentAnimatorStateInfo(0).IsName("idle")){
                //gameObject.GetComponent<SpriteRenderer>().color = new Color32( 255, 255, 255, 255);
                animator.Play("idle");
            }
        }
    }

    /// <summary>
    /// Rend l'unité prête pour le combat suivant.
    /// Les HP et certains états sont maintenus, le reste est remis à jour en conséquence.
    /// </summary>
    public void EndBattle(){
        GiveInstruction(false);
        Cleanse();
        SetArmor(0);
        modifiers[Heal] = new List<Modifier>();
        modifiers[Damage] = new List<Modifier>();
        actionQueue = new List<Tuple<Func<int, BattleEvent>, int, int>>();
        ModifiersEndBattle();
        DeleteMinorPassives();

        foreach (BaseSpell spell in GetSpells(true))
        {
            spell.Activate(false);
            if(spell != null){
                spell.EndBattle();
            }
        }
    }

    public void StartBattle(){
        foreach (Passive passive in GetPassives())
        {
            if(!passive.IsActivated()){
                passive.Activate(true);
                passive.Activate();
            }
        }

        foreach (BaseSpell spell in GetSpells(true)){
            if(!spell.IsActivated()){
                spell.Activate(true);
                spell.Activate();
            }
        }
    }

    /// <summary>
    /// Applique tous les effets de fin de tour liés à l'unité
    /// </summary>
    public void ApplyEndTurnEffects(){
        // Diminue le temps d'étourdissement de 1
        ModifyStunTime(-1);
        // Applique les effets de fin de tours de chacun des sorts de l'unité
        foreach (BaseSpell spell in GetSpells())
        {
            if(spell != null){
                spell.ApplyEndTurnEffects(); 
            }
        }
        GetAttack().ApplyEndTurnEffects();
        // Modifiers
        ModifiersEndTurn();
        // ActionQueue
        ReduceQueueTurns();
        ApplyQueueActions();
    }

    public void ApplyStartTurnEffects(){
        foreach (BaseSpell spell in GetSpells())
        {
            if(spell != null){
                spell.ApplyStartTurnEffects(); 
            }
        }
    }

    /// <summary>
    /// Utilise l'attaque de l'unité
    /// </summary>
    /// <param name="targetTile"></param>
    public void Attack(Tile targetTile, List<Properties> propertiesToApply){
        aAttack.Cast(targetTile, propertiesToApply);
    }

    /// <summary>
    /// L'unité peut-elle recevoir une instruction
    /// </summary>
    /// <returns></returns>
    public bool IsAvailable(){
        bool available = true;
        if(dead){
            available = false;
        }
        if(stun){
            available = false;
        }
        return available;
    }

    public bool IsDead(){
        return dead;
    }

    public void Revive(int hpAmount = 1){
        Assert.AreNotEqual(hpAmount, 0, "On ne peut pas revive une unité à zéro HP");
        if(dead){
            SetHP(hpAmount);
            // Comment gérer lorsqu'une unité est déjà présente sur la case ?
            OccupiedTile.SetUnit(this);
            dead = false;
            BattleEventManager.Instance.ApplyReviveEvent(BattleEventManager.Instance.CreateReviveEvent(this, hpAmount));
        }
    }

    public void Summon(ScriptableUnit summon, int healthAmount =  0, int powerAmount = 0){
        Tile summonTile = GridManager.Instance.GetRandomTile(GetTeam(), true);
        if(summonTile != null){
            BaseUnit summoned = UnitManager.Instance.CreateUnit(summonTile.GetPosition(), summon, GetLevel(), GetTeam(), healthAmount : healthAmount, powerAmount : powerAmount);
            UnitManager.Instance.SpawnUnit(summoned, GetTeam());
            BattleEventManager.Instance.ApplySummonEvent(BattleEventManager.Instance.CreateSummonEvent(summoned, summonTile, this));
            summoned.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Renvoie la valeur d'une statistique du niveau 100 adaptée à un autre niveau
    /// </summary>
    /// <param name="level_100_stat"></param>
    /// <param name="real_level"></param>
    /// <returns></returns>
    public int GetStatFromLevel(int level_100_stat, int real_level){

        var level_1_stat = (float)level_100_stat/10;
        var growth_by_level = (level_100_stat-level_1_stat)/99;
        var real_level_stat = Tools.Ceiling(level_1_stat+growth_by_level*(real_level-1));

        return real_level_stat;
    }

    public void CastSpell(BaseSpell spell, bool hyper){
        Assert.IsTrue(GetSpells(true).Contains(spell), "L'unité ne peut pas lancer un sort sans le posséder");
        if(hyper){
            spell.HyperCast(GetProperties());
        }
        else{
            spell.Cast(GetProperties());
        }
    }

    public void CastSpell(BaseSpell spell, Tile targetTile, bool hyper){
        Assert.IsTrue(GetSpells(true).Contains(spell));
        if(hyper){
            spell.HyperCast(targetTile, GetProperties());
        }
        else{
            spell.Cast(targetTile, GetProperties());
        }
    }

    /// <summary>
    /// Indique si l'unité a reçu une instruction ou non
    /// </summary>
    /// <param name="newInstructionGiven"></param>
    public void GiveInstruction(bool newInstructionGiven){
        instructionGiven = newInstructionGiven;
    }

    /// <summary>
    /// L'unité a-t-elle reçu une instruction
    /// </summary>
    /// <returns></returns>
    public bool HasGivenInstruction(){
        return instructionGiven;
    }

        #region Gestion de la queue d'actions

    /// <summary>
    /// Ajoute une action à la queue
    /// </summary>
    /// <param name="action"></param>
    /// <param name="parameter"></param>
    /// <param name="howManyTurns"></param>
    public void QueueAction(Func<int, BattleEvent> action, int parameter, int howManyTurns){
        actionQueue.Add(new Tuple<Func<int, BattleEvent>, int, int>(action, parameter, howManyTurns));
    }

    /// <summary>
    /// Réduit de 1 le nombre de tours de la queue d'actions
    /// </summary>
    public void ReduceQueueTurns(){
        List<Tuple<Func<int, BattleEvent>, int, int>> newQueue = new List<Tuple<Func<int, BattleEvent>, int, int>>();

        foreach (Tuple<Func<int, BattleEvent>, int, int> queuedAction in actionQueue)
        {
            newQueue.Add(new Tuple<Func<int, BattleEvent>, int, int>(queuedAction.Item1, queuedAction.Item2, queuedAction.Item3 - 1));
        }

        actionQueue = newQueue;
    }

    /// <summary>
    /// Applique les actions de la queue dont le nombre de tour restants est de 0. Les supprime ensuite de la queue.
    /// </summary>
    public void ApplyQueueActions(){
        foreach (Tuple<Func<int, BattleEvent>, int, int> queuedAction in actionQueue)
        {
            if(queuedAction.Item3 == 0){
                queuedAction.Item1(queuedAction.Item2);
            }
        }

        List<Tuple<Func<int, BattleEvent>, int, int>> newQueue = new List<Tuple<Func<int, BattleEvent>, int, int>>();

        foreach (Tuple<Func<int, BattleEvent>, int, int> queuedAction in actionQueue)
        {
            if(queuedAction.Item3 != 0){
                newQueue.Add(new Tuple<Func<int, BattleEvent>, int, int>(queuedAction.Item1, queuedAction.Item2, queuedAction.Item3));
            }
        }

        actionQueue = newQueue;
    }

    public int GetAvailableSpellIndex(){
        for (int index = 0; index <= 3; index++)
        {
            if(availableSpells[index] == null){
                return index;
            }
        }
        return -1;
    }

    public int GetSpellIndex(BaseSpell spellToFind){
        List<BaseSpell> spellList = GetSpells(includingAttack : true);
        foreach (BaseSpell spell in spellList)
        {    
            if(spell.GetScriptableSpell() == spellToFind.GetScriptableSpell()){
                return spell.GetIndex();
            }    
        }
        return -1;
    }

    public BaseSpell GetSpellByIndex(int index){
        foreach (BaseSpell spell in GetSpells(includingAttack:true))
        {
            if(spell.GetIndex() == index){
                return spell;
            }
        }
        return null;
    }

    public bool HasSpell(ScriptableSpell _spell){
        foreach (BaseSpell spell in availableSpells)
        {
            if(spell != null){
                if(_spell == spell.GetScriptableSpell()){
                    return true;
                }
            }
        }
        return false;
    }

    /// <summary>
    /// Renvoie un passif précis, si l'unité en possède une copie.
    /// </summary>
    /// <param name="_passive"></param>
    /// <returns></returns>
    public Passive GetPassive(ScriptablePassive _passive){
        foreach (Passive passive in GetPassives())
        {
            if(passive != null){
                if(_passive == passive.GetScriptablePassive()){
                    return passive;
                }
            }
        }
        return null;  
    }

    public bool HasPassive(ScriptablePassive _passive){
        foreach (Passive passive in GetPassives())
        {
            if(passive != null){
                if(_passive == passive.GetScriptablePassive()){
                    return true;
                }
            }
        }
        return false; 
    }

    public bool HasPassive(Passive _passive){
        foreach (Passive passive in GetPassives())
        {
            if(passive != null){
                if(_passive.GetScriptablePassive() == passive.GetScriptablePassive()){
                    return true;
                }
            }
        }
        return false; 
    }

    public void DeleteMinorPassives(){
        List<Passive> passivesToDelete = GetPassives().Where(_passive => _passive.IsMinor()).ToList();
        foreach (Passive passive in passivesToDelete)
        {
            DeletePassive(passive);
        }
    }

    #endregion

        #region Gestion des modificateurs

    public void AddGlobalModifier(Modifier modifier){
        globalModifiers.Add(modifier);
        modifier.transform.SetParent(transform);
        CheckModifiers();
    }

    public void DeleteGlobalModifier(Modifier modifier){
        globalModifiers.Remove(modifier);
        CheckModifiers();
    }

    public void DeleteGlobalModifier(List<Modifier> modifiers){
        foreach (Modifier _modifier in modifiers)
        {
            DeleteGlobalModifier(_modifier);
        }
    }

    /// <summary>
    /// Ajoute un modificateur sur une méthode
    /// </summary>
    /// <param name="modifier"></param>
    /// <param name="function"></param>
    public void AddModifier(Modifier modifier, Func<int, BattleEvent> function){
        modifiers[function].Add(modifier);
        modifier.transform.SetParent(transform);
        CheckModifiers();
    }

    /// <summary>
    /// Supprime un modificateur sur une méthode
    /// </summary>
    /// <param name="modifier"></param>
    /// <param name="function"></param>
    public void DeleteModifier(Modifier modifier, Func<int, BattleEvent> function){
        modifiers[function].Remove(modifier);
        CheckModifiers();
    }

    /// <summary>
    /// Diminue la durée restante des modificateurs de 1 et les supprime si celle-ci vaut 0
    /// </summary>
    private void ModifiersEndTurn(){
        foreach (Modifier modifier in globalModifiers){
            modifier.ReduceTurns();
        }
        foreach (KeyValuePair<Func<int, BattleEvent>, List<Modifier>> action in modifiers)
        {
            foreach (Modifier _modifier in action.Value)
            {
                _modifier.ReduceTurns();
            }
        }
        CheckModifiers();
    }

    public void ModifiersEndBattle(){
        foreach (Modifier modifier in globalModifiers){
            modifier.EndBattle();
        }
        foreach (KeyValuePair<Func<int, BattleEvent>, List<Modifier>> action in modifiers)
        {
            foreach (Modifier _modifier in action.Value)
            {
                _modifier.EndBattle();
            }
        }
        CheckModifiers();
    }

    private void CheckModifiers(){
        properties = new List<Properties>();
        int newPower = basePower;
        int newTotalHealth = baseTotalHealth;
        float currentHPRAtio = (float)finalHealth / (float)totalHealth;

        // Check the global modifiers

        for (int i = 0; i < globalModifiers.Count; i++){
            Modifier modifier = globalModifiers[i];
            if(modifier.IsEnded()){
                globalModifiers[i] = null;
                Destroy(modifier.gameObject);
            }
            else{
                if(modifier.powerBonus != 0f){
                    newPower = Tools.Ceiling(modifier.powerBonus * newPower + newPower);
                }
                if(modifier.healthBonus != 0f){
                    newTotalHealth = Tools.Ceiling(modifier.healthBonus * newTotalHealth + newTotalHealth);
                }
                if(modifier.properties.Contains(Properties.Curatif)){
                    properties.Add(Properties.Curatif);
                }
            }
        }

        // Update the power and HP
        
        int previousTotalHealth = totalHealth;
        int previousFinalHealth = finalHealth;
        int newFinalHealth = Tools.Ceiling(newTotalHealth * currentHPRAtio);

        totalHealth = newTotalHealth;
        if(totalHealth != previousTotalHealth){
            BattleEventManager.Instance.ApplyHPModificationEvent(BattleEventManager.Instance.CreateHPModificationEvent(this, previousTotalHealth, totalHealth, true));
        }
        
        finalHealth = newFinalHealth;
        if(newFinalHealth != previousFinalHealth){
            BattleEventManager.Instance.ApplyHPModificationEvent(BattleEventManager.Instance.CreateHPModificationEvent(this, previousFinalHealth, newFinalHealth, false));
        }

        finalPower = newPower;
        CheckFinalPower();

        // Check the effect modifiers
        
        foreach (var action in modifiers)
        {
            for (int i = 0; i < action.Value.Count; i++)
            {
                Modifier modifier = action.Value[i];
                if(modifier.IsEnded()){
                    action.Value[i] = null;
                    Destroy(modifier.gameObject);
                }
            }
        }
        

        // Remove the holes from the lists

        globalModifiers.RemoveAll(item => item == null);
        foreach (var action in modifiers)
        {
            action.Value.RemoveAll(item => item == null);
        }
    }

    private List<Properties> GetProperties(){

        // TODO add toutes les propriétés ici

        List<Properties> newList = properties;
        if(curatifCount > 0){
            curatifCount -= 1;
            if(!newList.Contains(Properties.Curatif)){
                newList.Add(Properties.Curatif);
            };
        }

        return newList;
    }

    #endregion
        #endregion

        #region Manipulation des caractéristiques de l'unité

    /// <summary>
    /// Renvoie le nom de l'unité
    /// </summary>
    /// <returns></returns>
    public string GetName(){
        return unit_name;
    }

    /// <summary>
    /// Renvoie la description en combat de l'unité
    /// </summary>
    /// <returns></returns>
    public string GetFightDescription(){
        return fight_description;
    }

    /// <summary>
    /// Renvoie le niveau de l'unité
    /// </summary>
    /// <returns></returns>
    public int GetLevel(){
        return level;
    }

    /// <summary>
    /// Renvoie l'équipe de l'unité
    /// </summary>
    /// <returns></returns>
    public Team GetTeam(){
        return Team;
    }

    public void SetTeam(Team _team){
        Team = _team;
    }

    public Vector2 GetPosition(){
        return position;
    }

    public List<Passive> GetPassives(){
        return passives;
    }

    public void AddPassive(Passive newPassive){
        passives.Add(newPassive);
        CheckModifiers();
    }

    public void DeletePassive(Passive passiveToDelete){
        passiveToDelete.Desactivate();
        passives.Remove(passiveToDelete);
        CheckModifiers();
    }

    /// <summary>
    /// Renvoie la liste de techniques de l'unité
    /// </summary>
    /// <param name="includingAttack">Si fixé à True, intègre l'attaque de l'unité à la liste, à l'index 4</param>
    /// <returns></returns>
    public List<BaseSpell> GetSpells(bool includingAttack = false, bool availableOnly = false){

        List<BaseSpell> completeAvailableSpells = new List<BaseSpell>();

        foreach(BaseSpell spell in availableSpells){
            if(spell != null){
                if(!(availableOnly && !spell.IsAvailable())){
                    completeAvailableSpells.Add(spell);
                }
            }
        }

        if(includingAttack){
            completeAvailableSpells.Add(GetAttack());
        }

        return completeAvailableSpells;
    }

    public BaseSpell[] GetFourSpells(bool includingAttack = false){

        if(includingAttack){
            
            BaseSpell[] completeAvailableSpells = new BaseSpell[5];
            int i = 0;
            foreach(BaseSpell spell in availableSpells){
                completeAvailableSpells[i] = spell;
                i++;
            }
            completeAvailableSpells[4] = GetAttack();

            return completeAvailableSpells;
        }

        else{
            return availableSpells;
        }

    }

    /// <summary>
    /// Supprime un sort donné de la liste de sorts
    /// </summary>
    /// <param name="spell_to_remove"></param>
    public void RemoveSpell(BaseSpell spell_to_remove){
        int i = 0;
        foreach (BaseSpell _spell in GetSpells())
        {
            if (_spell == spell_to_remove)
            {
                spell_to_remove.SetOwner(null);
                RemoveSpell(i);
            }
            i++;
        }
    }

    /// <summary>
    /// Supprime un sort de la liste de sorts, par index
    /// </summary>
    /// <param name="index"></param>
    public void RemoveSpell(int index){
        availableSpells[index] = null;
    }

    /// <summary>
    /// Renvoie une technique aléatoire de l'unité
    /// </summary>
    /// <param name="includingAttack">Si fixé à True, intègre l'attaque de l'unité à la liste</param>
    /// <returns></returns>
    public BaseSpell GetRandomSpell(bool includingAttack = true, bool availableOnly = false){
        return GetSpells(includingAttack, availableOnly).Where(t => t != null).OrderBy(t => UnityEngine.Random.value).First();
    }

    /// <summary>
    /// Renvoie l'attaque de l'unité
    /// </summary>
    /// <returns></returns>
    public BaseSpell GetAttack(){
        return aAttack;
    }

    /// <summary>
    /// Renvoie la case occupée par l'unité
    /// </summary>
    /// <returns></returns>
    public Tile GetTile(){
        return OccupiedTile;
    }

        #region Gestion de la puissance

    /// <summary>
    /// Renvoie la puissance de l'unité
    /// </summary>
    /// <returns></returns>
    public int GetFinalPower(){
        return finalPower;
    }

    /// <summary>
    /// Fixe la puissance de l'unité
    /// </summary>
    /// <param name="amount"></param>
    public void SetFinalPower(int amount){
        finalPower = amount;
    }

    /// <summary>
    /// Modifie la puissance de l'unité d'un certain pourcentage
    /// </summary>
    /// <param name="amount"></param>
    public void ModifyPower(float amount){
        SetFinalPower(Tools.Ceiling(amount * finalPower + finalPower));
        CheckFinalPower();
    }

    /// <summary>
    /// Vérifie si la puissance de l'unité est légale et la réajuste au besoin
    /// </summary>
    private void CheckFinalPower(){
        if(GetFinalPower() < 0){
            SetFinalPower(0);
        }
    }
        #endregion
        
        #region Gestion des HP

    /// <summary>
    /// Renvoie les HP totaux
    /// </summary>
    /// <returns>
    /// HP totaux de l'unité
    /// </returns>
    public int GetTotalHealth(){
        return totalHealth;
    }

    public int GetBaseTotalHealth(){
        return baseTotalHealth;
    }

    /// <summary>
    /// Renvoie les HP actuels
    /// </summary>
    /// <returns>
    /// HP actuels de l'unité
    /// </returns>
    public int GetFinalHealth(){
        return finalHealth;
    }

    /*

    /// <summary>
    /// Ajoute une quantité de HP aux HP actuels et totaux
    /// </summary>
    /// <param name="amount"></param>
    public void ModifyBothHP(int amount){
        ModifyTotalHP(amount);
        ModifyHP(amount);
    }
    */

    /// <summary>
    /// Ajoute une quantité de HP au nombre de HP actuels
    /// </summary>
    /// <param name="amount"></param>
    private int ModifyHP(int amount){
        finalHealth += amount;
        return CheckHP();
    }

    /*
    /// <summary>
    /// Ajoute une quantité de HP au nombre de HP totaux
    /// </summary>
    /// <param name="amount">
    /// Nombre de HP à ajouter
    /// </param>
    public void ModifyTotalHP(int amount){
        baseTotalHealth += amount;
        CheckTotalHP();
    }

    /// <summary>
    /// Multiplie les HP actuels et les HP totaux de l'unité par un facteur donné
    /// </summary>
    /// <param name="amount"></param>
    public void MultiplyBothHP(int amount){
        MultiplyTotalHP(amount);
        MultiplyHP(amount);
    }

    /// <summary>
    /// Multiplie les HP actuels de l'unité par un facteur donné
    /// </summary>
    /// <param name="amount"></param>
    public void MultiplyHP(int amount){
        finalHealth *= amount;
        CheckHP();
    }

    /// <summary>
    /// Multiplie les HP totaux de l'unité par un facteur donné
    /// </summary>
    /// <param name="amount"></param>
    public void MultiplyTotalHP(int amount){
        baseTotalHealth *= amount;
        CheckTotalHP();
        ModifyTotalHP(amount);
    }
    */

    /// <summary>
    /// Fixe les HP actuels de l'unité à une valeur donnée
    /// </summary>
    /// <param name="HP"></param>
    /// <param name="check"></param>
    public void SetHP(int HP, bool check = true){
        finalHealth = HP;
        if(check){
            CheckHP();
        }
    }

    /// <summary>
    /// Fixe les HP totaux de l'unité à une valeur donnée
    /// </summary>
    /// <param name="HP"></param>
    /// <param name="check"></param>
    public void SetTotalHP(int HP, bool check = true){
        totalHealth = HP;
        if(check){
            CheckTotalHP();
        }
    }

    /// <summary>
    /// Vérifie que les HP actuels de l'unité sont à légaux et les fixe au besoin. Applique également le statut KO de l'unité si ses HP sont inférieurs ou égaux à 0.
    /// </summary>
    public int CheckHP(){
        int difference = 0;
        if(AreHPBeyondMax()){
            difference = GetTotalHealth()-GetFinalHealth();
            SetHP(GetTotalHealth(), false);
        }
        if(AreHPBelowZero()){
            difference = -GetFinalHealth();
            SetHP(0, false);
            //InterfaceManager.Instance.KillLifeBar(this.lifeBar);
            Kill();
        }
        return difference;
    }

    /// <summary>
    /// Vérifie que les HP totaux de l'unité sont légaux et les fixe au besoin
    /// </summary>
    public void CheckTotalHP(){
        int totalHP = GetTotalHealth();
        if(AreTotalHPBelowZero()){
            SetTotalHP(0, false);
        }
    }

    /// <summary>
    /// Renvoie True si les HP actuels de l'unité sont inférieurs à zéro
    /// </summary>
    /// <returns></returns>
    public bool AreHPBelowZero(){
        return GetFinalHealth() <= 0;
    }

    /// <summary>
    /// Renvoie True si les HP totaux de l'unité sont inférieurs à zéro
    /// </summary>
    /// <returns></returns>
    public bool AreTotalHPBelowZero(){
        return GetTotalHealth() <= 0;
    }

    /// <summary>
    /// Renvoie True si les HP actuels de l'unité dépassent ses HP totaux
    /// </summary>
    /// <returns></returns>
    public bool AreHPBeyondMax(){
        return GetFinalHealth() >= GetTotalHealth();
    }
    #endregion

        #region Gestion de l'armure

    /// <summary>
    /// Renvoie la quantité d'armure possédée par l'unité
    /// </summary>
    /// <returns></returns>
    public int GetArmor(){
        return armor;
    }

    /// <summary>
    /// Fixe l'armure de l'unité à une valeur donnée
    /// </summary>
    /// <param name="amount"></param>
    public void SetArmor(int amount){
        if(amount >= 0){
            if(amount != GetArmor()){
                ModifyArmor(amount - GetArmor());
            }
        }
    }

    /// <summary>
    /// Ajoute une quantité d'armure donnée à l'unité
    /// </summary>
    /// <param name="amount"></param>
    public int ModifyArmor(int amount, bool animation = true){
        armor += amount;
        if(amount > 0){
            BattleEventManager.Instance.ApplyArmorGainEvent(BattleEventManager.Instance.CreateArmorGainEvent(this, amount, animation));
        }
        return CheckArmor();
    }

    /// <summary>
    /// Vérifie si l'armure de l'unité est légale et la fixe au besoin.
    /// </summary>
    public int CheckArmor(){
        int armorDifference = 0;
        if(armor < 0){
            armorDifference = -armor;
            SetArmor(0);
        }
        return armorDifference;
    }

    /// <summary>
    /// Renvoie True si l'unité possède de l'armure
    /// </summary>
    /// <returns></returns>
    public bool isArmored(){
        return armor > 0;
    }

    /*
    /// <summary>
    /// Convertit une quantité donnée d'armure de l'unité en points de vie
    /// Si l'unité ne possède pas assez d'armure, la méthode ne fait rien
    /// </summary>
    /// <param name="amount"></param>
    public BattleEvent ConvertArmorIntoHP(int amount){
        if(GetArmor() >= amount){
            ModifyArmor(-amount);
            ModifyBothHP(amount);
        }
        return null;
    }
    */
    #endregion

    /// <summary>
    /// Inflige un montant de dégats à l'unité
    /// </summary>
    /// <param name="amount"></param>
    public DamageEvent Damage(int amount){

        int finalDamage = amount - GetArmor();

        // Si l'armure ne suffit pas à tanker l'attaque
        if(finalDamage > 0){
            int armorDamages = 0;
            // Si l'unité possède de l'armure, retire l'armure
            if(GetArmor() > 0){
                armorDamages = GetArmor();
                ModifyArmor(-GetArmor());
            }
            int difference = ModifyHP(-finalDamage);
            return BattleEventManager.Instance.CreateDamageEvent(this, finalDamage - difference, armorDamages);
        }
        // Si l'armure suffit à tanker l'attaque
        else{
            ModifyArmor(-amount);
            return BattleEventManager.Instance.CreateDamageEvent(this, 0, amount);
        }
    }

    /// <summary>
    /// Soigne l'unité d'un montant donné
    /// </summary>
    /// <param name="amount"></param>
    public HealEvent Heal(int amount){
        int finalAmount = amount;
        foreach (Modifier _modifier in modifiers[Heal])
        {
            finalAmount = Tools.Ceiling(_modifier.GetNewAmount(finalAmount));
        }
        int result = ModifyHP(+finalAmount);
        return BattleEventManager.Instance.CreateHealEvent(null, this, finalAmount + result, true);
    }

        #region Gestion des états

    /// <summary>
    /// Fixe le nombre de tours d'étourdissement de l'unité à une valeur donnée
    /// </summary>
    /// <param name="amount"></param>
    public void SetStunTime(int amount){
        stunTime = amount;
        CheckStun();
    }

    /// <summary>
    /// Ajoute un certain nombre de tours d'étourdissement à l'unité
    /// </summary>
    /// <param name="amount"></param>
    public void ModifyStunTime(int amount){
        stunTime += amount;
        CheckStun();
    }

    /// <summary>
    /// Vérifie si l'unité est étourdie et ajuste la variable stun en fonction
    /// </summary>
    public void CheckStun(){
        if (stunTime > 0){
            stun = true;
        }
        else{
            stun = false;
        }
    }

    /// <summary>
    /// Supprime l'état négatif passé en paramètre
    /// </summary>
    /// <param name="status"></param>
    public void Cleanse(Status status){
        switch (status)
        {
            case Status.Stun:
                SetStunTime(0);
                break;

            default:
                break;
        }
    }

    /// <summary>
    /// Supprime la liste d'états négatifs passés en paramètre
    /// </summary>
    /// <param name="statusList"></param>
    public void Cleanse(List<Status> statusList){

        foreach (Status status in statusList)
        {
            Cleanse(status);
        }
    }

    /// <summary>
    /// Supprime tous les états négatifs possibles
    /// </summary>
    public void Cleanse(){
        Cleanse(SpellManager.Instance.GetCleansableStatus());
    }

    public void ModifyCuratifCount(int amount){
        curatifCount += amount;
    }

    public int GetCuratifCount(){
        return curatifCount;
    }

    /// <summary>
    /// A utiliser pour ajouter l'état dead.
    /// </summary>
    public void Kill(){
        dead = true;
        SetHP(0, false);
        BattleEventManager.Instance.ApplyDeathEvent(BattleEventManager.Instance.CreateDeathEvent(this, GetTile()));
        OccupiedTile.SetUnit(null);
        EndBattle();
    }

        #endregion

    public void CheckAssertions(){
        //Assert.AreEqual(GetFinalHealth().ToString() + " HP", lifeBar.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text, "La barre de HP et les valeurs réelles sont désynchronisées (HP finaux)");
        Assert.AreEqual(GetArmor().ToString() + " AR", lifeBar.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text, "La barre d'armure et les valeurs réelles sont désynchronisées");
        // Check que l'unité n'a aucun passif en doublon
        Assert.AreEqual(GetPassives()
            .Where(i => !i.IsMinor())
            .GroupBy(i => i.GetScriptablePassive())
            .Where(g => g.Count() > 1)
            .Select(g => g.Key).Count(), 0, "Il est interdit de posséder plusieurs fois le même passif");
        // Check que l'unité n'a aucun spell en doublon
        Assert.AreEqual(GetSpells(true)
            .GroupBy(i => i.GetScriptableSpell())
            .Where(g => g.Count() > 1)
            .Select(g => g.Key).Count(), 0, "Il est interdit de posséder plusieurs fois le même sort");
    }

    #endregion

}