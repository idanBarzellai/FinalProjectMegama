using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using Cinemachine.Utility;

public class CharcterController : MonoBehaviourPunCallbacks
{
    public bool invertLook;
    public Transform eyes;
    public Transform viewPoint;
    public Transform shootingPoint;
    public float mouseSensitivity = 2;


    public GameObject shootPlaceholder;
    public GameObject playerHitEffect;

    public float jumpHeight = 6.5f, fallMultiplyer = 1.8f;
    public float dashForce = 8f;
    public bool isGrounded = true;
    public float rayhit = 1.3f;

    private Rigidbody rb;
    private Animator anim;
    public GameObject skillObject;
    public GameObject skillObject2;
    public GameObject skillObject4;

    private Vector3 dashDirection;
    private Vector3 moveDir;
    private float activeSpeed;
    public float moveSpeed = 8f, runSpeed = 12f;

    private float lastTimeDashed = 0f, dashCooldown = 1f;

    public float shotForce = 20f;

    private bool inSkill = false;
    private bool isStaticSkill = false;
    private bool isRotationStaticSkill = false;

    private float verticalRotStore;
    private Vector2 mouseInput;

    private Camera cam;


    void Start()
    {

        Cursor.lockState = CursorLockMode.Locked;

        cam = Camera.main;

        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        
        activeSpeed = moveSpeed;
    }

    void Update()
    {
        if (photonView.IsMine)
        {
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
                //if (isGrounded && Input.GetButtonDown("Jump") && Physics.Raycast(transform.position, Vector3.down, rayhit))
                //    Jump();

                // Dashing 
                if (Input.GetKeyDown(KeyCode.LeftShift) && Time.time - lastTimeDashed > dashCooldown)
                {
                    dashDirection = (moveDir.z * transform.forward) + (transform.right * moveDir.x);

                    Dash(dashDirection);
                }

                if (Input.GetKeyDown(KeyCode.J)) // Jump
                    skillObject.GetComponent<SkillHelper>().airSkill();

                if (Input.GetKeyDown(KeyCode.T)) // Tsunami
                    skillObject2.GetComponent<SkillHelper2>().waterSkill();

                if (Input.GetKeyDown(KeyCode.F)) // Fire
                    this.GetComponent<SkillHelper3>().fireSkill();

                if (Input.GetKeyDown(KeyCode.E)) // Earth
                    skillObject4.GetComponent<SkillHelper4>().skillStarted();
            }

            // WaterSkill
            if (Input.GetKeyUp(KeyCode.T))
                skillObject2.GetComponent<SkillHelper2>().resetVariables();

            if (Input.GetKeyUp(KeyCode.E))
                skillObject4.GetComponent<SkillHelper4>().earthSkill();

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
    private void FixedUpdate()
    {

        if (photonView.IsMine)
        {
            if (!inSkill)
            {
                rb.AddForce(Physics.gravity * fallMultiplyer * rb.mass);
                // Jumping
                if (isGrounded && Input.GetButtonDown("Jump") && Physics.Raycast(transform.position, Vector3.down, rayhit))
                    Jump();

            }
            else
            {
                // Check minmumHeight
                if (transform.position.y > 18)
                {
                    rb.velocity = new Vector3(rb.velocity.x, Physics.gravity.y, rb.velocity.z);
                }
            }
        }
    }

    private void LateUpdate()
    {
        if (photonView.IsMine)
        {
            cam.transform.position = viewPoint.position;
            cam.transform.rotation = viewPoint.rotation;
        }
    }
    public void Jump(float addition = 1f)
    {
        rb.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
        setGrounded(false);
    }
    //public void Jump(float addition = 1f)
    //{
    //    rb.velocity += Vector3.up * jumpHeight * addition;
    //    setGrounded(false);
    //}

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

        GameObject shot = PhotonNetwork.Instantiate(shootPlaceholder.name,shootingPoint.position, Quaternion.identity);
        Debug.Log("nickname set to this shot is: " + photonView.Owner.NickName);
        shot.GetComponent<ShotController>().SetName(photonView.Owner.NickName);
        shot.GetComponent<Rigidbody>().AddForce((eyes.forward) * shotForce, ForceMode.Impulse);

        Destroy(shot, 2f);
       
    }

    public void freezeMovement(bool toFreeze)
    {
        rb.constraints = toFreeze? RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation : RigidbodyConstraints.FreezeRotation;

    }

    // Getters
    public bool GetInSkill()
    {
        return inSkill;
    }

    public bool GetIsStaticSkill()
    {
        return isStaticSkill;
    }
    public float GetActiveSpeed()
    {
        return activeSpeed;
    }


    // Setters
    public void setGrounded(bool grounded)
    {
        isGrounded = grounded;  
    }

    public void SetInSkill(bool isInSkill)
    {
        inSkill = isInSkill;
    }

    public void SetIsStaticSkill(bool isStatic)
    {
        isStaticSkill= isStatic;
    }

    public void SetIsRotationStaticSkill(bool isStatic)
    {
        isRotationStaticSkill = isStatic;
    }

    public void SetAnim(string animName)
    {
        anim.SetTrigger(animName);
    }

    public void SetSpeed(float speed)
    {
        activeSpeed = speed;
    }

    public void SetFallMultiplyer(float force)
    {
        fallMultiplyer = force;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Ground"))
            setGrounded(true);
    }



   
}
