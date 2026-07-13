using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{

    public static Main Instance;
    void Start()
    {
        //读表工具测试代码
        //GameObjData data = new GameObjData();
        //print( data.TowerInfoDic[1].name );
    }
    private void Awake()
    {

        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        DataAndInitMgr.Instance.Init();
        UIMgr.Instance.Init();
        UIMgr.Instance.ShowOneUI<BeginPanel>();
        MusicMgr.Instance.Init();
        print(Application.persistentDataPath);
    }
}
