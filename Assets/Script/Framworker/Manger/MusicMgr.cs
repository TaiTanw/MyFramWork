using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Experimental.GlobalIllumination;
using static Unity.VisualScripting.Member;

public class MusicMgr : BaseMgr<MusicMgr>
{
    public MusicMgr()
    {
        //不能在 MonoBehaviour 构造函数里 new GameObject
        //因为构造函数执行时：

        //引擎还没准备好

        //场景系统还没初始化

        //GameObject 系统不可用
        //MonoPublicMgr.Instance.AddFixedUpdateEvent(UpdateT);
        //Debug.Log("音乐管理器加载成功");
    }
    /// <summary>
    /// 循环删除非循环已结束音效
    /// </summary>
    private void UpdateT()
    {
        //需要逆向遍历删除
        if(isDelete)
        {
            for(int i = ListSoundMusics.Count-1; i >=0; i--)
            {
                if (!ListSoundMusics[i].isPlaying)
                {
                    ListSoundMusics[i].clip = null;
                    PoolMgr.Instance.PushInObj(ListSoundMusics[i].gameObject);
                    ListSoundMusics.Remove(ListSoundMusics[i]);
                }
            }
        }
    }

    private AudioSource BkMusic;
    /// <summary>
    /// 用于配置默认的开始背景音效名称，用于Addressable加载的资源名
    /// </summary>
    private string BkDefaultMusicName = "BkMusic";

    public float BkVolume ;
    public bool BkMute ;

    private List<AudioSource> ListSoundMusics=new List<AudioSource>();

    public float SoundVolume ;
    public bool SoundMute;

    /// <summary>
    /// 删除准许，默认为true，暂停时为false
    /// </summary>
    private bool isDelete=true;


    /// <summary>
    /// 初始化
    /// </summary>
    public void Init()
    {
        if(BkMusic==null)
        {
            MonoPublicMgr.Instance.AddFixedUpdateEvent(UpdateT);
            Debug.Log("音乐管理器加载成功");
            //关联背景音乐的主要组件和脚本
            GameObject obj = new GameObject("BkMusicAudio");
            BkMusic = obj.AddComponent<AudioSource>();
            GameObject.DontDestroyOnLoad(obj);//过场景不移除
        }

        //读取本地音乐设置文件
        BkMute = DataAndInitMgr.Instance.musicData.bkMusicOpen;
        SoundMute = DataAndInitMgr.Instance.musicData.soundOpen;
        BkVolume = DataAndInitMgr.Instance.musicData.bkMusicValue;
        SoundVolume = DataAndInitMgr.Instance.musicData.soundValue;

        //根据数据设置相关数据
        PlayBkMusic(BkDefaultMusicName);
        SetBkMusicVolume(BkVolume);
        PauseOrPlaySoundMusic(SoundMute);
        SetSoundVolume(SoundVolume);

        Debug.Log(BkMute+""+ SoundMute+ BkVolume+ SoundVolume);
    }

    #region 背景音乐相关
    /// <summary>
    /// 播放背景音乐
    /// </summary>
    /// <param name="name">资源名称</param>
    public void PlayBkMusic(string name)//重复调用会使背景音乐重新播放，需注意
    {
        
        AddresableMge.Instance.LoadAssetAsyncI<AudioClip>(name, (hande) =>
        {
            //回调传入加载好的资源，是否成功加载会在内部自动判断
            BkMusic.clip = hande.Result;
            BkMusic.loop=true;//循环
            BkMusic.volume = BkVolume;//音量大小
            if (BkMute)
            {
                BkMusic.Play();
            }
            else
            {
                StopBkMusic();
            }
        });
    }
    public void StartBkMusic()//开启背景音乐
    {
        if (BkMusic != null)
        {
            BkMusic.Play();
        }
    }
    /// <summary>
    /// 停止背景音乐
    /// </summary>
    /// <param name="toDelete">默认为false表示停止但不卸载资源</param>
    /// <param name="assetName">资源名称</param>
    public void StopBkMusic(bool toDelete=false,string assetName="")
    {
        if (BkMusic != null)
        {
            BkMusic.Stop();
            if (toDelete)
            {
                if(AddresableMge.Instance.FindNameConfirmexistence(assetName))
                    AddresableMge.Instance.Release<AudioClip>(assetName);
            }
                
        }
    }
    public void PauseBKMusic()//暂停
    {
        if (BkMusic != null)
        {
            BkMusic.Pause();
        }
    }
    //设置背景音乐大小
    public void SetBkMusicVolume(float value)
    {
        //if (BkMusic != null) 
        //    BkMusic.volume=value;//此写法只能在背景音乐存在时候修改，缺乏灵活
        BkVolume = value;
        if (BkMusic != null)
        {
            BkMusic.volume = BkVolume;
        }
    }
    #endregion

