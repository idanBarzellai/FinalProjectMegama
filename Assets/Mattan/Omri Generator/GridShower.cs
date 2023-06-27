using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TileMap_Auto_Generation;

public class GridShower : MonoBehaviour
{
    [SerializeField] int mapRadius = 3;
    public GameObject gridPoint;
 
    public bool includeSubGrid; 
    public int rangeY; 
    public int distBetweenDots; 
    public int subDotsCount;
    [SerializeField] float scaleFactor = 1;
    // Start is called before the first frame update
    void Start()
    {
        NewSpawnSurface();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space)) ReSpawn();   
    }
    void ReSpawn(){
        foreach (Transform child in transform) Destroy(child.gameObject);
        NewSpawnSurface();
    }
    Vector3 PointToVector3(Point3D p) => new Vector3(((float)p.X) * scaleFactor, ((float)p.Z) * scaleFactor, ((float)p.Y) * scaleFactor);
    public void NewSpawnSurface(){

        bool inTestMode = MatchManager.GetState() == MatchManager.GameState.Testing;
        if (!inTestMode && !PhotonNetwork.IsMasterClient) return;



        //          WHATSAPP
        var xStep = Mathf.Sqrt(0.75f) * 2; // = 1.732 ~ 1.75
        var yStep = (xStep / 2f) * Mathf.Sqrt(3f); // = 1.5

        float sizeX = mapRadius * 2 * xStep + 2;
        float sizeY = mapRadius * 2 * yStep + 2;

        float originX = -(sizeX + 2) / 2;
        float originY = -(sizeY + 2) / 2;
        //          WHATSAPP
        Debug.Log("trying to create a grid");
        var grid = GridGenerator.GenerateDoubleGridOver(sizeX, sizeY, originX, originY, rangeY, distBetweenDots, subDotsCount);
        Debug.Log("created grid");
        foreach (Point3D point in grid)
        {
            if (point.Type != 0) continue;

            GameObject pointGameObject = Instantiate(gridPoint, PointToVector3(point), Quaternion.identity);
            pointGameObject.transform.SetParent(transform, false);
        }
       
    }
}
