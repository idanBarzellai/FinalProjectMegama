using UnityEngine;

[CreateAssetMenu(fileName = "passDataScriptable", menuName = "FinalProjectMegama/new PassData scriptable object", order = 0)]
public class PassDataScriptableObject : ScriptableObject {
    public float length = 0;    
    public float min = 30;    
    public float max = 300;    
}