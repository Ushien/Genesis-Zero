using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraEffects : MonoBehaviour
{
    public static CameraEffects Instance;
    public float shakeIntensity = 1f;
    public float shakeTime = 0f;
    public float shakeDuration = 0f;
    public Vector3 originalPosition;

    void Awake(){
        Instance = this;
    }

    void Start(){
        originalPosition = transform.position;
    }

    void Update(){
            if (shakeTime > 0)
            {
                float currentIntensity = Mathf.Lerp(shakeIntensity, 0, 1 - (shakeTime / shakeDuration));
                Vector2 shakeOffset = Random.insideUnitCircle * currentIntensity;
                transform.localPosition = originalPosition + (Vector3)shakeOffset;
                
                shakeTime -= Time.deltaTime;
            }
            else
            {
                // Reset position when shaking is done
                transform.localPosition = originalPosition;
            }
    }
    
    public void TriggerShake(float intensity, float duration){
        shakeIntensity = intensity;
        shakeTime = duration;
        shakeDuration = duration;
    }
}
