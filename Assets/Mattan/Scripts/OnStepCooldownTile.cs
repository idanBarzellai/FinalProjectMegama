using UnityEngine;

public class OnStepCooldownTile : Tile {
    [SerializeField] float cooldown;
    float lastApply;
    public override void Awake() {
        base.Awake();
        lastApply = Time.realtimeSinceStartup;
    }

    private void Update() {
        if (Time.realtimeSinceStartup - lastApply <= cooldown)
            return;
        if (!playerTouching)
            return;
        
        effect.Apply();
        lastApply = Time.realtimeSinceStartup;
    }
}