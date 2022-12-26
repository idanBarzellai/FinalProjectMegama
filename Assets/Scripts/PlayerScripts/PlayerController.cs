using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private CharacterController playerController;
    private Vector3 moveDirection;
    private float verticalVelocity;
    private int jumpCount = 0;
    private float nextAttakAt = 0;


    public float gravity = 20;
    public Animator basicAnimator;
    public Animator basicAttakAnimation;


    void Awake()
    {
        playerController = GetComponent<CharacterController>();
    }

    void Start()
    {

    }

    private void Update()
    {
        if (!PlayerStats.Inctance.isDead)
        {
            movePlayer();
            playerAttak();
        }

    }

    void movePlayer()
    {
        if (!PlayerStats.Inctance.isDash)
        {
            moveDirection = new Vector3(Input.GetAxis(Axis.HORIZONTAL), 0f, Input.GetAxis(Axis.VERTICAL));
            runAnimationControll(moveDirection);
            moveDirection = transform.TransformDirection(moveDirection);
            moveDirection *= PlayerStats.Inctance.speed * Time.deltaTime;
            applyGravity();
            jumpCheck();
            moveDirection.y = verticalVelocity * Time.deltaTime;
        }
        if (Input.GetKeyDown(KeyCode.Mouse1)) StartCoroutine("dash");  
        playerController.Move(moveDirection);
    }
    void applyGravity()
    {
        if (!playerController.isGrounded)
        {
            verticalVelocity -= gravity * Time.deltaTime;
        }
        
    }
    void jumpCheck()
    {
        if (playerController.isGrounded) jumpCount = 0;
        if (Input.GetKeyDown(KeyCode.Space) && jumpCount < PlayerStats.Inctance.maxJumpCount)
        {
            verticalVelocity = PlayerStats.Inctance.jumpForce;
            jumpCount++;
        }  
    }

    IEnumerator dash()
    {
        basicAnimator.Play("Dash");
        for (float i = 7.5f; i >= 0; i -= 0.1f)
        {
            PlayerStats.Inctance.canAttak = false;
            PlayerStats.Inctance.isDash = true;
            moveDirection = new Vector3(0f, 0f, 0.1f);
            moveDirection = transform.TransformDirection(moveDirection);
            yield return null;
        }
        PlayerStats.Inctance.canAttak = true;
        PlayerStats.Inctance.isDash = false;
    }

    void playerAttak()
    {
        if (Input.GetKey(KeyCode.Mouse0) && PlayerStats.Inctance.canAttak)
        {
            if (nextAttakAt <= Time.time)
            {
                basicAnimator.Play("Attak");
                basicAttakAnimation.Play("BasicAttakAnimation_Attak");
                nextAttakAt = Time.time + (1 / PlayerStats.Inctance.attakSpeed);
            }
        }
    }

    void runAnimationControll(Vector3 runDirection)
    {
        if (runDirection.x != 0 || runDirection.z != 0)
        {
            if (runDirection.z >= 0)
            {
                basicAnimator.ResetTrigger("RunBack");
                basicAnimator.ResetTrigger("Idle");
                basicAnimator.SetTrigger("Run");
            }
            else
            {
                basicAnimator.ResetTrigger("Idle");
                basicAnimator.ResetTrigger("Run");
                basicAnimator.SetTrigger("RunBack");
            }
        }
        else
        {
            //basicAnimator.CrossFade("Idle",0.5f);
            basicAnimator.ResetTrigger("RunBack");
            basicAnimator.ResetTrigger("Run");
            basicAnimator.SetTrigger("Idle");
        }

    }
}
