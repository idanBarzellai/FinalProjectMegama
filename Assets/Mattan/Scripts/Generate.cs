 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Generate : MonoBehaviourPunCallbacks
{
      private void Awake()
    {
    }
    
    public bool diffHeights;
    [SerializeField] int mapRadius = 3;
    [SerializeField] float pBadTile = 0.2f;

    public MapPalletteScriptableObject [] pallettes;
    MapPalletteScriptableObject chosenPallette;
    [SerializeField] float scaleFactor = 1;
    
    float heightDeltas = 3.5f;
 
    
    void PickRandomPallette(){chosenPallette = pallettes[Random.Range(0, pallettes.Length)];}
    public void SpawnSurface(bool diffHeights){
        if (!PhotonNetwork.IsMasterClient) return;

        PickRandomPallette();

        int i=0, j=0;
        float x, y;
        for (i = -mapRadius + 1; i < mapRadius; i++)
        {
            for (j = -mapRadius + 1; j < mapRadius; j++)
            {
                if (i + j < mapRadius && i + j > -mapRadius)
                {
                    x = 1.75f * i + (1.75f / 2.0f) * j;
                    y = 1.5f * j;

                    float z = !diffHeights ? 0 : Mathf.Sin((i + j) * 2.2f) / heightDeltas;
                    bool spawnGoodTile = Random.Range(0f,1f) > pBadTile || ((Mathf.Abs(i) < 2) && (Mathf.Abs(j) < 2));
                    GameObject goodTile =  chosenPallette.goodTile;
                    string resourcesPath = $"Tiles/{chosenPallette.name}/";
                    GameObject newTile = PhotonNetwork.Instantiate(
                                            spawnGoodTile ? resourcesPath + goodTile.name : (Random.Range(0f,1f) < 0.5f ? resourcesPath + chosenPallette.dmgTile.name : resourcesPath + chosenPallette.windingTile.name), 
                                            new Vector3(x * scaleFactor, z * scaleFactor,  y * scaleFactor), 
                                            Quaternion.Euler(0, 0, 0));
                    newTile.transform.localScale = newTile.transform.localScale * scaleFactor * 1.1f;
                    newTile.transform.Rotate(0, 60 * Random.Range(0, 6), 0);
                    newTile.transform.parent = this.transform;
                }

            }
        }
    }
    public void RecreateSurface(){

        foreach (Transform tile in transform)
        {
            Destroy(tile.gameObject);
        }

        SpawnSurface(diffHeights);
    }

    void Start()
    {
        SpawnSurface(diffHeights);
    }


    public float GetScaleFactor()
    {
        return scaleFactor;
    }

    private void Update() {
        bool isWaiting = MatchManager.GetState() == MatchManager.GameState.Waiting;
        if (transform.childCount == 0 && isWaiting) SpawnSurface(diffHeights);
    }
}
