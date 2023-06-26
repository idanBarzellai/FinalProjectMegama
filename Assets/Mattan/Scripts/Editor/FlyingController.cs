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
