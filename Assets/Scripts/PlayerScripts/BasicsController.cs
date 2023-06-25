using Cinemachine.Utility;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityEditor;
using TMPro;


public class BasicsController : MonoBehaviourPunCallbacks
{
    private bool invertLook;
    public Transform eyes;
    public Transform viewPoint;
    public Transform shootingPoint;
    private float mouseSensitivity = 1;

    public GameObject shootPlaceholder;
    public GameObject playerHitEffect;
    public GameObject playerDeathEffect;


    protected float jumpVelocity = 40, fallMultiplyer = 10, downForce = 1.2f;
    protected bool shouldJump = false;
    protected float additionToJump = 1;
    private float lastValY = 0, newValY = 0;
    private bool isDirectionChanged = false, isWalking = false;

    private bool isAttackRight = true;

    public bool isGrounded = true;
    private float rayhit = 1.2f;

    protected Rigidbody rb;
    protected CapsuleCollider col;
    private Animator anim;

    private int xDir = -10;
    private int zDir = -10;

    private Vector3 moveDir;
    protected float activeSpeed, moveSpeed = 8f;

    private float dashForce = 25f;
    private float lastTimeDashed = 0f, dashCooldown = 1f, dashThershold = 0.005f, dashDur = 0.8f;

    private float shotForce = 6f;

    private bool inSkill = false, isStaticSkill = false, isRotationStaticSkill = false;
    protected float skillCooldown = 5f, skillLastUseTime = 0f;

    private float minimumHeight = -5f;

    private float verticalRotStore;
    private Vector2 mouseInput;

    private Camera cam;

    public int maxHelath = 100;
    private int currHealth;
    private bool receivingDPS = false;
    private bool isDead = false;


    private float lastTimeMoved = 0;
    private float triggerBoardTime = 4;

    public GameObject[] bodyParts;
    public GameObject deadBodyPartsParent;
    public GameObject[] DeadBodyParts;
    private GameObject deadHeadParent;
    private GameObject deadHead;

    private List<PowerupBaseController> currentPowerups = new List<PowerupBaseController>();
    protected virtual void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        cam = Camera.main;

        rb = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();
        anim = GetComponent<Animator>();

        activeSpeed = moveSpeed;

        // Canvas controlller
        currHealth = maxHelath;
        skillLastUseTime = Time.time;
        lastTimeMoved = Time.time;

        // UIController.instance.healthShaderProgress.SetFloat("_Progress", maxHelath / maxHelath);

        // UIController.instance.skillSliderFillColor.SetFloat("_Progress", (Time.time - skillLastUseTime) / skillCooldown);

