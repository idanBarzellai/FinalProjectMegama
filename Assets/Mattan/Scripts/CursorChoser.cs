using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CursorChoser : MonoBehaviour
{
    public static CursorChoser instance;
    void Start()
    {
        if (instance != null) Destroy(gameObject);
        instance = this;

        DontDestroyOnLoad(gameObject);       
             
        PickNewCursor();
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode){PickNewCursor();}

    void PickNewCursor(){}
}
