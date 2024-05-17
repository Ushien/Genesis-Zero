using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    public static AnimationManager Instance;

    void Awake(){
        Instance = this;
    }

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
    }
}