        UpdateUIController(currHealth, Time.time - skillLastUseTime);
    }

    public bool IsPlaying() => MatchManager.instance.state == MatchManager.GameState.Playing;
    protected virtual void Update()
    {
        if (photonView.IsMine)
        {
            ResetUIControllerInGame();

            // Mouse movement
            mouseInput = MouseMovement();

            // Rotation with camera movement
            RotateWithCamMovement();

            // you can move as a head hence movemnet is out of dead segment
            //moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
            moveDir = GetMovement();

            if (IsPlaying())
            {
                if (!isDead)
                {
                    // Canvas controlller
                    UpdateUIController(currHealth, Time.time - skillLastUseTime);

                    // Moving
                    MoveTowards();
                    
                    // Appling down force
                    ApplyDownForce();
                    
                    setGrounded(Physics.Raycast(transform.position, Vector3.down, rayhit, LayerMask.GetMask("Ground")));

                    if (Input.GetKeyDown(KeyCode.Space)) TryJump();

                    if (Input.GetKeyDown(KeyCode.LeftShift)) TryDash();

                    // Skill trigger
                    if (Input.GetKeyDown(KeyCode.Q)) TryPreformSkill();
                    
                     Shoot();

                    // Check for fall death
                    IsDeadFromFallDmg();

                    TryIdle();
                    
                }
                // Rolling Head
                else
                {
                    if(deadHead && deadHeadParent)
                        MoveDeadHead();
                    
                    if (Input.GetKey(KeyCode.R)) Respawn();
                }
            }
            // Unlocking camera and mouse connection
            UnlockAndLockMouse();
        }
    }
    protected virtual void FixedUpdate()
    {
        if (amIPlayingAndNotDead())
        {
            if(shouldJump)
                Jump();
        }
    }

    protected virtual void LateUpdate()
    {
        if (photonView.IsMine)
        {
            cam.transform.position = viewPoint.position;
            cam.transform.rotation = viewPoint.rotation;
        }
    }

    // ******************************Movement******************************************
    void PrintToDebugger(string message){GameObject.Find("DEBUGGER").GetComponent<TMP_Text>().text = $"{message}{'\n'}"; Debug.Log(message);}
    void AddToDebugger(string message){GameObject.Find("DEBUGGER").GetComponent<TMP_Text>().text += $"{message}{'\n'}"; Debug.Log(message);}


     private float touchSensitivity = 0.5f;
    Vector3 GetJoystickMovement(){
#if UNITY_ANDROID
        Vector3 joystickMovement = Vector3.zero;
        Joystick joystick = FindObjectOfType<Joystick>();
        if(joystick != null) joystickMovement = Vector3.forward * joystick.Vertical + Vector3.right * joystick.Horizontal;
        return joystickMovement;
#endif

        return Vector3.zero;
    }

    bool TouchOnJoystick(){
        for (int i = 0; i < Input.touchCount; i++){
            Touch touch = Input.touches[i];

            bool touchingJoystick = FindObjectOfType<Joystick>().TouchingJoystick(touch.position);
            if (touchingJoystick){
                joystickTouchIndex = i;
                return true;
            }
        }
        return false;
    }
    void TouchNearJoystick(){
        float minVal = float.MaxValue;
        int minIndex = -1;
        
        Rect rect = FindObjectOfType<Joystick>().GetComponent<RectTransform>().rect;
        Vector2 rectDimentions = new Vector2 (rect.width, rect.height);

        for (int i = 0; i < Input.touchCount; i++){
            Touch touch = Input.touches[i];

            float dis = Vector2.Distance(rectDimentions, touch.position);
            
            if (dis < minVal){
                minIndex = i;
            }
        }

        joystickTouchIndex = minIndex;
    }

    int joystickTouchIndex = -1;
    protected virtual Vector2 TouchMovement()
    {
        Vector2 touchInput = Vector2.zero;
        
        bool joystickMoving = GetJoystickMovement() != Vector3.zero;

        if (!joystickMoving) joystickTouchIndex = -1;
        else TouchNearJoystick();
        // else{
        //     if (Input.touchCount == 1) {joystickTouchIndex = 0; return touchInput;}
        //     else if (joystickTouchIndex == -1) {
        //         bool caughtIndex = TouchOnJoystick();
        //         if (!caughtIndex) TouchNearJoystick();
        // }
        // }
        
        string message = "";
        
        for (int i = 0; i < Input.touchCount; i++){
            Touch touch = Input.touches[i];

            bool touchingJoystick = FindObjectOfType<Joystick>().TouchingJoystick(touch.position);
            message += String.Format("touch #{0} is {1} touching joystick. position: {2}\n", i, (touchingJoystick ? " " : " not"), touch.position);
        }
        message += "joystick touch index: " + joystickTouchIndex;
        PrintToDebugger(message);
       
        

        for (int i = 0; i < Input.touchCount; i++){
            if (i == joystickTouchIndex) continue;
            
            Touch touch = Input.touches[i];


            Vector2 touchDelta = touch.deltaPosition;


            float rotationX = touchDelta.y * touchSensitivity;
            float rotationY = touchDelta.x * touchSensitivity;

            // Apply rotation based on touch input
            verticalRotStore += rotationX * 0.5f;
            verticalRotStore = Mathf.Clamp(verticalRotStore, -60f, 60f);

            if (invertLook)
                viewPoint.rotation = Quaternion.Euler(verticalRotStore, viewPoint.rotation.eulerAngles.y, viewPoint.rotation.eulerAngles.z);
            else
                viewPoint.rotation = Quaternion.Euler(-verticalRotStore, viewPoint.rotation.eulerAngles.y, viewPoint.rotation.eulerAngles.z);

            // Set touch input values similar to Input.GetAxisRaw()
            touchInput.x = rotationY;
            touchInput.y = rotationX;

            return touchInput;
        }
        
        return Vector2.zero;
    }

    protected virtual Vector2 MouseMovement()
    {
        if (Input.touchCount > 0) return TouchMovement();
        else                      joystickTouchIndex = -1;

        mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")) * mouseSensitivity;

        verticalRotStore += mouseInput.y;
        verticalRotStore = Mathf.Clamp(verticalRotStore, -60f, 60f);

        if (invertLook)
            viewPoint.rotation = Quaternion.Euler(verticalRotStore, viewPoint.rotation.eulerAngles.y, viewPoint.rotation.eulerAngles.z);
        else
            viewPoint.rotation = Quaternion.Euler(-verticalRotStore, viewPoint.rotation.eulerAngles.y, viewPoint.rotation.eulerAngles.z);

        return mouseInput;
        
    }

    protected virtual void RotateWithCamMovement()
    {
        if (!isRotationStaticSkill)
        {
            if (!isDead)
                transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + mouseInput.x, transform.rotation.eulerAngles.z);
            else
            {
                if(deadHeadParent)
                    deadHeadParent.transform.rotation = Quaternion.Euler(deadHeadParent.transform.rotation.eulerAngles.x, deadHeadParent.transform.rotation.eulerAngles.y + mouseInput.x, deadHeadParent.transform.rotation.eulerAngles.z);
            }
        }
    }

    protected virtual Vector3 GetMovement()
    {

        Vector3 pcMovement = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
        
        return pcMovement + GetJoystickMovement();
    }

    protected virtual void MoveTowards()
    {
        if (!isStaticSkill)
            transform.Translate(moveDir.normalized * activeSpeed * Time.deltaTime);


        // Animation controller
        bool isMoving = Math.Abs(moveDir.x) > 0 || Math.Abs(moveDir.z) > 0;
        int newXDir = (int)Input.GetAxisRaw("Horizontal");
        int newZDir = (int)Input.GetAxisRaw("Vertical");


        isDirectionChanged = newXDir != xDir || newZDir != zDir;
        bool startedWalking = !isWalking && (isMoving && !inSkill && isGrounded);
        if (isDirectionChanged || startedWalking)
        {
            photonView.RPC("SetAnim", RpcTarget.All, "move Direction Changed");
            lastTimeMoved = Time.time;

        }

        xDir = newXDir;
        zDir = newZDir;
        isWalking = isMoving && !inSkill && isGrounded;
        photonView.RPC("SetAnimInt", RpcTarget.All, "Walk X" , xDir);
        photonView.RPC("SetAnimInt", RpcTarget.All, "Walk Z" , zDir);
        photonView.RPC("SetAnimBool", RpcTarget.All, "is Walking", isWalking);
    }
    private int calcDirValForAnim(float dir)
    {
        float thershold = 0.1f;
        
        if (dir > thershold)
            return 1;

        else if (dir < (-thershold))
           return -1;

        return 0;
    }

    protected virtual void MoveDeadHead()
    {
        deadHeadParent.transform.Translate(moveDir.normalized * activeSpeed * 1.5f * Time.deltaTime);

        Vector3 rotateDir = new Vector3(moveDir.z, moveDir.y, moveDir.x);
        deadHead.transform.Rotate(rotateDir.normalized * (activeSpeed / 2));
    }

    public void freezeMovement(bool toFreeze)
    {
        rb.constraints = toFreeze ? RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation : RigidbodyConstraints.FreezeRotation;

    }

    // ******************************Physics Controller******************************************
    protected virtual void ApplyDownForce()
    {
        if (IsApplingDownForce())
        {
            newValY = rb.velocity.y;
            if(newValY <= 0 && Mathf.Sign(newValY) != Mathf.Sign(lastValY) && !isGrounded)
                photonView.RPC("SetAnim", RpcTarget.All, "Jump - Zero G");

            lastValY = newValY;

            // Falling down
            if (rb.velocity.y <= 0)
                rb.velocity += Vector3.up * Physics.gravity.y * fallMultiplyer * downForce * Time.deltaTime;

            // Going up
            else
                rb.velocity += Vector3.up * Physics.gravity.y * fallMultiplyer * Time.deltaTime;
        }
    }

    protected virtual bool IsApplingDownForce()
    {
        return !isGrounded;
    }
    public void TryJump()
    {
        if (IsAbleToJump())
        {
            shouldJump = true;
        }

    }
    public void Jump()
    {
        photonView.RPC("SetAnim", RpcTarget.All, "Jump - leap");
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        rb.AddForce(Vector3.up * jumpVelocity * additionToJump, ForceMode.Impulse);
        setGrounded(false);
        additionToJump = 1;
        shouldJump = false;

    }
    protected virtual bool IsAbleToJump()
    {
        return !inSkill && isGrounded ;
    }

    protected virtual void IsDeadFromFallDmg()
    {
        if (transform.position.y < minimumHeight)
        {
            Die("Height", 200);
        }
    }

    

    public void TryDash(){
        bool enoughTimePassed = Time.time - lastTimeDashed > dashCooldown;
        if (inSkill || !enoughTimePassed) return;

        Vector3 dir = Vector3.zero;
        if (Mathf.Abs(moveDir.z) < dashThershold && Mathf.Abs(moveDir.x) < dashThershold)
                dir = transform.forward;
        else    dir = (moveDir.z * transform.forward) + (moveDir.x * transform.right);

        dir.y += 0.5f;

        StartCoroutine(DashCo(dir));
        lastTimeDashed = Time.time;

    }

    private IEnumerator DashCo(Vector3 dir)
    {
        photonView.RPC("SetAnim", RpcTarget.All, "TryDash");
        //if (dir.x > 0)
        //    photonView.RPC("SetAnimInt", RpcTarget.All, "TryDash R");
        //else if (dir.x < 0)
        //    photonView.RPC("SetAnimInt", RpcTarget.All, "TryDash L");
        //if (dir.z > 0)
        //   photonView.RPC("SetAnim", RpcTarget.All, "TryDash F");
        //else if (dir.z < 0)
        //   photonView.RPC("SetAnim", RpcTarget.All, "TryDash B");

        rb.velocity = rb.velocity + dir.normalized * dashForce;
        yield return new WaitForSeconds(dashDur);
        rb.velocity = Vector3.zero;
    }

    [PunRPC]
    public void DealDamage(int dmg, Vector3 dir, int actor ,string damager)
    {
        if(!isDead)
            TakeDamage(dmg, dir,actor, damager);
    }
    string lastDamager;
    bool IsHeight(string damager) => damager.IndexOf("height", StringComparison.OrdinalIgnoreCase) >= 0;
    bool IsTile(string damager) => damager.IndexOf("tile", StringComparison.OrdinalIgnoreCase) >= 0;
    bool IsPlayer(string damager) => !IsTile(damager) && !IsHeight(damager);
    public void TakeDamage(int dmg, Vector3 dir,int actor, string damager)
    {
        if (lastDamager == null) lastDamager = "";
        if(photonView.IsMine)
        {
            SoundManager.instacne.Play("Hit");
            currHealth -= dmg;
            
            if(currHealth <= 0)
            {
                string killer = IsPlayer(lastDamager) && !IsPlayer(damager) ? lastDamager : damager;
                Die(killer, actor);
            }
            else
            {
                lastDamager =  IsPlayer(damager) ? damager : lastDamager;
                rb.AddForce(dir, ForceMode.Impulse);

            }
        }
    }
    public void Die(string damager, int actor)
    {
        isDead = true;
        SoundManager.instacne.Play("Death");
        col.enabled = false;
        rb.velocity = Vector3.zero;
        Destroy(rb);
        GetComponentInChildren<CharcterBody>().gameObject.SetActive(false);
        PlayerSpawner.instance.Die(damager); // debug purposes change false to regular
        PhotonNetwork.Instantiate(playerDeathEffect.name, transform.position, Quaternion.identity);
        MatchManager.instance.UpdateStatSend(actor, 0, 1);
    }

    [PunRPC]
    public void ScatterBodyParts()
    {
        deadBodyPartsParent.SetActive(true);
        foreach (GameObject obj in bodyParts)
            obj.SetActive(false);

        foreach (GameObject obj in DeadBodyParts)
        {
            obj.GetComponent<MeshCollider>().enabled = true;
            Rigidbody objRb = obj.AddComponent<Rigidbody>();

            if (obj.CompareTag("DeadHead"))
            {
                deadHead = obj;
                deadHeadParent = deadHead.transform.parent.gameObject;
                deadHeadParent.transform.position = deadHead.transform.position;
                viewPoint.parent = deadHeadParent.transform;
            }
            else
            {
                Vector3 dir = new Vector3(UnityEngine.Random.Range(-1f, 1f), 0, UnityEngine.Random.Range(-1f, 1f));
                objRb.AddForce(dir * 10, ForceMode.Impulse);
            }
        }

        isDead = true;
    }

    public virtual void Respawn()
    {
        PlayerSpawner.instance.ReSpawn();
    }

    private void TryIdle()
    {
        if(lastTimeMoved - Time.time > triggerBoardTime)
        {
            photonView.RPC("SetAnim", RpcTarget.All, "Bored");
            lastTimeMoved = Time.time;
        }
    }
    // ******************************Skills******************************************
    bool canShoot = true;
    float shotCooldown = 0.1f;
    IEnumerator ShootCooldown(){
        canShoot = false;
        yield return new WaitForSecondsRealtime(shotCooldown);
        canShoot = true;
    }
    protected virtual void Shoot()
    {
        if (!canShoot) return;

        bool isShooting = Input.GetMouseButton(0);

#if UNITY_ANDROID
        isShooting = TouchMovement() != Vector2.zero;
#endif

        if (isShooting)
        {
            StartCoroutine(ShootCooldown());
            SoundManager.instacne.Play("Shot");

            photonView.RPC("SetAnim", RpcTarget.All, "attack");

            photonView.RPC("SetAnimBool", RpcTarget.All, "is Right Attack", isAttackRight);
            isAttackRight = !isAttackRight;

            GameObject shot = PhotonNetwork.Instantiate(shootPlaceholder.name, shootingPoint.position, Quaternion.identity);
            shot.GetComponent<ShotController>().SetPlayer(this);
            shot.GetComponent<ShotController>().SetName(photonView.Owner.NickName);
            
            shot.GetComponent<Rigidbody>().AddForce(eyes.forward * shotForce, ForceMode.Impulse);
        }
    }


    public bool IsInSkill() => inSkill;

    public virtual void TryPreformSkill()
    {
        if (Time.time - skillLastUseTime > skillCooldown && !inSkill)
        {
            skillLastUseTime = Time.time;

            SkillTrigger();
        }
    }
    virtual protected void SkillTrigger()
    {

        SetInSkill(true);
    }

    public void ApplyPowerup(PowerupsManager.PowerUpsPowers power)
    {
        int addition = PowerupsManager.instance.AdditionList[power];
        switch (power)
        {
            case PowerupsManager.PowerUpsPowers.Armor:
                // Add armor
                break;
            case PowerupsManager.PowerUpsPowers.HigherJump:
                jumpVelocity += addition;
                break;
            case PowerupsManager.PowerUpsPowers.ExtraDmg:
                // Add dmg
                break;
            case PowerupsManager.PowerUpsPowers.ExtraLife:
                currHealth += addition;
                break;
            case PowerupsManager.PowerUpsPowers.Speed:
                break;
            case PowerupsManager.PowerUpsPowers.ShortCooldown:
                skillCooldown -= addition;
                break;
        }
        Debug.Log(power.ToString() + " Power was added");
    }

    // *****************************UI and Match*******************************************
    private void ResetUIControllerInGame()
    {
        if (MatchManager.instance.state == MatchManager.GameState.Waiting)
        {
            currHealth = maxHelath;
            skillLastUseTime = Time.time;
        }
    }
    public void UpdateUIController(float health, float skillTime)
    {
        // UIController.instance.healthShaderProgress.SetFloat("_Progress", health / maxHelath);
        // UIController.instance.skillSliderFillColor.SetFloat("_Progress", skillTime / skillCooldown);

        UIController.instance.healthSlider.value = health / maxHelath;
        UIController.instance.skillSlider.value = skillTime / skillCooldown;
    } 

    private void UnlockAndLockMouse()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Cursor.lockState = CursorLockMode.None;

        else if (Cursor.lockState == CursorLockMode.None)
            if (Input.GetMouseButtonDown(0) && !UIController.instance.optionsScreen.activeInHierarchy)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
    }
    protected void SetSkillBarColor(Color color)
    {
        // if (photonView.IsMine)
        //     UIController.instance.skillSliderFillColor.SetColor("_WaterColor", color); ;

    }
    

    

    protected bool amIPlayingAndNotDead()
    {
        return photonView.IsMine && MatchManager.instance.state == MatchManager.GameState.Playing && !isDead;
    }

    

    // Getters
    protected bool GetInSkill()
    {
        return inSkill;
    }

    protected bool GetIsStaticSkill()
    {
        return isStaticSkill;
    }
    protected float GetActiveSpeed()
    {
        return activeSpeed;
    }

    public bool GetReceivingDPS()
    {
        return receivingDPS;
    }

    public bool IsDead()
    {
        return isDead;
    }

    // Setters
    protected void setGrounded(bool grounded)
    {
        isGrounded = grounded;
    }

    protected void SetInSkill(bool isInSkill)
    {
        inSkill = isInSkill;
    }

    protected void SetIsStaticSkill(bool isStatic)
    {
        isStaticSkill = isStatic;
    }

    protected void SetIsRotationStaticSkill(bool isStatic)
    {
        isRotationStaticSkill = isStatic;
    }

    [PunRPC]
    public void SetAnim(string animName)
    {
        if(anim)
            anim.SetTrigger(animName);

    }
    [PunRPC]
    public void SetAnimFloat(string animName, float amount)
    {
        if (anim)
            anim.SetFloat(animName, amount);

    }
    [PunRPC]
    public void SetAnimBool(string animName, bool _bool)
    {
        if (anim)
            anim.SetBool(animName, _bool);

    }
    [PunRPC]
    public void SetAnimInt(string animName, int amount)
    {
        if (anim)
            anim.SetInteger(animName, amount);

    }
    protected void SetSpeed(float speed)
    {
        activeSpeed = speed;
    }

    protected void SetFallMultiplyer(float force)
    {
        fallMultiplyer = force;
    }

    public void SetReceivingDPS(bool _receivingDPS)
    {
        receivingDPS= _receivingDPS;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Ground"))
        {
            if(!isGrounded) photonView.RPC("SetAnim", RpcTarget.All, "Jump - land");
            setGrounded(true);

        }
    }

}