    #region 音效相关
    /// <summary>
    /// 播放声效,addressable异步加载
    /// </summary>
    /// <param name="name">音效名称</param>
    /// <param name="isLoop">是否循环</param>
    /// <param name="callBack">函数回调，只在循环音效使用</param>
    public void StartSound(string name, bool isLoop = false, UnityAction<AudioSource> callBack = null)
    {
        if (SoundMute == false)
        {
            return;
        }
        //此处已经设置音效最大同时存在15个
        AudioSource sound = PoolMgr.Instance.
            GetPoolValue("MusicPlayObj",15,DataAndInitMgr.Instance.resourcesNecessaryAssetsPath).GetComponent<AudioSource>();
        AddresableMge.Instance.LoadAssetAsyncI<AudioClip>(name, (hande) =>
        {
            //防止出错，先停止播放
            sound.Stop();

            sound.clip = hande.Result;
            sound.loop = isLoop;
            sound.volume = SoundVolume;
            sound.Play();

            //由于从缓存池中取出对象 有可能取出一个之前正在使用的（超上限时）
            //所以我们需要判断 容器中没有记录再去记录 不要重复去添加即可
            if (!ListSoundMusics.Contains(sound))
                ListSoundMusics.Add(sound);
            callBack?.Invoke(sound);
        });

    }

    //public void StartSound(string name, bool isLoop = false, UnityAction<AudioSource> callBack = null)
    //{
    //    //循环音效需要外部控制结束，所以需要回调
    //    AudioSource sound;
    //    PoolMgr.Instance.GetPoolValue("MusicPlayObj", (obj) =>
    //    {
    //        sound = obj.GetComponent<AudioSource>();
    //        AddresableMge.Instance.LoadAssetAsyncI<AudioClip>(name, (hande) =>
    //        {
    //            sound.Stop();

    //            sound.clip = hande.Result;
    //            sound.loop = isLoop;
    //            sound.volume = SoundVolume;
    //            sound.Play();
    //            if (!ListSoundMusics.Contains(sound))
    //                 ListSoundMusics.Add(sound);
    //                callBack?.Invoke(sound);
    //        });
    //    });
    //}//异步问题，暂时弃用，之后来改

    /// <summary>
    /// 设置音效大小
    /// </summary>
    /// <param name="value"></param>
    public void SetSoundVolume(float value)
    {
        SoundVolume = value;
        foreach(AudioSource sound in ListSoundMusics)
        {
            sound.volume = SoundVolume;
        }
    }
    /// <summary>
    /// 停止播放指定音效
    /// </summary>
    /// <param name="sound">要停止的音效组件</param>
    public void StopSoundMusic(AudioSource sound)
    {
        if (ListSoundMusics.Contains(sound))
        {
            sound.Stop();
            //清除文件引用
            sound.clip = null;
            //从音效容器中移除
            ListSoundMusics.Remove(sound);
            //依附对象放入缓存池
            PoolMgr.Instance.PushInObj(sound.gameObject);
        }
    }
    /// <summary>
    /// 音效暂停开关
    /// </summary>
    /// <param name="isPlay">true为播放</param>
    public void PauseOrPlaySoundMusic(bool isPlay)
    {
        isDelete=isPlay;
        foreach(AudioSource sound in ListSoundMusics)
        {
            if (isPlay)
            {
                sound.Play();
            }
            else
            {
                sound.Pause();
            }
        }
    }
    #endregion
}
