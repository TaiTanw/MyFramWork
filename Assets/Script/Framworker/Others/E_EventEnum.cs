using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 事件类型枚举
/// </summary>
public enum E_EventEnum
{   //死亡触发，参数自身
    Monster_Dead,

    //执行者获利
    E_Player_GetReward,

    E_LoadScene,
}
