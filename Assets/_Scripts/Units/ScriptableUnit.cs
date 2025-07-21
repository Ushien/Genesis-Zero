using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contient les informations relatives à l'initialisation d'une unité.
/// </summary>
[CreateAssetMenu(fileName = "New Unit", menuName = "Unit")]
public class ScriptableUnit : ScriptableObject
{
    public int id;
    public Sprite sprite;
    public RuntimeAnimatorController animator;
    public int original_power;
    public int original_health;
    public List<ScriptablePassive> passives;
    public ScriptableSpell aAttack;
    public List<ScriptableSpell> spells;
}