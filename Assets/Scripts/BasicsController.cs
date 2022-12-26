using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BasicsController : MonoBehaviourPunCallbacks
{
    private bool invertLook;
    public Transform eyes;
    public Transform viewPoint;
    public Transform shootingPoint;
    private float mouseSensitivity = 2;

    public GameObject shootPlaceholder;
    public GameObject playerHitEffect;

    private float jumpHeight = 20f, fallMultiplyer = 3f;
    private float dashForce = 20f;
    public bool isGrounded = true;
    private float rayhit = 1.2f;

    protected Rigidbody rb;
    private Animator anim;

    private Vector3 dashDirection;
    private Vector3 moveDir;
    private float activeSpeed;
    private float moveSpeed = 8f, runSpeed = 12f;

    private float lastTimeDashed = 0f, dashCooldown = 1f;

    private float shotForce = 5f;

    private bool inSkill = false;
    private bool isStaticSkill = false;
    private bool isRotationStaticSkill = false;

    private float verticalRotStore;
    private Vector2 mouseInput;

    private Camera cam;

    public int maxHelath = 100;
    private int currHealth;


    protected virtual void Start()
    {

        Cursor.lockState = CursorLockMode.Locked;

        cam = Camera.main;

        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();

        activeSpeed = moveSpeed;

        currHealth = maxHelath;
        UIController.instance.healthSlider.maxValue = maxHelath;

    }

    protected virtual void Update()
    {
        if (photonView.IsMine)
        {
            UIController.instance.healthSlider.value = currHealth;
            // Reset isGrounded
            if (Physics.Raycast(transform.position, Vector3.down, rayhit, LayerMask.GetMask("Ground")))
                setGrounded(true);
            else
                setGrounded(false);



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
                transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + mouseInput.x, transform.rotation.eulerAngles.z);

            moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));

            // Falling
            if (!isGrounded && rb.velocity.y < 0)
                rb.velocity += Vector3.up * Physics.gravity.y * fallMultiplyer * Time.deltaTime;

            if (!isGrounded && rb.velocity.y > 0)
                rb.velocity += Vector3.up * Physics.gravity.y * Time.deltaTime;

            anim.SetTrigger((Math.Abs(moveDir.x) > 0 || Math.Abs(moveDir.z) > 0) ? "Run" : "Idle");

            if (!inSkill)
            {
                //// Jumping
                rb.AddForce(Physics.gravity * fallMultiplyer * rb.mass);
                // Jumping
                if(Input.GetKeyDown(KeyCode.Space))
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

            if (Input.GetMouseButtonDown(0))
                Shoot();

            // Unlocking camera and mouse connection
            if (Input.GetKeyDown(KeyCode.Escape))
                Cursor.lockState = CursorLockMode.None;

            else if (Cursor.lockState == CursorLockMode.None)
                if (Input.GetMouseButtonDown(0))//&& !UIController.instance.optionsScreen.activeInHierarchy)
                    Cursor.lockState = CursorLockMode.Locked;

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
    public void DealDamage(int dmg, string damager = "default")
    {
        TakeDamage(dmg, damager);
    }
    public void TakeDamage(int dmg, string damager)
    {
        if(photonView.IsMine)
        {
            currHealth -= dmg;
            
            if(currHealth <= 0)
            {
                PlayerSpawner.instance.Die(damager);

            }


        }
 
    }

    [PunRPC]
    public void PushedForce(Vector3 dir)
    {
        if (photonView.IsMine)
        {
            rb.AddForce(dir, ForceMode.Impulse);
        }
    }

    public void Jump(float addition = 1f)
    {
        rb.velocity += Vector3.up * jumpHeight * addition;
        setGrounded(false);
    }

    private void Dash(Vector3 dir)
    {
        anim.SetTrigger("Dash");
        dir.y += 0.5f;
        rb.AddForce(dir.normalized * dashForce, ForceMode.Impulse);

        lastTimeDashed = Time.time;

    }

    private void Shoot()
    {
        anim.SetTrigger("Attack");

        GameObject shot = PhotonNetwork.Instantiate(shootPlaceholder.name, shootingPoint.position, Quaternion.identity);
        Debug.Log("nickname set to this shot is: " + photonView.Owner.NickName);
        shot.GetComponent<ShotController>().SetName(photonView.Owner.NickName);
        shot.GetComponent<Rigidbody>().AddForce((eyes.forward) * shotForce, ForceMode.Impulse);


    }



    public void freezeMovement(bool toFreeze)
    {
        rb.constraints = toFreeze ? RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation : RigidbodyConstraints.FreezeRotation;

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

    protected void SetAnim(string animName)
    {
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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Ground"))
            setGrounded(true);
    }

}
