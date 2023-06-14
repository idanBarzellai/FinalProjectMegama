using UnityEditor;
using UnityEngine;


public class FlyingController
{
    public BasicsController controller;

    struct DoubleVector3{public Vector3 newV; public Vector3 oldV;}
    Rigidbody rb;
    DoubleVector3 pos;
    DoubleVector3 scale;

    public FlyingController(BasicsController _controller){
        controller = _controller;

        rb = controller.GetComponent<Rigidbody>();

        pos.oldV = controller.transform.localPosition;
        pos.newV = new Vector3(75,83,-184);

        scale.oldV = controller.transform.localScale;
        scale.newV = new Vector3(0,0,0);
    }
    public void Fly(){
        rb.useGravity = false;

        var trans = controller.transform;
        trans.localPosition = pos.newV;
        trans.localScale = scale.newV;
   }

    public void Land(){
        rb.useGravity = true;

        var trans = controller.transform;
        trans.localPosition = pos.oldV;
        trans.localScale = scale.oldV;
    }
}

//public class GenerateEditor : MonoBehaviour { }

[CustomEditor(typeof(Generate))]
public class GenerateEditor2 : Editor
{
    private bool showPickPaletteButton = false;
    Vector3 spawnPoint;
    FlyingController controller;

    private void Initialize() { controller = new FlyingController(FindObjectOfType<BasicsController>()); }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Generate generator = (Generate)target;

        if (controller == null || controller.controller == null) Initialize();

        if (GUILayout.Button("Pick Different Palette")) generator.RecreateSurface();

        GUILayout.Space(15);

        try { if (GUILayout.Button("Fly")) controller?.Fly(); }
        catch (MissingReferenceException e) { Initialize(); controller?.Fly(); }

        try { if (GUILayout.Button("Land")) controller?.Land(); }
        catch (MissingReferenceException e) { Initialize(); controller?.Land(); }

    }

}
