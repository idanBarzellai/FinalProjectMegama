using UnityEngine;

public class SpawnPlatform : MonoBehaviour
{
    void Update()
    {
        if(MatchManager.instance.state == MatchManager.GameState.Playing)
        {
            this.gameObject.SetActive(false);
        }
    }
}
