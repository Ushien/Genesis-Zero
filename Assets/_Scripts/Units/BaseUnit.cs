using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Unity.VisualScripting;

/// <summary>
/// Représente une unité au sens large
/// </summary>
public class BaseUnit : MonoBehaviour
{
        #region Fields
        #region Fields de setup

    public ScriptableUnit scriptableUnit;
    public ScriptableJob scriptableJob;
    public Passive emptyPassive;
    public Modifier emptyModifier;
        #endregion

        #region Références à d'autres objets

    public Tile OccupiedTile;
    public Passive passive;
    public BaseSpell attack;
    public List<BaseSpell> availableSpells;
    //TODO Supprimer les 2 sorts
        #endregion

        #region Caractéristiques

    public string unit_name = "Name";
    // Puissance de l'unité
    public int finalPower = 1;
    // Points de vie maximaux
    public int totalHealth = 1;
    // Points de vie actuels
    public int finalHealth = 1;
    // Niveau (influence la vie et la puissance)
    public int level = 1;
    [TextArea(5,10)]
    public string lore_description = "Lore Description";
    [TextArea(5,10)]
    public string fight_description = "Fight Description";
    public Team Team = Team.Enemy;
    // Combien de tours d'étourdissement restants
    public int stunTime = 0;
    // Est-ce que l'unité est étourdie ?
    public bool stun;
    // Est-ce que l'unité est hors-combat ?
    public bool dead = false;
    public int armor = 0;
        #endregion

        #region Fields relatifs au moteur de jeu

    // Liste des modificateurs associés à l'unité
    public Dictionary<Action<int>, List<Modifier>> modifiers = new Dictionary<Action<int>, List<Modifier>>();
    // Liste des actions enregistrées. Le tuple est composé de 3 éléments: La méthode qui doit être appelée, le paramètre avec lequel elle doit être appelée, le nombre de tours dans lequel l'action doit être effectuée.
    private List<Tuple<Action<int>, int, int>> actionQueue = new List<Tuple<Action<int>, int, int>>();
    // L'unité a-t-elle déjà reçu une instruction ?
    private bool instructionGiven = false;
        #endregion
        #endregion

    ////////////////////////////////////////////////////////

        #region Méthodes d'initialisation

    /// <summary>
    /// Initialise l'unité
    /// </summary>
    /// <param name="originUnit">Modèle d'unité</param>
    /// <param name="setup_level">Niveau de l'unité</param>
    /// <param name="team">Equipe de l'unité</param>
    public void Setup(ScriptableUnit originUnit, int setup_level, Team team){
        scriptableUnit = originUnit;
        this.name = scriptableUnit.unit_name;
        
        this.GetComponent<SpriteRenderer>().sprite = scriptableUnit.sprite;
        unit_name = scriptableUnit.unit_name;

        Team = team;
        level = setup_level;

        finalPower = GetStatFromLevel(scriptableUnit.original_power, level);
        totalHealth = GetStatFromLevel(scriptableUnit.original_health, level);
        finalHealth = totalHealth;

        lore_description = scriptableUnit.lore_description;
        fight_description = scriptableUnit.fight_description;

        // Il faut ajouter un modifier pour chaque méthode qui pourrait y être sujette
        modifiers[Heal] = new List<Modifier>();
        modifiers[Damage] = new List<Modifier>();

        if(scriptableUnit.passive != null){
            passive = Instantiate(scriptableUnit.passive);
        }
        else{
            passive = Instantiate(emptyPassive);
        }

        passive.Setup(this);

        attack = SpellManager.Instance.SetupAttack(this);

        foreach (BaseSpell spell in scriptableUnit.spells)
        {
            availableSpells.Add(SpellManager.Instance.SetupSpell(spell, this));
        }
        if (scriptableUnit.scriptableJob != null){
            LoadJob(scriptableUnit.scriptableJob);
        }
    }

    /// <summary>
    /// Charger la classe de l'unité
    /// </summary>
    /// <param name="scriptableJob"></param>
    public void LoadJob(ScriptableJob scriptableJob){
        foreach (BaseSpell spell in scriptableJob.spells)
        {
            availableSpells.Add(SpellManager.Instance.SetupSpell(spell, this));
        }
        if (scriptableJob.passive != null){
            Destroy(passive.gameObject);
            passive = Instantiate(scriptableJob.passive);
            passive.Setup(this);
        }
    }

    #endregion

        #region Méthodes relatives au moteur de jeu

    /// <summary>
    /// Mets à jour visuellement l'unité à chaque frame
    /// </summary>
    public void Update(){
        // Rend l'unité grisée si elle a déjà reçu une instruction
        if(HasGivenInstruction()){
            this.gameObject.GetComponent<SpriteRenderer>().color = new Color32( 173, 173, 173, 200);
        }
        else{
            this.gameObject.GetComponent<SpriteRenderer>().color = new Color32( 255, 255, 255, 255);
        }
    }
    
