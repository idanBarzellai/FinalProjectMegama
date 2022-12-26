using Photon.Pun;
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
    public IEnumerator DestroyOvertime(float waitBeforeDestroy)
    {
        yield return new WaitForSecondsRealtime(waitBeforeDestroy);

        //looping = isFire;
        DestroyEachAtATime();

        //while (looping)
        //{
        //    DestroyEachAtATime();
        //    yield return new WaitForSecondsRealtime(0.2f);
        //}
    }

    private void DestroyEachAtATime()
    {
        if (currectActiveSkillInstances.Count > 0)
        {
            GameObject obj = currectActiveSkillInstances[0];
            currectActiveSkillInstances.RemoveAt(0);
            PhotonNetwork.Destroy(obj);
        }
        else
        {
            currectActiveSkillInstances.Clear();
            //looping= false;
        }
    }
}
