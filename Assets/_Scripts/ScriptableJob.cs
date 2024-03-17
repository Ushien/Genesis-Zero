using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Job", menuName = "Job")]

public class Job : ScriptableObject
{
    public string job_name;

    public Sprite artwork;

    [TextArea(5,10)]
    public string fight_description;

    [TextArea(5,10)]
    public string lore_description;

    public List<ScriptableSpell> spells;
}
