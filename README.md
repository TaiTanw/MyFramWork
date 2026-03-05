# MyFramework

一个基于 Unity 的轻量级游戏开发框架，用于提高游戏开发效率，
实现模块化管理、常用功能封装以及统一的开发规范。

该框架整合了常见游戏开发模块，如：
UI管理、资源管理、对象池、计时器、数据管理等。

---

## 项目介绍

MyFramework 是在学习 Unity 游戏开发过程中，
结合课程内容和个人理解逐步搭建的一个基础框架。

主要目标：

- 提高项目结构清晰度
- 提供常用系统的统一管理方式
- 减少重复代码
- 提高开发效率

---

## 技术栈

- Unity
- C#
- 单例模式
- 泛型
- 反射
- 对象池
- 事件系统
- Addressables（资源管理）
- 二进制数据持久化

---

## 框架结构

框架主要分为以下几个模块：

Manager 层
- AddressableMge 可寻址资源加载管理器
- DataAndInitMgr 数据加载与初始化管理
- EventCenterSystem 事件中心
- MonoPublicMgr 公共Mono模块
- MusicMgr 音乐管理
- PoolMgr 缓存池管理器
- SceneChangeMgr 场景管理器
- TimerMgr 计时器
- UIMgr UI管理器

Base基类层
- BaseAutoMonoMgr 继承Mono的自动挂载管理器
- BaseMgr 普通管理器
- BasePanel 基础UI类

工具层
- Excel数据工具
- 二进制数据读写
- 常用工具类

---

## 核心功能

### UI管理系统
统一管理UI的打开、关闭、层级以及生命周期。

### 资源管理系统
封装 Unity的Addressable 资源加载流程

### 对象池系统
减少频繁创建销毁对象带来的性能开销。

### 计时器系统
提供统一的计时器管理，可用于技能CD、延时执行等场景。

### 数据管理
支持二进制数据存储和读取。

---

## 使用注意事项

一，资源动态加载注意事项和必备预设体导入

此框架需涉及组件：Addressable ，InputSystem
需要使用resources加载且会放入缓存池的对象，请放入Resources/default路径下，自定义工具内点击“动态加载路径自检”可自动创建相关文件夹，并在创建的NecessaryAssets文件下，放入框架必备的相关预设体，包括：MusicPlayObj/UICamera/Canvas/EventSystem

二，UI相关
	
	
1，所有UI都通过代码控制
（如果直接将UI相关组件放入场景作为默认显示，则在切换场景时候会导致重复组件出现）

2，所有UI必须将层级设置为UI，否则摄像机不显示

3，需要代码监听和控制的UI组件，不能是默认名称
（代码用名称作为监听事件的判断条件，用于分辨同类控件不同按钮）

4，UI对象名称必须与：UI脚本名称，放入Addressable的包名一致

三，改键相关

1，若需要添加或移除输入事件：
	（1）则先在DataAndInitMgr内修改按键输入枚举
	（2）然后在改键面板逻辑内的ChangeBtnReally修改switch内的逻辑
	（3）随后根据按键事件创建input action
	（4）随后在文件内，将所有绑定的path部分，换成<>，尖括号内填入事件名称（一致性）	（5）随后导出为json文件，以player为固定名称，直接存入Resources，路径若需要修改，则对应修改DataAndInitMgr第133行

2，UI面板逻辑在start内关联名称固定为player的玩家对象，需注意命名规范
3，玩家名称固定为player（唯一），需挂载playerinput组件，Behavior设置为invokeCSharpEvent

---
##其他
涉及资源加载的其他类，例如音乐管理类，因为使用Addressable异步加载涉及到异步嵌套，产生不稳定的问题，暂未解决，因此混合了部分Resources，用于暂时解决问题
