using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class FileTool 
{
    [MenuItem("自定义工具/动态加载路径自检")]
    private static void DataFiledetection()
    {
        if (!Directory.Exists(Application.dataPath + "/Resources/" +DataAndInitMgr.Instance.resourcesNecessaryAssetsPath))
            Directory.CreateDirectory(Application.dataPath + "/Resources" + DataAndInitMgr.Instance.resourcesNecessaryAssetsPath);

        if (!Directory.Exists(Application.dataPath + "/Resources/" +DataAndInitMgr.Instance.defaultResourcesPath))
            Directory.CreateDirectory(Application.dataPath + "/Resources" + DataAndInitMgr.Instance.defaultResourcesPath);

        //刷新Project窗口
        AssetDatabase.Refresh();
    }

}
