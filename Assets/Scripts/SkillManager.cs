using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviourPunCallbacks
{
    public static SkillManager instance;
    public List<GameObject> currectActiveSkillInstances =  new List<GameObject>();
    //bool looping = false;

    private void Awake()
    {
        instance = this;
      

    }
    private void Update()
    {
        
    }

    public void DestoryOverNetwork(float waitBeforeDestroy, GameObject obj)
    {
        StartCoroutine(DestroyOvertime(waitBeforeDestroy, obj));
    }
    public IEnumerator DestroyOvertime(float waitBeforeDestroy, GameObject obj)
    {
        yield return new WaitForSecondsRealtime(waitBeforeDestroy);
        if (obj)
        {
            PhotonNetwork.Destroy(obj);
        }
        //DestroyEachAtATime();
    }

    //private void DestroyEachAtATime()
    //{
    //    if (currectActiveSkillInstances.Count > 0)
    //    {
    //        GameObject obj = currectActiveSkillInstances[0];
    //        if (obj)
    //        {
    //            currectActiveSkillInstances.RemoveAt(0);
    //            PhotonNetwork.Destroy(obj);
    //        }
    //    }
    //    else
    //        currectActiveSkillInstances.Clear();
    //}

    //private void ClearActiveSkills()
    //{

    //}
}
