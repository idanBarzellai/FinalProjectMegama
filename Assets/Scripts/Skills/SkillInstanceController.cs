using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillInstanceController : MonoBehaviourPunCallbacks
{
    public GameObject hitEffect;
    protected int dmg;
    public string playerName;
    protected BasicsController player;
    protected float lifetime = 5f;
    protected SkillManager skillManager;
    protected virtual void Start()
    {
        skillManager = SkillManager.instance;
    }

    
    public void SetName(string name)
    {
        playerName = name;
    }

    public void SetPlayer(BasicsController _player)
    {
        player= _player;
    }
}
