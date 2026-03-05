using OpenCover.Framework.Model;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 数据行为接口，需要存入缓存池的自定义数需继承此接口
/// </summary>
public interface I_InitDataToPool
{
    public void DataFormatting();
}

/// <summary>
/// 缓存池
/// </summary>
public class PoolMgr : BaseMgr<PoolMgr>
{
    /// <summary>
    /// 缓存池数据基本单位,抽屉
    /// </summary>
    public class PoolData
    {
        //抽屉根物体
        public GameObject poolObj;

        //内容物，即为缓存池最小单位
        public Stack<GameObject> poolStack = new Stack<GameObject>();

        //使用中
        public List<GameObject> userList = new List<GameObject>();
        //此柜子的最大容量
        public int maxNum;
        public PoolData(string name, GameObject rootobj, int num)
        {
            if (PoolMgr.poolOpen)
            {
                poolObj = new GameObject(name);
                //设置柜子为父对象
                poolObj.transform.SetParent(rootobj.transform);
            }
            maxNum = num;
        }
        public int Count => poolStack.Count;
        public int userCount => userList.Count;
        //取出
        public GameObject PopObj(string name)
        {
            GameObject obj;
            if (Count > 0)
            {
                obj = poolStack.Pop();
            }
            else if (userCount <= maxNum - 1)//抽屉为空，但是使用中对象未达到最大容纳上限。索引从0开始，所以maxNub-1
            {
                //obj = GameObject.Instantiate(Resources.Load<GameObject>(name));
                //职责分离，因为管理器可能用resource加载或Addressable加载
                //所以数据类不负责资源创建
                return null;
            }
            else
            {
                //取出第0个对象，因为这是最早存入的内容
                obj = userList[0];
                //随后再次放入，刷新位置为末尾，因为需要返回，表示对象需要使用

            }
            userList.Add(obj);
            obj.name = name;
            return obj;
        }
        //存入
        public void PushObj(GameObject obj)
        {
            //压入抽屉
            poolStack.Push(obj);
            //从使用中列表移除
            userList.Remove(obj);
        }

        /// <summary>
        /// 设置布局优化的父对象以及控制显隐
        /// </summary>
        /// <param name="getIn">取出为false</param>
        public void SetPoolmgr(bool getIn, GameObject obj)
        {
            if (PoolMgr.poolOpen)
            {
                if (getIn)
                {
                    //存入对象时，设置父对象布局优化
                    obj.transform.SetParent(poolObj.transform, false);
                }
                else
                {
                    //断开联系
                    obj.transform.SetParent(null, false);
                }
            }
            obj.SetActive(getIn ? false : true);

        }
    }

    public abstract class BaseDataN { }
    public class DataN<T>:BaseDataN where T : class,I_InitDataToPool, new() 
    {
        public Queue<T> queue = new Queue<T>();
    }

    /// <summary>
    /// 缓存池基本逻辑
    /// </summary>
    public PoolMgr()
    {
        if(poolOpen)
            root = new GameObject("Pool");
        //过场景不移除，因为管理类有同样性质?
        //问题在于，父对象过场景不移除，但是子对象同样受到影响，逻辑不符
        //GameObject.DontDestroyOnLoad(root);
    }
    //柜子容器(gameObj
    public Dictionary<string,PoolData> valueDic = new Dictionary<string, PoolData>();

    //容器（存入自定义数据类
    public Dictionary<string,BaseDataN> nDataDic = new Dictionary<string, BaseDataN>();
    
    /// <summary>
    /// 柜子根对象
    /// </summary>
    private GameObject root;

    //需全局访问具体含义，表示是否开启布局优化
    //建议只在编辑器下开启
    public static bool poolOpen=true;

    #region 错误方式
    // <summary>
    // 布局优化基本动作单位
    //</summary>
    // <param name="getIn">默认false为取出</param>
    //// <param name="obj">需要改变的对象</param>
    ///// <param name="nameObj">对象名称</param>
    //public void SetObjtransform(bool getIn = false,GameObject obj=null,string nameObj="")
    //{
    //    if (poolOpen)
    //    {
    //        if(getIn)
    //        {

    //        }
    //    }
    //}
    //失败案例：先完成功能再考虑提取函数简化代码，过早优化永远是大忌！！！
    #endregion

