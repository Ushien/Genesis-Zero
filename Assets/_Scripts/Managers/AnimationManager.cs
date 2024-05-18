using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    public static AnimationManager Instance;

    void Awake(){
        Instance = this;
    }

    /*
    public IEnumerator Jump(GameObject item)
    {
        for (float distance = 0.0f; distance <= 0.4f; distance += 0.02f)
        {
            item.transform.Translate(new Vector3(0, 0.02f, 0));
            yield return null;
        }
        for (float distance = 0.4f; distance >= 0.0f; distance -= 0.02f)
        {
            item.transform.Translate(new Vector3(0, -0.02f, 0));
            yield return null;
        }
    }*/

    public async Task Jump(GameObject item)
    {
        for (float distance = 0.0f; distance <= 0.4f; distance += 0.02f)
        {
            item.transform.Translate(new Vector3(0, 0.02f, 0));
            await Task.Yield();
        }
        for (float distance = 0.4f; distance >= 0.0f; distance -= 0.02f)
        {
            item.transform.Translate(new Vector3(0, -0.02f, 0));
            await Task.Yield();
        }
        await Task.Delay(1000);
    }

    /*
    public async Task Animate(List<BattleEvent> battleEvents){
        var tasks = new List<Task>();
        for (var i = 0; i < battleEvents.Count; i++)
        {
           tasks.Add(Animate(battleEvents[i]));
        }

        await Task.WhenAll(tasks);
    }*/

    public async Task Animate(List<BattleEvent> battleEvents){

        for (var i = 0; i < battleEvents.Count; i++)
        {
           await Animate(battleEvents[i]);
           await Task.Delay(500);
        }
    }

    private async Task Animate(CastEvent castEvent){
        for (float distance = 0.0f; distance <= 0.4f; distance += 0.02f)
        {
            castEvent.GetSourceUnit().gameObject.transform.Translate(new Vector3(0, 0.02f, 0));
            await Task.Yield();
        }
        for (float distance = 0.4f; distance >= 0.0f; distance -= 0.02f)
        {
            castEvent.GetSourceUnit().gameObject.transform.Translate(new Vector3(0, -0.02f, 0));
            await Task.Yield();
        }
    }

    private async Task Animate(BattleEvent battleEvent){
        if (battleEvent is CastEvent){
            await Animate((CastEvent)battleEvent);
        }
    }
}
