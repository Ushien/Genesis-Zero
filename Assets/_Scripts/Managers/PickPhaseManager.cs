using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class PickPhaseManager : MonoBehaviour
{
    [SerializeField]
    private GameObject choiceCell;
    public enum RewardType{EMPTY, PASSIVE, SPELL}
    private List<System.IO.FileInfo> spellList = new List<System.IO.FileInfo>();

    // Start is called before the first frame update
    void Start()
    {
        string worldsFolder = "./Assets/Resources/Spells";
        if (Directory.Exists(worldsFolder))
        {
            DirectoryInfo d = new DirectoryInfo(worldsFolder);
            foreach (FileInfo file in d.GetFiles("*.asset"))
            {
                spellList.Add(file);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private Reward GenerateReward(RewardType rewardType){
        if(rewardType == RewardType.SPELL){
            //
        }
        return new Reward();
    }


    private void DisplayRewards(List<Reward> rewards){
        foreach (Reward reward in rewards)
        {
            Instantiate(choiceCell);
        }

    }
}
