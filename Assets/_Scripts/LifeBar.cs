using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    private Vector2 currentTiling;
    private Vector2 targetTiling;
    private Vector2 lerpedTiling;
    private Material lifeBarMaterial;
    private Material armorBarMaterial;
    
    public void Awake(){
        HPtext = transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
        ARtext = transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>();

    }

    public void Update(){
        // Mise à jour des barres
        transform.Find("LifeBar").localScale = Vector3.Lerp(transform.Find("LifeBar").localScale, targetLifeBarScale, Time.deltaTime*8);
        
        // Get the material and current tiling value
        lifeBarMaterial = transform.Find("LifeBar").GetComponent<Image>().material;
        currentTiling = lifeBarMaterial.GetVector("_Tiling");

        // Lerp the tiling value
        targetTiling = new Vector2(targetLifeBarScale.x, 1f);
        lerpedTiling = Vector2.Lerp(currentTiling, targetTiling, Time.deltaTime * 8);

        // Apply the interpolated tiling
        lifeBarMaterial.SetVector("_Tiling", lerpedTiling);


        transform.Find("ArmorBar").localScale = Vector3.Lerp(transform.Find("ArmorBar").localScale, targetArmorBarScale, Time.deltaTime*8);

        // Get the material and current tiling value
        armorBarMaterial = transform.Find("ArmorBar").GetComponent<Image>().material;
        currentTiling = armorBarMaterial.GetVector("_Tiling");

        // Lerp the tiling value
        targetTiling = new Vector2(targetArmorBarScale.x, 1f);
        lerpedTiling = Vector2.Lerp(currentTiling, targetTiling, Time.deltaTime * 8);

        // Apply the interpolated tiling
        armorBarMaterial.SetVector("_Tiling", lerpedTiling);
    }

    public void Setup(BaseUnit _owner){
        owner = _owner;
        HP = owner.GetBaseTotalHealth();
        HPtext.text = HP.ToString() + " HP";
        Armor = owner.GetArmor();
        ARtext.text = Armor.ToString() + " AR";
        totalHP = owner.GetTotalHealth();

        CheckNewScale();
        transform.Find("LifeBar").localScale = targetLifeBarScale;
        transform.Find("ArmorBar").localScale = targetArmorBarScale;

        //Instantiate the material
        lifeBarMaterial = transform.Find("LifeBar").GetComponent<Image>().material;
        armorBarMaterial = transform.Find("ArmorBar").GetComponent<Image>().material;
        transform.Find("LifeBar").GetComponent<Image>().material = new Material(lifeBarMaterial);
        armorBarMaterial = transform.Find("ArmorBar").GetComponent<Image>().material = new Material(armorBarMaterial);

    }

    public void UpdateHP(int HPChange){
        HP += HPChange;
        HPtext.text = HP.ToString() + " HP";
        CheckNewScale();
    }

    public void SetHP(int newHP){
        HP = newHP;
        HPtext.text = HP.ToString() + " HP";
        CheckNewScale(); 
    }

    public void UpdateTotalHP(int totalHPChange){
        totalHP += totalHPChange;
        CheckNewScale();
    }

    public void SetTotalHP(int newTotalHP){
        totalHP = newTotalHP;
        CheckNewScale();
    }

    public void UpdateArmor(int ArmorChange){
        Armor += ArmorChange;
        ARtext.text = Armor.ToString() + " AR";
        CheckNewScale();
    }

    public void SetArmor(int newArmor){
        Armor = newArmor;
        ARtext.text = Armor.ToString() + " AR";
        CheckNewScale();
    }

    public void CheckNewScale(){
        if(HP != 0){
            targetLifeBarScale = new Vector3((float)HP/totalHP, 1, 1);
        }
        else{
            targetLifeBarScale = new Vector3(0f, 1, 1);
        }
        targetArmorBarScale = new Vector3((float)Armor/totalHP, 1, 1);
    }
}
