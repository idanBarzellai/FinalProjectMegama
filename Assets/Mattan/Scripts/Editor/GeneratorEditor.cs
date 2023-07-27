using UnityEditor;
using UnityEngine;



//public class GenerateEditor : MonoBehaviour { }

[CustomEditor(typeof(Generate))]
public class GenerateEditor2 : Editor
{
    private bool showPickPaletteButton = false;
    Vector3 spawnPoint;
    FlyingController controller;

    bool canFly = false;

    private void Initialize() { 
        canFly = MatchManager.GetState() != MatchManager.GameState.Testing;
        if (!canFly) return;

        controller = new FlyingController(FindObjectOfType<BasicsController>()); 
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Generate generator = (Generate)target;

        if (GUILayout.Button("Pick Different Palette")) {
            generator.inTestMode = true;
            generator.RecreateSurfaceTesting();
        }

        GUILayout.Space(15);

        if (!canFly) return;

        if (controller == null || controller.controller == null) Initialize();


        try { if (GUILayout.Button("Fly")) controller?.Fly(); }
        catch (MissingReferenceException e) { Initialize(); controller?.Fly(); }

        try { if (GUILayout.Button("Land")) controller?.Land(); }
        catch (MissingReferenceException e) { Initialize(); controller?.Land(); }

    }

}