    /// <summary>
    /// 拿东西,Resources加载
    /// 需注意，参数max只在第一次调用时候有效，之后同类抽屉容量上限以第一次传入参数为准
    /// </summary>
    /// <param name="assetName">对象名称</param>
    /// <param name="max">最大上限默认为10</param>
    /// <param name="path">资源来源</param>
    public GameObject GetPoolValue(string assetName,int max=10,string path="")
    {
        //此处用Resources加载，之后可替换为综合加载管理器
        //此处用资源的完整路径作为键，减少同名可能性
        string objName;
        if (path == "")
        {
            //若路径为空，则表示默认路径
            objName = DataAndInitMgr.Instance.defaultResourcesPath + assetName;
        }
        else
        {
            objName = path + assetName;
        }
        GameObject obj;
        if (valueDic.ContainsKey(objName))
        {
            obj = valueDic[objName].PopObj(objName);
            if (obj == null)
            {



                obj = GameObject.Instantiate(Resources.Load<GameObject>(objName));//实例化再返回
                valueDic[objName].userList.Add(obj);
                obj.name = objName;
            }
        }
        else
        {
            //确保柜子根对象存在
            if (poolOpen&&root==null)
                root = new GameObject("Pool");
            valueDic.Add(objName, new PoolData(objName+"pool",root,max));//因为只有此分支会new字典的值，所以可以理解为父对象存入的基本入口
            
            obj = GameObject.Instantiate(Resources.Load<GameObject>(objName));
            obj.name = objName;
            //存入正在使用对象列表
            valueDic[objName].userList.Add(obj);
        }

        //出厂设置
        valueDic[objName].SetPoolmgr(false,obj);
        return obj;
    }
    /// <summary>
    /// Addressable加载取东西,异步嵌套可能有时序问题，谨慎使用
    /// </summary>
    /// <param name="objName">资源名称</param>
    /// <param name="callBack">函数回调</param>
    /// <param name="max">最大限制</param>
    public void GetPoolValue(string objName, UnityAction<GameObject> callBack, int max = 10)
    {
        
        GameObject obj;
        if (valueDic.ContainsKey(objName))
        {
            obj = valueDic[objName].PopObj(objName);
            if (obj == null)
            {
                AddresableMge.Instance.LoadAssetAsyncI<GameObject>(objName, (hande) =>
                {
                    obj = GameObject.Instantiate(hande.Result);
                    obj.name = objName;
                    //存入正在使用对象列表
                    valueDic[objName].userList.Add(obj);
                    //出厂设置
                    valueDic[objName].SetPoolmgr(false, obj);
                    callBack(obj);

                    //因为创建逻辑统一由管理器管理，所以创建时的初始化也需要注意
                    valueDic[objName].userList.Add(obj);
                    obj.name = objName;

                });
            }
            //出厂设置
            valueDic[objName].SetPoolmgr(false, obj);
        }
        else
        {
            //确保柜子根对象存在
            if (poolOpen&&root==null)
                root = new GameObject("Pool");
            valueDic.Add(objName, new PoolData(objName + "pool", root, max));//因为只有此分支会new字典的值，所以可以理解为父对象存入的基本入口
            
            AddresableMge.Instance.LoadAssetAsyncI<GameObject>(objName, (hande) =>
            {
                obj=GameObject.Instantiate(hande.Result);
                obj.name = objName;
                //存入正在使用对象列表
                valueDic[objName].userList.Add(obj);
                //出厂设置
                valueDic[objName].SetPoolmgr(false, obj);
                callBack(obj);
            });

        }

    }
    /// <summary>
    /// 从缓存池取出自定义类数据
    /// </summary>
    /// <typeparam name="T">类型需继承接口</typeparam>
    /// <param name="nameSpace">命名空间</param>
    /// <returns></returns>
    public T GetPoolValue<T>(string nameSpace="") where T : class, I_InitDataToPool,new()
    {
        string nameID=nameSpace+"_"+typeof(T).Name;
        T tObj;
        if (nDataDic.ContainsKey(nameID))
        {
            tObj = (nDataDic[nameID] as DataN<T>).queue.Dequeue();
        }
        else
        {
            tObj=new T();
        }
        return tObj;
    }


    //放东西
    public void PushInObj(GameObject obj)
    {
        if (valueDic.ContainsKey(obj.name))
        {
            //入厂设置
            valueDic[obj.name].SetPoolmgr(true,obj);
            valueDic[obj.name].PushObj(obj);
        }
    }
    public void PushInObj<T>(T obj,string nameSpace="") where T :class, I_InitDataToPool,new()
    {
        //如果想要压入null对象 是不被允许的
        if (obj == null)
        {
            Debug.LogError("不允许往缓存池存入null数据");
            return;
        }
            
        string nameID=nameSpace+"_"+typeof(T).Name;
        //格式化数据，便于取时直接使用
        obj.DataFormatting();
        if (!nDataDic.ContainsKey(nameID))
        {
            nDataDic.Add(nameID, new DataN<T>());
        }
        (nDataDic[nameID] as DataN<T>).queue.Enqueue(obj);
    }

    /// <summary>
    /// 清空容器，过场景可使用
    /// </summary>
    public void ClearPoolObj()
    {
        valueDic.Clear();
        nDataDic.Clear();
        //清空引用，要使用再创建
        root = null;
    }


    //说明：从缓存池取出的对象尽量不要在外部有引用，生命周期全程由缓存池自动管理！！！
    //若有引用，请务必在存入缓存池时，清除外部引用，否则有内存泄漏风险！！！
}
