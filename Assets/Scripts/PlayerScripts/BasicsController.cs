using Cinemachine.Utility;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

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


    private float jumpHeight = 20f, fallMultiplyer = 3f;
    private float dashForce = 20f;
    public bool isGrounded = true;
    private float rayhit = 1.2f;

    protected Rigidbody rb;
    private Animator anim;

    private Vector3 dashDirection, moveDir;
    private float activeSpeed, moveSpeed = 8f;

    private float lastTimeDashed = 0f, dashCooldown = 1f;

    private float shotForce = 5f;

    private bool inSkill = false, isStaticSkill = false, isRotationStaticSkill = false;
    protected float skillCooldown = 5f, skillLastUseTime = 0f;


    private float verticalRotStore;
    private Vector2 mouseInput;

    private Camera cam;

    public int maxHelath = 100;
    private int currHealth;
    private bool receivingDPS = false;
    private bool isDead = false;

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
        anim = GetComponent<Animator>();

        activeSpeed = moveSpeed;

        currHealth = maxHelath;
        skillLastUseTime = Time.time;
        UIController.instance.healthSlider.maxValue = maxHelath;
        UIController.instance.skillSlider.maxValue = skillCooldown;


    }

    protected virtual void Update()
    {
        if (photonView.IsMine && MatchManager.instance.state == MatchManager.GameState.Playing && !UIController.instance.optionsScreen.activeInHierarchy)
        {
            // Mouse movement
            mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")) * mouseSensitivity;

            verticalRotStore += mouseInput.y;
            verticalRotStore = Mathf.Clamp(verticalRotStore, -60f, 60f);

            if (invertLook)
                viewPoint.rotation = Quaternion.Euler(verticalRotStore, viewPoint.rotation.eulerAngles.y, viewPoint.rotation.eulerAngles.z);
            else
                viewPoint.rotation = Quaternion.Euler(-verticalRotStore, viewPoint.rotation.eulerAngles.y, viewPoint.rotation.eulerAngles.z);

            // Rotation with camera movement
            if (!isRotationStaticSkill)
            {
                if (!isDead)
                    transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + mouseInput.x, transform.rotation.eulerAngles.z);
                else
                    deadHeadParent.transform.rotation = Quaternion.Euler(deadHeadParent.transform.rotation.eulerAngles.x, deadHeadParent.transform.rotation.eulerAngles.y + mouseInput.x, deadHeadParent.transform.rotation.eulerAngles.z);
            }

           
            // you can move as a head hence movemnet is out of dead segment
            moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));

            if (!isDead)
            {
                
                // Canvas controlller
                UIController.instance.healthSlider.value = currHealth;
                UIController.instance.skillSlider.value = Time.time - skillLastUseTime;

                // Reset isGrounded
                if (Physics.Raycast(transform.position, Vector3.down, rayhit, LayerMask.GetMask("Ground")))
                    setGrounded(true);
                else
                    setGrounded(false);

                // Falling
                if (!isGrounded && rb.velocity.y < 0)
                    rb.velocity += Vector3.up * Physics.gravity.y * fallMultiplyer * Time.deltaTime;

                if (!isGrounded && rb.velocity.y > 0)
                    rb.velocity += Vector3.up * Physics.gravity.y * Time.deltaTime;

                photonView.RPC("SetAnim", RpcTarget.All, (Math.Abs(moveDir.x) > 0 || Math.Abs(moveDir.z) > 0) ? "Run" : "Idle");

                if (!inSkill)
                {
                    //// Jumping
                    rb.AddForce(Physics.gravity * fallMultiplyer * rb.mass);
                    // Jumping
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        if (isGrounded && Physics.Raycast(transform.position, Vector3.down, rayhit))
                            Jump();
                    }

                    // Dashing 
                    if (Input.GetKeyDown(KeyCode.LeftShift) && Time.time - lastTimeDashed > dashCooldown)
                    {
                        dashDirection = (moveDir.z * transform.forward) + (transform.right * moveDir.x);

                        Dash(dashDirection);
                    }
                }
                else
                {
                    // Check minmumHeight
                    if (transform.position.y > 18)
                    {
                        rb.velocity = new Vector3(rb.velocity.x, Physics.gravity.y, rb.velocity.z);
                    }
                }


                if (!isStaticSkill)
                    // Moving
                    transform.Translate(moveDir.normalized * activeSpeed * Time.deltaTime);

                // Skill trigger
                if (Time.time - skillLastUseTime > skillCooldown && Input.GetKeyDown(KeyCode.Q))
                {
                    skillLastUseTime = Time.time;

                    SkillTrigger();
                }

                if (Input.GetMouseButtonDown(0))
                    Shoot();
            }
            // Rolling Head
            else
            {
                deadHeadParent.transform.Translate(moveDir.normalized * activeSpeed * 1.5f * Time.deltaTime);

                Vector3 rotateDir = new Vector3(moveDir.z, moveDir.y, moveDir.x);
                deadHead.transform.Rotate(rotateDir.normalized * (activeSpeed / 2)  );

                if(Input.GetKey(KeyCode.R))
                    PlayerSpawner.instance.ReSpawn();

            }
        }

        // Unlocking camera and mouse connection
        if (Input.GetKeyDown(KeyCode.Escape))
            Cursor.lockState = CursorLockMode.None;

        else if (Cursor.lockState == CursorLockMode.None)
            if (Input.GetMouseButtonDown(0) && !UIController.instance.optionsScreen.activeInHierarchy)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
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

    [PunRPC]
    public void DealDamage(int dmg, Vector3 dir ,string damager)
    {
        if(!isDead)
            TakeDamage(dmg, dir, damager);
    }
    public void TakeDamage(int dmg, Vector3 dir, string damager)
    {
        if(photonView.IsMine)
        {
            currHealth -= dmg;
            
            if(currHealth <= 0)
            {
                rb.velocity= Vector3.zero;
                photonView.RPC("ScatterBodyParts", RpcTarget.All);
                PlayerSpawner.instance.Die(damager); // debug purposes change false to regular
                PhotonNetwork.Instantiate(playerDeathEffect.name, transform.position, Quaternion.identity);
                MatchManager.instance.UpdateStatSend(photonView.Owner.ActorNumber, 1, 1);
            }
            else
            {
                
                rb.AddForce(dir, ForceMode.Impulse);

            }
        }
    }
    
    public void Jump(float addition = 1f)
    {
        rb.velocity += Vector3.up * jumpHeight * addition;
        setGrounded(false);
    }

    private void Dash(Vector3 dir)
    {

        photonView.RPC("SetAnim", RpcTarget.All, "Dash");
        dir.y += 0.5f;
        rb.AddForce(dir.normalized * dashForce, ForceMode.Impulse);

        lastTimeDashed = Time.time;

    }

    private void Shoot()
    {
        photonView.RPC("SetAnim", RpcTarget.All, "Attack");

        GameObject shot = PhotonNetwork.Instantiate(shootPlaceholder.name, shootingPoint.position, Quaternion.identity);
        shot.GetComponent<ShotController>().SetName(photonView.Owner.NickName);
        shot.GetComponent<Rigidbody>().AddForce((eyes.forward) * shotForce, ForceMode.Impulse);
    }

    virtual protected void SkillTrigger()
    {
        SetInSkill(true);
    }


    public void freezeMovement(bool toFreeze)
    {
        rb.constraints = toFreeze ? RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation : RigidbodyConstraints.FreezeRotation;

    }

    public void ApplyPowerup(PowerupsManager.PowerUpsPowers power, int amountToAdd = 0)
    {
        switch (power)
        {
            case PowerupsManager.PowerUpsPowers.Armor:
                break;
            case PowerupsManager.PowerUpsPowers.DoubleJump:
                jumpHeight += amountToAdd;
                break;
            case PowerupsManager.PowerUpsPowers.ExtraDmg:
                // Add dmg
                break;
            case PowerupsManager.PowerUpsPowers.ExtraLife:
                currHealth += amountToAdd;
                break;
            case PowerupsManager.PowerUpsPowers.Shield:
                break;
            case PowerupsManager.PowerUpsPowers.Speed:
                break;
        }
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
            setGrounded(true);
    }

}
