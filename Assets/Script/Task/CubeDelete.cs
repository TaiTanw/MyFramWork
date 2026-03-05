using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeDelete : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void OnEnable()
    {
        this.transform.position = Vector3.zero;
        //激活时调用
        Invoke("Delete", 1f);
    }
    private void Delete()
    {
        
        //放入缓存池
        PoolMgr.Instance.PushInObj(this.gameObject);
        //对象失活建议放在自定义“删除”函数内，否则在生命周期函数调用会出错
    }
    // Update is called once per frame
    void Update()
    {
        this.transform.Translate(10 * Time.deltaTime * Vector3.forward);
    }
    private void OnDisable()
    {
        //PoolMgr.Instance.PushInObj(this.gameObject);
        //生命周期回调中，不允许再次改变生命周期状态
    }
}