    /// <summary>
    /// Applique tous les effets de fin de tour liés à l'unité
    /// </summary>
    public void ApplyEndturnEffects(){
        // Diminue le temps d'étourdissement de 1
        ModifyStunTime(-1);
        // Applique les effets de fin de tours de chacun des sorts de l'unité
        foreach (BaseSpell spell in GetSpells())
        {
            spell.ApplyEndTurnEffects(); 
        }
        GetAttack().ApplyEndTurnEffects();
        // Modifiers
        ModifierEndTurn();
        // ActionQueue
        ReduceQueueTurns();
        ApplyQueueActions();
    }

    /// <summary>
    /// Utilise l'attaque de l'unité
    /// </summary>
    /// <param name="targetTile"></param>
    public void Attack(Tile targetTile){
        attack.Cast(targetTile);
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

    /// <summary>
    /// Active une technique de l'unité
    /// </summary>
    /// <param name="index"></param>
    public void CastSpell(int index){
        availableSpells[index].Cast();
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
    public void QueueAction(Action<int> action, int parameter, int howManyTurns){
        actionQueue.Add(new Tuple<Action<int>, int, int>(action, parameter, howManyTurns));
    }

    /// <summary>
    /// Réduit de 1 le nombre de tours de la queue d'actions
    /// </summary>
    public void ReduceQueueTurns(){
        List<Tuple<Action<int>, int, int>> newQueue = new List<Tuple<Action<int>, int, int>>();

        foreach (Tuple<Action<int>, int, int> queuedAction in actionQueue)
        {
            newQueue.Add(new Tuple<Action<int>, int, int>(queuedAction.Item1, queuedAction.Item2, queuedAction.Item3 - 1));
        }

        actionQueue = newQueue;
    }

    /// <summary>
    /// Applique les actions de la queue dont le nombre de tour restants est de 0. Les supprime ensuite de la queue.
    /// </summary>
    public void ApplyQueueActions(){
        foreach (Tuple<Action<int>, int, int> queuedAction in actionQueue)
        {
            if(queuedAction.Item3 == 0){
                queuedAction.Item1(queuedAction.Item2);
            }
        }

        List<Tuple<Action<int>, int, int>> newQueue = new List<Tuple<Action<int>, int, int>>();

        foreach (Tuple<Action<int>, int, int> queuedAction in actionQueue)
        {
            if(queuedAction.Item3 != 0){
                newQueue.Add(new Tuple<Action<int>, int, int>(queuedAction.Item1, queuedAction.Item2, queuedAction.Item3));
            }
        }

        actionQueue = newQueue;
    }
    #endregion

        #region Gestion des modificateurs

    /// <summary>
    /// Ajoute un modificateur sur une méthode
    /// </summary>
    /// <param name="modifier"></param>
    /// <param name="function"></param>
    public void AddModifier(Modifier modifier, Action<int> function){
        modifiers[function].Add(modifier);
    }

    /// <summary>
    /// Supprime un modificateur sur une méthode
    /// </summary>
    /// <param name="modifier"></param>
    /// <param name="function"></param>
    public void DeleteModifier(Modifier modifier, Action<int> function){
        modifiers[function].Remove(modifier);
    }

    /// <summary>
    /// Diminue la durée restante des modificateurs de 1 et les supprime si celle-ci vaut 0
    /// </summary>
    private void ModifierEndTurn(){
        foreach (var action in modifiers)
        {
            foreach (Modifier _modifier in action.Value)
            {
                _modifier.ModifyTurns(-1);
                if(_modifier.IsEnded()){
                    //FIXME this les listes aiment pas beaucoup ça
                    action.Value.Remove(_modifier);
                }
            }
        }
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

    /// <summary>
    /// Renvoie le passif de l'unité
    /// </summary>
    /// <returns></returns>
    public Passive GetPassive(){
        return passive;
    }

    /// <summary>
    /// Renvoie la liste de techniques de l'unité
    /// </summary>
    /// <param name="includingAttack">Si fixé à True, intègre l'attaque de l'unité à la liste</param>
    /// <returns></returns>
    public List<BaseSpell> GetSpells(bool includingAttack = false){

        if(includingAttack){
            
            List<BaseSpell> completeAvailableSpells = new List<BaseSpell>();
            foreach(BaseSpell spell in availableSpells){
                completeAvailableSpells.Add(spell);
            }
            completeAvailableSpells.Add(GetAttack());

            return completeAvailableSpells;
        }

        else{
            return availableSpells;
        }

    }

    public void RemoveSpell(BaseSpell spell_to_remove){
        List<BaseSpell> currentSpells = GetSpells();
        for (int i = currentSpells.Count -1; i >= 0; i--)
        {
            if (currentSpells[i] == spell_to_remove)
            {
                currentSpells[i].SetOwner(null);
                currentSpells.RemoveAt(i);
            }
        }
    }

    /// <summary>
    /// Renvoie une technique aléatoire de l'unité
    /// </summary>
    /// <param name="includingAttack">Si fixé à True, intègre l'attaque de l'unité à la liste</param>
    /// <returns></returns>
    public BaseSpell GetRandomSpell(bool includingAttack){
        return GetSpells(includingAttack).OrderBy(t => UnityEngine.Random.value).First();
    }

    /// <summary>
    /// Renvoie l'attaque de l'unité
    /// </summary>
    /// <returns></returns>
    public BaseSpell GetAttack(){
        return attack;
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

    /// <summary>
    /// Renvoie les HP actuels
    /// </summary>
    /// <returns>
    /// HP actuels de l'unité
    /// </returns>
    public int GetFinalHealth(){
        return finalHealth;
    }

    /// <summary>
    /// Ajoute une quantité de HP aux HP actuels et totaux
    /// </summary>
    /// <param name="amount"></param>
    public void ModifyBothHP(int amount){
        ModifyTotalHP(amount);
        ModifyHP(amount);
    }

    /// <summary>
    /// Ajoute une quantité de HP au nombre de HP actuels
    /// </summary>
    /// <param name="amount"></param>
    private void ModifyHP(int amount){
        finalHealth += amount;
        CheckHP();
    }

    /// <summary>
    /// Ajoute une quantité de HP au nombre de HP totaux
    /// </summary>
    /// <param name="amount">
    /// Nombre de HP à ajouter
    /// </param>
    public void ModifyTotalHP(int amount){
        totalHealth += amount;
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
        totalHealth *= amount;
        CheckTotalHP();
        ModifyTotalHP(amount);
    }

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
    public void CheckHP(){
        if(AreHPBeyondMax()){
            SetHP(GetTotalHealth(), false);
        }
        if(AreHPBelowZero()){
            SetHP(0, false);
            Kill();
        }
    }

    /// <summary>
    /// Vérifie que les HP totaux de l'unité sont légaux et les fixe au besoin
    /// </summary>
    public void CheckTotalHP(){
        if(AreTotalHPBelowZero()){
            SetTotalHP(0, false);
        }
        CheckHP();
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
            armor = amount;
        }
    }

    /// <summary>
    /// Ajoute une quantité d'armure donnée à l'unité
    /// </summary>
    /// <param name="amount"></param>
    public void ModifyArmor(int amount){
        armor += amount;
        CheckArmor();
    }

    /// <summary>
    /// Vérifie si l'armure de l'unité est légale et la fixe au besoin.
    /// </summary>
    public void CheckArmor(){
        if(armor < 0){
            SetArmor(0);
        }
    }

    /// <summary>
    /// Renvoie True si l'unité possède de l'armure
    /// </summary>
    /// <returns></returns>
    public bool isArmored(){
        return armor > 0;
    }

    /// <summary>
    /// Convertit une quantité donnée d'armure de l'unité en points de vie
    /// Si l'unité ne possède pas assez d'armure, la méthode ne fait rien
    /// </summary>
    /// <param name="amount"></param>
    public void ConvertArmorIntoHP(int amount){
        if(GetArmor() >= amount){
            ModifyArmor(-amount);
            ModifyBothHP(amount);
        }
    }
    #endregion

    /// <summary>
    /// Inflige un montant de dégats à l'unité
    /// </summary>
    /// <param name="amount"></param>
    public void Damage(int amount){
        int finalDamage = amount - GetArmor();
        if(finalDamage > 0){
            if(GetArmor() > 0){
                ModifyArmor(-GetArmor());
                BattleManager.Instance.AddEvent(new DamageEvent(this, -GetArmor(), true));
            }
            ModifyHP(-finalDamage);
            BattleManager.Instance.AddEvent(new DamageEvent(this, finalDamage));
        }
        else{
            ModifyArmor(-amount);
            BattleManager.Instance.AddEvent(new DamageEvent(this, amount, true));
        }
    }

    /// <summary>
    /// Soigne l'unité d'un montant donné
    /// </summary>
    /// <param name="amount"></param>
    public void Heal(int amount){
        int finalAmount = amount;
        foreach (Modifier _modifier in modifiers[Heal])
        {
            finalAmount = Tools.Ceiling(_modifier.GetNewAmount(finalAmount));
        }
        ModifyHP(+finalAmount);
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

    /// <summary>
    /// A utiliser pour ajouter l'état dead.
    /// </summary>
    public void Kill(){
        dead = true;
        EventManager.Instance.UnitDied(this);
        UnitManager.Instance.Kill(this);
    }
        #endregion

    #endregion

}