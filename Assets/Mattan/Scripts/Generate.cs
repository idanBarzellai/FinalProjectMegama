 using System.Linq;
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
    
    string badBehaviourPath = "Tiles/behaviour - dmg";
    string windingBehaviourPath = "Tiles/behaviour - winding";
    public bool diffHeights;
    [SerializeField] int mapRadius = 3;
    [SerializeField] float pBadTile = 0.2f;

    public MapPalletteScriptableObject [] pallettes;
    MapPalletteScriptableObject chosenPallette;
    [SerializeField] float scaleFactor = 1;
    [SerializeField] int seedsCount = 2;
    public bool interpolateHeights; 
    public int rangeY; 
    public int distBetweenDots; 
    public int subDotsCount;
    public bool inTestMode = false;
    public bool spawnBehaviourInTesting = false;
    

    void PickRandomPallette(){chosenPallette = pallettes[Random.Range(0, pallettes.Length)];}

    public void RecreateSurfaceTesting(){
        if (!inTestMode) return;
        
        foreach (Transform tile in transform)
        {
            Destroy(tile.gameObject);
        }

        PickRandomPallette();


        var xStep = Mathf.Sqrt(0.75f) * 2f; // = 1.732 ~ 1.75
        var yStep = (xStep / 2f) * Mathf.Sqrt(3f); // = 1.5

        float sizeX = mapRadius * 2 * xStep + 2;
        float sizeY = mapRadius * 2 * yStep + 2;

        float originX = -(sizeX + 2) / 2;
        float originY = -(sizeY + 2) / 2;
        var grid = GridGenerator.GenerateDoubleGridOver(sizeX, sizeY, originX, originY, rangeY, distBetweenDots, subDotsCount);
        var linearInterpolator = new LinearInterpolator(grid);
        
        List<Point3D> seeds = new List<Point3D>();
        for (var i = 0; i < seedsCount; i++)
        {
            var seedX = (GridGenerator.NextDouble() * sizeX) - (sizeX / 2);
            var seedY = (GridGenerator.NextDouble() * sizeY) - (sizeY / 2);
            var type = GridGenerator.Next(chosenPallette.tiles.Length);
            var seed = new Point3D(seedX, seedY, 0, type);
            seeds.Add(seed);
        }
        for (int xIndex = -mapRadius + 1; xIndex < mapRadius; xIndex++)
        {
            for (int yIndex = -mapRadius + 1; yIndex < mapRadius; yIndex++)
            {
                if (xIndex + yIndex < mapRadius && xIndex + yIndex > -mapRadius)
                {

                    float x = xStep * xIndex + (xStep/2f) * yIndex;

                    float y = yStep * yIndex;
                    
                    var closest = new Point3D((double)x, (double)y).FindClosestPointInList(seeds);

                    GameObject tileToSpawn = chosenPallette.tiles[closest.Type];
                    float z;
                    switch (tileToSpawn.tag)
                    {
                        case "STONE":
                            z = (float)linearInterpolator.CalculateAt(x,y,true);
                            break;
                        default:
                            z = (float)linearInterpolator.CalculateAt(x,y,false);
                            break;
                    }

                    Vector3 pos = new Vector3(x * scaleFactor, z * scaleFactor,  y * scaleFactor);

                    
                    string prefabPath = $"Tiles/{tileToSpawn.name}";

                    GameObject newTile = Instantiate(Resources.Load(prefabPath), 
                                            pos, 
                                            Quaternion.Euler(0, 0, 0)) as GameObject;
                    
                    

                    newTile.transform.localScale = newTile.transform.localScale * scaleFactor * 1.1f;
                    newTile.transform.Rotate(0, 60 * Random.Range(0, 6), 0);
                    newTile.transform.parent = this.transform;

                    if (!spawnBehaviourInTesting) continue;

                    bool shouldBeGoodTile = Random.Range(0f,1f) > pBadTile || ((Mathf.Abs(xIndex) < 2) && (Mathf.Abs(yIndex) < 2));
                    TileBehaviour tileBehaviour = shouldBeGoodTile ? TileBehaviour.NONE 
                                                                : Random.Range(0f,1f) < 0.5f 
                                                                    ? TileBehaviour.BAD 
                                                                    : TileBehaviour.WINDING;
                    switch (tileBehaviour)
                    {
                        case TileBehaviour.BAD:
                            newTile.GetComponent<TileDecorBehaviour>().SpawnBehaviourTesting(badBehaviourPath);
                            break;
                        case TileBehaviour.WINDING:
                            newTile.GetComponent<TileDecorBehaviour>().SpawnBehaviourTesting(windingBehaviourPath);
                            break;
                        default:
                            newTile.GetComponent<TileDecorBehaviour>().DecorTesting();
                            break;
                    }

                }

            }
        }
    }

    public void SpawnSurface(){
        if (!inTestMode && !PhotonNetwork.IsMasterClient) return;


        PickRandomPallette();


        var xStep = Mathf.Sqrt(0.75f) * 2f; // = 1.732 ~ 1.75
        var yStep = (xStep / 2f) * Mathf.Sqrt(3f); // = 1.5

        float sizeX = mapRadius * 2 * xStep + 2;
        float sizeY = mapRadius * 2 * yStep + 2;

        float originX = -(sizeX + 2) / 2;
        float originY = -(sizeY + 2) / 2;
        var grid = GridGenerator.GenerateDoubleGridOver(sizeX, sizeY, originX, originY, rangeY, distBetweenDots, subDotsCount);
        var linearInterpolator = new LinearInterpolator(grid);
        
        List<Point3D> seeds = new List<Point3D>();
        for (var i = 0; i < seedsCount; i++)
        {
            var seedX = (GridGenerator.NextDouble() * sizeX) - (sizeX / 2);
            var seedY = (GridGenerator.NextDouble() * sizeY) - (sizeY / 2);
            var type = GridGenerator.Next(chosenPallette.tiles.Length);
            var seed = new Point3D(seedX, seedY, 0, type);
            seeds.Add(seed);
        }
        for (int xIndex = -mapRadius + 1; xIndex < mapRadius; xIndex++)
        {
            for (int yIndex = -mapRadius + 1; yIndex < mapRadius; yIndex++)
            {
                if (xIndex + yIndex < mapRadius && xIndex + yIndex > -mapRadius)
                {

                    float x = xStep * xIndex + (xStep/2f) * yIndex;

                    float y = yStep * yIndex;
                    
                    var closest = new Point3D((double)x, (double)y).FindClosestPointInList(seeds);

                    GameObject tileToSpawn = chosenPallette.tiles[closest.Type];
                    float z;
                    switch (tileToSpawn.tag)
                    {
                        case "STONE":
                            z = (float)linearInterpolator.CalculateAt(x,y,true);
                            break;
                        default:
                            z = (float)linearInterpolator.CalculateAt(x,y,false);
                            break;
                    }

                    Vector3 pos = new Vector3(x * scaleFactor, z * scaleFactor,  y * scaleFactor);

                    
                    string prefabPath = $"Tiles/{tileToSpawn.name}";

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
                            newTile.GetComponent<TileDecorBehaviour>().SpawnBehaviour(badBehaviourPath);
                            break;
                        case TileBehaviour.WINDING:
                            newTile.GetComponent<TileDecorBehaviour>().SpawnBehaviour(windingBehaviourPath);
                            break;
                        default:
                            newTile.GetComponent<TileDecorBehaviour>().Decor();
                            break;
                    }
                }

            }
        }
    }

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
        bool isWaiting = MatchManager.GetState() == MatchManager.GameState.Waiting || inTestMode;
        if (transform.childCount == 0 && isWaiting) SpawnSurface();
    }
}
