using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinematicCameraRotate : MonoBehaviour
{
    [Header("Rotating settings")]
    [SerializeField] float rotateSpeed = 20f;
    [SerializeField] float angleToSwitch = 180f;
    [SerializeField] bool rotate = true;
    
    [Header("Cinematic map switching settings (optional)")]
    [Tooltip("List of maps to switch when reaching the starting position")]
    [SerializeField] Generate [] maps;


    float totalAngle = 0;
    int currentMap = 0;
    Vector3 startPosition;
    
    private void Start() {
        startPosition = transform.position;

        maps[currentMap]?.RecreateSurfaceTesting();
        foreach (Generate map in maps) map.gameObject.SetActive(false);
    }

    int DivideBy(float angle) => (int)(angle/180f);

    // Update is called once per frame
    void Update()
    {
        if (!rotate) return;

        maps[currentMap]?.gameObject.SetActive(true);
        
        float angle = rotateSpeed * Time.deltaTime;

        transform.RotateAround(Vector3.zero, Vector3.up, angle);
        
        if (DivideBy(totalAngle) < DivideBy(totalAngle + angle)) SwitchMap();
        
        totalAngle += angle;
    }

    void SwitchMap(){
        maps[currentMap]?.gameObject.SetActive(false);
        currentMap = (currentMap + 1) % maps.Length;
        maps[currentMap]?.RecreateSurfaceTesting();
        maps[currentMap]?.gameObject.SetActive(true);
    }
}
