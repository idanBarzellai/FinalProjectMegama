using UnityEngine;
using System;

[Serializable]
public enum ButtonType {JUMP, DASH, SKILL, RESPAWN, PAUSE};
public class AndroidUI : MonoBehaviour {

    public ButtonType ButtonTypeToString(string str){
        switch (str)
        {
            case "jump":    return ButtonType.JUMP;
            case "dash":    return ButtonType.DASH;
            case "skill":   return ButtonType.SKILL;
            case "pause":   return ButtonType.PAUSE;
            default:        return ButtonType.RESPAWN;
        }
    }

    [SerializeField] GameObject canvasMock; 
    [SerializeField] GameObject respawnButton; 
    BasicsController controller;

    private void Awake() {
#if (!UNITY_ANDROID) 
        gameObject.SetActive(false);
#endif
    }
    
    private void Start() {
        canvasMock.SetActive(false);
        respawnButton.SetActive(false);
    }


    private void Update() {
        if (controller == null) {controller = FindObjectOfType<BasicsController>(); return;} 

        if (controller.IsDead()) respawnButton.SetActive(true);
        else                     respawnButton.SetActive(false);
    }

    void ButtonClickPause() =>   FindObjectOfType<UIController>()?.ShowHideOptions();
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
            case ButtonType.PAUSE:
                ButtonClickPause();
                break;
        }


    }
}