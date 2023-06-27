 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TileMap_Auto_Generation;

public enum TileBehaviour {BAD,WINDING,NONE}
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

    public void SpawnSurface(){
        if (!PhotonNetwork.IsMasterClient) return;

        bool inTestMode = MatchManager.GetState() == MatchManager.GameState.Testing;


        PickRandomPallette();


        var xStep = Mathf.Sqrt(0.75f) * 2f; // = 1.732 ~ 1.75
        var yStep = (xStep / 2f) * Mathf.Sqrt(3f); // = 1.5

        float sizeX = mapRadius * 2 * xStep + 2;
        float sizeY = mapRadius * 2 * yStep + 2;

        float originX = -(sizeX + 2) / 2;
        float originY = -(sizeY + 2) / 2;
        var grid = GridGenerator.GenerateDoubleGridOver(sizeX, sizeY, originX, originY, rangeY, distBetweenDots, subDotsCount);
        var linearInterpolator = new LinearInterpolator(grid);

        for (int xIndex = -mapRadius + 1; xIndex < mapRadius; xIndex++)
        {
            for (int yIndex = -mapRadius + 1; yIndex < mapRadius; yIndex++)
            {
                if (xIndex + yIndex < mapRadius && xIndex + yIndex > -mapRadius)
                {

                    float x = xStep * xIndex + (xStep/2f) * yIndex;

                    float y = yStep * yIndex;

                    float z = (float)linearInterpolator.CalculateAt(x,y,includeSubGrid);

                    Vector3 pos = new Vector3(x * scaleFactor, z * scaleFactor,  y * scaleFactor);

                    
                    string prefabPath = $"Tiles/{chosenPallette.tiles[0].name}";

                    GameObject newTile = inTestMode 
                                    ?   Instantiate(Resources.Load(prefabPath), 
                                            pos, 
                                            Quaternion.Euler(0, 0, 0)) as GameObject
                                    :   PhotonNetwork.Instantiate(
                                            prefabPath, 
                                            pos, 
                                            Quaternion.Euler(0, 0, 0));
                    
                    

                    newTile.transform.localScale = newTile.transform.localScale * scaleFactor * 1.1f;
                    newTile.transform.Rotate(0, 60 * Random.Range(0, 6), 0);
                    newTile.transform.parent = this.transform;

                    bool shouldBeGoodTile = Random.Range(0f,1f) > pBadTile || ((Mathf.Abs(xIndex) < 2) && (Mathf.Abs(yIndex) < 2));
                    TileBehaviour tileBehaviour = shouldBeGoodTile ? TileBehaviour.NONE 
                                                                : Random.Range(0f,1f) < 0.5f 
                                                                    ? TileBehaviour.BAD 
                                                                    : TileBehaviour.WINDING;

                    switch (tileBehaviour)
                    {
                        case TileBehaviour.BAD:
                            newTile.transform.Find("behaviour - dmg").gameObject.SetActive(true);
                            break;
                        case TileBehaviour.WINDING:
                            newTile.transform.Find("behaviour - winding").gameObject.SetActive(true);
                            break;
                        default:
                            newTile.GetComponent<TileDecor>().Decor();
                            break;
                    }
                }

            }
        }
    }

    public bool interpolateHeights; 
    public bool includeSubGrid; 
    public int rangeY; 
    public int distBetweenDots; 
    public int subDotsCount;
    public void RecreateSurface(){

        foreach (Transform tile in transform)
        {
            Destroy(tile.gameObject);
        }

        SpawnSurface();
    }

    void Start()
    {
        SpawnSurface();
    }


    private void Update() {
        bool isWaiting = MatchManager.GetState() == MatchManager.GameState.Waiting || MatchManager.GetState() == MatchManager.GameState.Testing;
        if (transform.childCount == 0 && isWaiting) SpawnSurface();
    }
}
