using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    void Start()
    {
        
    }
    private void Awake()
    {
        DataAndInitMgr.Instance.Init();
        UIMgr.Instance.Init();
        UIMgr.Instance.ShowOneUI<BeginPanel>();
        MusicMgr.Instance.Init();
        print(Application.persistentDataPath);
    }
}
