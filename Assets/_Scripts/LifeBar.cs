using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LifeBar : MonoBehaviour
{
    public int HP;
    public int Armor;
    public int totalHP;
    public BaseUnit owner;
    public TextMeshProUGUI HPtext;
    public TextMeshProUGUI ARtext;
    public Vector3 targetLifeBarScale;
    public Vector3 targetArmorBarScale;
    
    public void Awake(){
        HPtext = transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
        ARtext = transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>();
    }

    public void Update(){
        // Mise à jour des barres
        transform.GetChild(3).localScale = Vector3.Lerp(transform.GetChild(3).localScale, targetLifeBarScale, Time.deltaTime*8);
        transform.GetChild(4).localScale = Vector3.Lerp(transform.GetChild(4).localScale, targetArmorBarScale, Time.deltaTime*8);
    }

    public void Setup(BaseUnit _owner){
        owner = _owner;
        HP = owner.GetBaseTotalHealth();
        HPtext.text = HP.ToString() + " HP";
        Armor = owner.GetArmor();
        ARtext.text = Armor.ToString() + " AR";
        totalHP = owner.GetTotalHealth();

        CheckNewScale();
        transform.GetChild(3).localScale = targetLifeBarScale;
        transform.GetChild(4).localScale = targetArmorBarScale;
        Debug.Log(HPtext.text);
    }

    public void UpdateHP(int HPChange){
        HP += HPChange;
        HPtext.text = HP.ToString() + " HP";
        CheckNewScale();
    }

    public void UpdateTotalHP(int totalHPChange){
        totalHP += totalHPChange;
    }

    public void UpdateArmor(int ArmorChange){
        Armor += ArmorChange;
        ARtext.text = Armor.ToString() + " AR";
        CheckNewScale();
    }

    public void CheckNewScale(){
        targetLifeBarScale = new Vector3((float)HP/totalHP, 1, 1);
        targetArmorBarScale = new Vector3((float)Armor/totalHP, 1, 1);
    }
}
