using UnityEngine;


[CreateAssetMenu(fileName = "MapPalletasyte", menuName = "MapPallettes/New Pallette", order = 1)]
public class MapPalletteScriptableObject : ScriptableObject {
    public GameObject dmgTile;
    public GameObject goodTile;
    public GameObject windingTile;

    public GameObject [] tiles;
}
