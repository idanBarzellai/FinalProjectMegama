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
 
    
    public float GetScaleFactor()
    {
        return scaleFactor;
    }
    void PickRandomPallette(){chosenPallette = pallettes[Random.Range(0, pallettes.Length)];}

    public void SpawnSurface(bool diffHeights){

        bool inTestMode = MatchManager.GetState() == MatchManager.GameState.Testing;
        if (!inTestMode && !PhotonNetwork.IsMasterClient) return;

        if (inTestMode) {NewSpawnSurface(); return;}
        

        PickRandomPallette();

        int i=0, j=0;
        float x, y;
        float minX = float.MaxValue, minY = float.MaxValue;
        float maxX = float.MinValue, maxY = float.MinValue;

        for (i = -mapRadius + 1; i < mapRadius; i++)
        {
            for (j = -mapRadius + 1; j < mapRadius; j++)
            {
                if (i + j < mapRadius && i + j > -mapRadius)
                {

                    x = 1.5f * j;
                    minX = Mathf.Min(x, minX);
                    maxX = Mathf.Max(x, maxX);

                    y = 1.75f * i + (1.75f / 2.0f) * j;
                    minY = Mathf.Min(y, minY);
                    maxY = Mathf.Max(y, maxY);

                    float z = !diffHeights ? 0 : Mathf.Sin((i + j) * 2.2f) / heightDeltas;
                    bool spawnGoodTile = Random.Range(0f,1f) > pBadTile || ((Mathf.Abs(i) < 2) && (Mathf.Abs(j) < 2));
                    GameObject goodTile =  chosenPallette.goodTile;
                    string resourcesPath = $"Tiles/{chosenPallette.name}/";
                    string prefabPath = spawnGoodTile       
                                        ? resourcesPath + goodTile.name 
                                        : (Random.Range(0f,1f) < 0.5f 
                                                ? resourcesPath + chosenPallette.dmgTile.name 
                                                : resourcesPath + chosenPallette.windingTile.name
                                        );
                    GameObject newTile = inTestMode 
                                    ?   Instantiate(Resources.Load(prefabPath), 
                                            new Vector3(x * scaleFactor, z * scaleFactor,  y * scaleFactor), 
                                            Quaternion.Euler(0, 30, 0)) as GameObject
                                    :   PhotonNetwork.Instantiate(
                                            prefabPath, 
                                            new Vector3(x * scaleFactor, z * scaleFactor,  y * scaleFactor), 
                                            Quaternion.Euler(0, 30, 0));
                    newTile.transform.localScale = newTile.transform.localScale * scaleFactor * 1.1f;
                    newTile.transform.Rotate(0, 60 * Random.Range(0, 6), 0);
                    newTile.transform.parent = this.transform;
                }

            }
        }
        
        if (!interpolateHeights) return;

        int iMinX = (int)Mathf.Floor(minX);
        int iMinY = (int)Mathf.Floor(minY);
        
        int iWeidth = (int)Mathf.Ceil(maxX) - iMinX;
        int iHeight = (int)Mathf.Ceil(maxY) - iMinY;

        Debug.Log($"min x: {iMinX}, min y: {iMinY}, weidth: {iWeidth}, height: {iHeight}");

        var grid = GridGenerator.GenerateDoubleGridOver(iWeidth, iHeight, iMinX, iMinY, rangeY, distBetweenDots,subDotsCount);
        var linearInterpolator = new LinearInterpolator(grid);
        
        foreach (Transform child in transform)
        { 
            Debug.Log("creating point");
            var childPoint = new Point3D(child.position.x, child.position.z);
            Debug.Log($"calculating point for child at {child.position}");
            var childY = (float)linearInterpolator.Calculate(childPoint, includeSubGrid);
            Debug.Log("positioning child");
            child.position = new Vector3 (child.position.x, childY, child.position.z);
            Debug.Log(string.Format("child{0}: interpolation = {1}, height = {2}"
                                    , child.GetSiblingIndex(), (float)linearInterpolator.Calculate(childPoint,includeSubGrid), child.position.y));

        }
    }
    public void NewSpawnSurface(){

        bool inTestMode = MatchManager.GetState() == MatchManager.GameState.Testing;
        if (!PhotonNetwork.IsMasterClient) return;


        PickRandomPallette();


        //          WHATSAPP
        var xStep = Mathf.Sqrt(0.75f) * 2f; // = 1.732 ~ 1.75
        var yStep = (xStep / 2f) * Mathf.Sqrt(3f); // = 1.5

        float sizeX = mapRadius * 2 * xStep + 2;
        float sizeY = mapRadius * 2 * yStep + 2;

        float originX = -(sizeX + 2) / 2;
        float originY = -(sizeY + 2) / 2;
        //          WHATSAPP
        var grid = GridGenerator.GenerateDoubleGridOver(sizeX, sizeY, originX, originY, rangeY, distBetweenDots, subDotsCount);
        var linearInterpolator = new LinearInterpolator(grid);
        //          ADDONS

        for (int xIndex = -mapRadius + 1; xIndex < mapRadius; xIndex++)
        {
            for (int yIndex = -mapRadius + 1; yIndex < mapRadius; yIndex++)
            {
                if (xIndex + yIndex < mapRadius && xIndex + yIndex > -mapRadius)
                {

                    float x = xStep * xIndex + (xStep/2f) * yIndex;

                    float y = yStep * yIndex;

                    // float z = !diffHeights ? 0 : Mathf.Sin((xIndex + yIndex) * 2.2f) / heightDeltas;
                    Debug.LogWarning("before calculate");
                    float z = (float)linearInterpolator.CalculateAt(x,y,includeSubGrid);
                    Debug.LogWarning("after calculate");

                    Vector3 pos = new Vector3(x * scaleFactor, z * scaleFactor,  y * scaleFactor);

                    
                    string prefabPath = $"Tiles/{chosenPallette.tiles[0].name}";

                    Debug.LogWarning("before instantiating");
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
                    Debug.LogWarning("instantiated");

                    // GameObject gridPoint = GameObject.FindObjectOfType<GridShower>().gridPoint;
                    // GameObject newGridPoint = Instantiate(gridPoint, pos, Quaternion.identity);
                    // Debug.LogWarning("instantiated point");
                    // newGridPoint.transform.localScale = Vector3.one;
                    // newGridPoint.transform.SetParent(this.transform, false);

                    bool shouldBeGoodTile = Random.Range(0f,1f) > pBadTile || ((Mathf.Abs(xIndex) < 2) && (Mathf.Abs(yIndex) < 2));
                    TileBehaviour tileBehaviour = shouldBeGoodTile ? TileBehaviour.NONE 
                                                                : Random.Range(0f,1f) < 0.5f 
                                                                    ? TileBehaviour.BAD 
                                                                    : TileBehaviour.WINDING;

                    Debug.LogWarning("searching for behaviour");
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
                    Debug.LogWarning("picked behaviour and finished spawning the tile");
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

        NewSpawnSurface();
    }

    void Start()
    {
        NewSpawnSurface();
    }


    private void Update() {
        bool isWaiting = MatchManager.GetState() == MatchManager.GameState.Waiting || MatchManager.GetState() == MatchManager.GameState.Testing;
        if (transform.childCount == 0 && isWaiting) NewSpawnSurface();
    }
}
