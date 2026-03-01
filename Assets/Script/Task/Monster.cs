using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    // Start is called before the first frame update
    public int myMoney = 100;
    void Start()
    {
        
    }
    public void Dead()
    {
        //触发事件
        EventCenterSystem.Instance.EventTrigger<Monster>(E_EventEnum.Monster_Dead, this);
        print("事件触发");
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
