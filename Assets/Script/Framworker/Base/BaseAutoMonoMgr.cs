using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 自动挂载继承Mono的单例模式
/// </summary>
/// <typeparam name="T"></typeparam>
public class BaseAutoMonoMgr<T> : MonoBehaviour where T : MonoBehaviour
{

    private static T instance;

    public static T Instance
    {
        get 
        {
            if (instance == null)
            {
                GameObject obj = new GameObject(typeof(T).Name);
                //只有继承mono才可以使用添加脚本方法
                //关联示例化单例和已添加脚本
                instance = obj.AddComponent<T>();

                //管理器过场景不移除
                DontDestroyOnLoad(obj);
            }
            return instance; 
        }
    }
  
}
