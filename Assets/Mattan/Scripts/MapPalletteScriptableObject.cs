using UnityEngine;

[CreateAssetMenu(fileName = "MapPallette", menuName = "MapPallettes/New Pallette", order = 1)]
public class MapPalletteScriptableObject : ScriptableObject {
    
    public GameObject goodTile;
    public GameObject dmgTile;
    public GameObject windingTile;
}