using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineController : MonoBehaviour
{
    public float wiggleSpeed = 1.0f; // Speed of wiggle
    public float wiggleStrength = 0.5f; // How much it wiggles
    public LineRenderer lr;
    public Transform[] points;
    public bool[] verticalPoint;
    public Vector3[] initialPointPosition;
    public Vector3[] wiggleDirections;
    public float xOffset;
    public float yOffset;

    private void Start(){
        SetUpLine(points);
        
        // Initialisation des positions initiales des points
        // -------------------------------------------------
        initialPointPosition = new Vector3[points.Length];
        wiggleDirections = new Vector3[points.Length];
        for (int i = 0; i < points.Length; i++){
            initialPointPosition[i] = points[i].position;
            
            // On donne une direction différente à chaque point
            // ------------------------------------------------
            switch (i % 4)
            {
                case 0: wiggleDirections[i] = Vector3.right; break;   // Wiggle left/right
                case 1: wiggleDirections[i] = Vector3.up; break;      // Wiggle up/down
                case 2: wiggleDirections[i] = Vector3.forward; break; // Wiggle in/out (Z axis)
                case 3: wiggleDirections[i] = new Vector3(1, 1, 0).normalized; break; // Diagonal wiggle
            }
        }
    }

    // Set up le nombre de points de la ligne et ses positions
    // -------------------------------------------------------
    public void SetUpLine(Transform[] points) {
        lr.positionCount = points.Length;
        this.points = points;
    }

    // A chaque update, on ajuste la position de la ligne sur celle des points, et on wiggle les points
    // ------------------------------------------------------------------------------------------------
    private void Update() {
        for (int i = 0; i < points.Length; i++){
            if(i > 0 && i < points.Length - 1){
                float wiggleAmount = Mathf.Sin(Time.time * wiggleSpeed + i) * wiggleStrength;
                points[i].position = initialPointPosition[i] + wiggleDirections[i] * wiggleAmount
                + new Vector3(InterfaceManager.Instance.UILineVertical.transform.position.x, InterfaceManager.Instance.UILineHorizontal.transform.position.y, 0f);
            }
            lr.SetPosition(i, points[i].position);
        }
        
        // drifting de la ligne de UI
        // --------------------------
        Vector3 spellSelectorPos = Camera.main.ScreenToWorldPoint(InterfaceManager.Instance.spellSelector.position);
        points[0].position = new Vector3 (spellSelectorPos.x + xOffset, spellSelectorPos.y +yOffset, 0f);

    }

}
