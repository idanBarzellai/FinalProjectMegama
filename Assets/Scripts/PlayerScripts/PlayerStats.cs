using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats _instance;
    public float maxHP = 100f;
    public float currHP = 100f;
    public float armore = 1;
    public float jumpForce = 6f;
    public int maxJumpCount = 2;
    public float speed = 6f;
    public float dmg = 5f;
    public string type = Elements.NATURAL;

    public Animator basicAnimator;
    public float attakSpeed = 1.5f;
    public bool canAttak = true;
    public bool isDash = false;
    public bool canMove = true;
    public bool isDead = false;

    public static PlayerStats Inctance
    {
        get
        {
            if (_instance == null)
            {
                var obj = GameObject.FindGameObjectWithTag("Player");
                if (obj != null)
                {
                    _instance = obj.GetComponent<PlayerStats>();
                }
            }
            return _instance;
        }

        set
        {
            _instance = value;
        }
    }

    void Awake()
    {
        _instance = this;
        currHP = maxHP;
    }

    void Update()
    {
        if(currHP <= 0)
        {
            dead();
        }
    }

    public void takeDmg(float dmg)
    {
        currHP -= dmg;
    }

    void dead()
    {
        basicAnimator.Play("Dead");
        isDead = true;
    }

    public void powerUp(string type)
    {
        switch (type)
        {
            case Elements.EARTH:
                powerUpEarth();
                break;
        }
        
    }

    void powerUpEarth()
    {

    }
}
