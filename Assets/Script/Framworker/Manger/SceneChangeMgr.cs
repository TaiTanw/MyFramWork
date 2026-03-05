using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/// <summary>
/// 场景切换管理器
/// </summary>
public class SceneChangeMgr : BaseMgr<SceneChangeMgr>
{
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    public void LoadSceneAsync(string sceneName,UnityAction callBack)
    {
        AsyncOperation tion = SceneManager.LoadSceneAsync(sceneName);
        MonoPublicMgr.Instance.StartCoroutine(Load(tion,callBack));
    }

    IEnumerator Load(AsyncOperation ao,UnityAction action)
    {
        while (!ao.isDone)
        {
            //事件分发，用于外部获取加载进度
            EventCenterSystem.Instance.EventTrigger<float>(E_EventEnum.E_LoadScene,ao.progress);
            yield return 0;
        }

        EventCenterSystem.Instance.EventTrigger<float>(E_EventEnum.E_LoadScene, 1);
        action?.Invoke();
    }
}
