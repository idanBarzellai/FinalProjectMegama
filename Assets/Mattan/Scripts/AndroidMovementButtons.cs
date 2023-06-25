using UnityEngine;
using System;
using TMPro;

public class AndroidMovementButtons : MonoBehaviour {
    void PrintToDebugger(string message){GameObject.Find("DEBUGGER").GetComponent<TMP_Text>().text = $"{message}{'\n'}"; Debug.Log(message);}
    void AddToDebugger(string message){GameObject.Find("DEBUGGER").GetComponent<TMP_Text>().text += $"{message}{'\n'}"; Debug.Log(message);}

    BasicsController controller;

    bool IsDestroyed() => respawnButton == null || gameObject == null;
    void Show(){if(IsDestroyed()) return; respawnButton.gameObject.SetActive(false); gameObject.SetActive(true);}
    void Hide(){if(IsDestroyed()) return; respawnButton.gameObject.SetActive(true); gameObject.SetActive(false);}
    void SelfDestruct(){
        DestroyImmediate(respawnButton, true); 
        DestroyImmediate(this.gameObject, true);
    }

    public void SetController(BasicsController _controller){
        controller = _controller;
        PrintToDebugger($"set controller: {controller.gameObject.name}");
        Show();
    }

    
    private void Awake() {
#if (!UNITY_ANDROID) 
        PrintToDebugger("no android");
        SelfDestruct();
        return;
#endif
    }

    [SerializeField] GameObject respawnButton; 
    
    private void Start() => Show();


    private void Update() 
    {
        if (controller == null) return;

        bool gameHasEnded = MatchManager.instance.state == MatchManager.GameState.Ending;
        // if (gameHasEnded) SelfDestruct();
        if (gameHasEnded) return;
        if (!controller.IsDead()) return;
        
        Hide();
    }

    bool CanJump() => controller.isGrounded && !controller.IsInSkill();
    void ButtonClickJump() {if(CanJump()) controller?.Jump();}
    void ButtonClickDash() =>    controller?.TryDash();
    void ButtonClickRespawm() {controller?.Respawn(); Show();}
    void ButtonClickSkill() {
        if (controller == null) return;

        if (!controller.IsInSkill()) {controller.TryPreformSkill(); return;}

        EarthPlayer earth = FindObjectOfType<EarthPlayer>();
        WaterPlayer water = FindObjectOfType<WaterPlayer>();
        if (earth != null) earth.earthSkill();
        if (water != null) water.resetVariables();
    }





    public void ClickButton(string str){

        // if not playing, return
        if (controller == null || !controller.IsPlaying()) return;

        PrintToDebugger($"button click: {str}");
        AddToDebugger($"controller exist? {controller!=null}");
        AddToDebugger($"controller can jump? {CanJump()}");

        // if dead, only respawn available
        if (controller.IsDead()) {
            if (str == "respawn") 
                ButtonClickRespawm(); 
            return;
        }

        
        switch (str)
        {
            case "jump":
                ButtonClickJump();
                break;
            case "dash":
                ButtonClickDash();
                break;
            case "skill":
                ButtonClickSkill();
                break;
        }


    }
}