using UnityEngine;

public class OnStepTile : Tile {
    
    private void Update() {
        if (!playerTouching)
            return;
        
        effect.Apply();
    }
}