using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraEffects : MonoBehaviour
{
    public static CameraEffects Instance;
    public float shakeIntensity = 1f;
    public float shakeTime = 0f;
    public float shakeDuration = 0f;

    public float driftVelocity = 1;
    public float driftSmoothing = 0.3f;
    public bool drifting = false;

    public Vector3 originalPosition;
    public Vector3 selectorLinePosition;
    public Vector3 selectorLineTarget;
    public Vector3 driftedPosition;
    public Vector3 targetPosition;
    private Vector3 velocity = new Vector3(0f,0f,0f);

    void Awake(){
        Instance = this;
    }

    void Start(){
        originalPosition = transform.position;
        driftedPosition = originalPosition;
    }

    void Update(){
            // Effet de camera shake
            // ---------------------
            if (shakeTime > 0)
            {
                float currentIntensity = Mathf.Lerp(shakeIntensity, 0, 1 - (shakeTime / shakeDuration));
                Vector2 shakeOffset = Random.insideUnitCircle * currentIntensity;
                transform.localPosition = driftedPosition + (Vector3)shakeOffset;
                
                shakeTime -= Time.deltaTime;
            }
            else
            {
                // Reset de la position
                transform.localPosition = driftedPosition;
            }

            // Effet de camera drift
            // ---------------------
            if (drifting)
            {
                // Add slight wave motion for extra floatiness
                // targetPosition.x += Mathf.Sin(Time.time * 1.5f) * 0.1f; 
                // targetPosition.y += Mathf.Cos(Time.time * 1.5f) * 0.1f; 

                transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, driftSmoothing);
                driftedPosition = transform.position;

                if (transform.position == targetPosition){
                    drifting = false;
                }
            }


    }
    
    // Fonction pour lancer le camera shake
    // ------------------------------------
    public void TriggerShake(float intensity, float duration){
        shakeIntensity = intensity;
        shakeTime = duration;
        shakeDuration = duration;
    }

    // Fonction pour lancer le camera drift
    // ------------------------------------
    public void TriggerDrift(float intensity, float smoothing, Vector3 direction){
        drifting = true;
        driftVelocity = intensity;
        driftSmoothing = smoothing;
        targetPosition = originalPosition + direction;
        if(selectorLinePosition != null)
            selectorLineTarget = selectorLinePosition + direction;

    }

    public void ChangeSelectorLinePosition(Vector3 newPosition){
        selectorLinePosition = newPosition;
    }
}
