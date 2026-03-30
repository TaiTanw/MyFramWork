using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    void Start()
    {
        //读表工具测试代码
        //GameObjData data = new GameObjData();
        //print( data.TowerInfoDic[1].name );
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
