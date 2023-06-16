using UnityEngine;
using System;

[Serializable]
public enum ButtonType {JUMP, DASH, SKILL, RESPAWN};
public class AndroidMovementButtons : MonoBehaviour {

    private void Awake() {
#if (!UNITY_ANDROID) 
        gameObject.SetActive(false);
#endif
    }

    [SerializeField] GameObject respawnButton; 
    BasicsController controller;

    void SetController(){
        foreach (BasicsController controller_i in FindObjectsOfType<BasicsController>()){
            if (!controller_i.photonView.IsMine) continue;

            controller = controller_i;
            break;
        }
    }
    
    private void Start() {
        respawnButton.transform.parent = this.transform.parent;
        respawnButton.transform.SetSiblingIndex(6);

        respawnButton.SetActive(false);
        SetController();
    }   


    private void Update(){
        SetController();
        
        if(controller != null) respawnButton.SetActive(controller.IsDead());
    }

    void ButtonClickDash() =>    controller?.TryDash();
    void ButtonClickJump() =>    controller?.TryJump();
    void ButtonClickRespawm() => controller?.Respawn();
    void ButtonClickSkill() {
        if (controller == null) return;

        if (!controller.IsInSkill()) {controller.TryPreformSkill(); return;}

        EarthPlayer earth = FindObjectOfType<EarthPlayer>();
        WaterPlayer water = FindObjectOfType<WaterPlayer>();
        if (earth != null) earth.earthSkill();
        if (water != null) water.resetVariables();
    }






    public void ClickButton(string str){
        ButtonType type = ButtonTypeToString(str);
        SetController();

        // if not playing, return
        if (controller == null || !controller.IsPlaying()) return;

        // if dead, only respawn available
        if (controller.IsDead()) {
            if (type == ButtonType.RESPAWN) 
                ButtonClickRespawm(); 
            return;
        }

        
        switch (type)
        {
            case ButtonType.JUMP:
                ButtonClickJump();
                break;
            case ButtonType.DASH:
                ButtonClickDash();
                break;
            case ButtonType.SKILL:
                ButtonClickSkill();
                break;
        }


    }
    public ButtonType ButtonTypeToString(string str){
        switch (str)
        {
            case "jump":    return ButtonType.JUMP;
            case "dash":    return ButtonType.DASH;
            case "skill":   return ButtonType.SKILL;
            default:        return ButtonType.RESPAWN;
        }
    }
}