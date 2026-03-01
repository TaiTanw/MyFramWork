using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    // Start is called before the first frame update
    public int myMoney = 0;
    public PlayerInput input;
    void Start()
    {
        //序列化情况测试
        //Debug.Log(DataAndInitMgr.Instance.asset.FindAction("Move").bindings[1].effectivePath);

        //注册事件监听
        EventCenterSystem.Instance.AddEventListener<Monster>(E_EventEnum.Monster_Dead, GetMoney);
        input =this.gameObject.GetComponent<PlayerInput>();
        if(input == null)
        {
            //gameObject的方法，注意不要直接类点出添加脚本
            //input=this.gameObject.AddComponent<PlayerInput>();
            //组件初始化可能有时序问题，此处建议直接挂载相应组件
            print("请为player对象挂载playerinput组件,Behavior设置为invokeCSharpEvent");
            
        }

        ChangeInputAsset();//关联action唯一入口

        //注册按键响应
        input.onActionTriggered += (callBack) =>
        {
            //确保是触发下响应
            if (callBack.phase == InputActionPhase.Performed)
            {
                //用于测试当前触发的action名称
                //Debug.Log("Action Triggered: " + callBack.action.name);
                switch (callBack.action.name) 
                {
                    case "Move":
                        print("移动");
                        print(callBack.ReadValue<Vector2>());
                        break;

                    case "Fire":
                        print("开火");
                        break ;
                }
            }
        };
    }
    public void GetMoney(Monster monster)
    {
        myMoney += monster.myMoney;
        print("当前拥有"+myMoney+"钱");
    }
    
    public void ChangeInputAsset(bool isOpnen=true)
    {
        input.actions = DataAndInitMgr.Instance.asset;
        //开启
        if (isOpnen )
        {
            input.actions.Enable();
        }
        else
        {
            input.actions.Disable();
        }

    }

    void Update()
    {
        
    }
}
