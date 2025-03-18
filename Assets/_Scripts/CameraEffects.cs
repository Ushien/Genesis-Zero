using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraEffects : MonoBehaviour
{
    // Singleton
    // ---------
    public static CameraEffects Instance;

    // Shake parameters
    // ----------------
    public float shakeIntensity = 1f;
    public float shakeTime = 0f;
    public float shakeDuration = 0f;

    // Drift parameters
    // ----------------
    public float driftVelocity = 1;
    public float driftSmoothing = 0.3f;
    public bool drifting = false;
    public Vector3 selectorLinePosition;
    public Vector3 selectorLineTarget;
    public Vector3 originalPosition;
    public Vector3 driftedPosition;
    public Vector3 targetPosition;
    private Vector3 velocity = new Vector3(0f,0f,0f);

    // Zoom parameters
    // ---------------
    public float minZoom = 2f;
    public float maxZoom = 10f;
    private Camera cam;
    private Coroutine zoomCoroutine;

    void Awake(){
        Instance = this;
    }

    void Start(){
        originalPosition = transform.position;
        driftedPosition = originalPosition;
        cam = GetComponent<Camera>();
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
                //Parallax.Instance.UpdateParallax(transform.position, targetPosition);
                Parallax.Instance.UpdateParallax(transform.position, targetPosition);
                driftedPosition = transform.position;

                if ((transform.position - targetPosition).sqrMagnitude < 0.001f){
                    drifting = false;
                }
            }


    }

    // Fonctions pour zoom effect
    // --------------------------
    public void TriggerZoom(Vector3 zoomTarget, float zoomAmount, float zoomSpeed, bool dezoom = false)
    {
        if (zoomCoroutine != null) StopCoroutine(zoomCoroutine);
        zoomCoroutine = StartCoroutine(SmoothZoom(zoomTarget, zoomAmount, zoomSpeed, dezoom));
    }

    private IEnumerator SmoothZoom(Vector3 zoomTarget, float zoomAmount, float zoomSpeed, bool dezoom = false)
    {
        float startZoom = cam.orthographicSize;
        float t = zoomSpeed;
        
        while (t > 0)
        {
            t -= Time.deltaTime;
            cam.orthographicSize = Mathf.Lerp(startZoom, zoomAmount, 1f-(t/zoomSpeed));
            cam.transform.position = Vector3.Lerp(cam.transform.position, zoomTarget, 1f-(t/zoomSpeed));
            yield return null;
        }

        cam.orthographicSize = zoomAmount;
        cam.transform.position = zoomTarget;
        driftedPosition = transform.position;

        if(dezoom)
            zoomCoroutine = StartCoroutine(SmoothZoom(originalPosition, startZoom, zoomSpeed, false));
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
        targetPosition = transform.position + direction;

        // Ces deux fonctions limitent la caméra à un certain espace de jeu
        // ----------------------------------------------------------------
        targetPosition.x = Mathf.Clamp(targetPosition.x, 2f, 2.4f);
        targetPosition.y = Mathf.Clamp(targetPosition.y, 0.8f, 1.2f);
        
        if(selectorLinePosition != null)
            selectorLineTarget = selectorLinePosition + direction;

    }

    public void ChangeSelectorLinePosition(Vector3 newPosition){
        selectorLinePosition = newPosition;
    }
}
