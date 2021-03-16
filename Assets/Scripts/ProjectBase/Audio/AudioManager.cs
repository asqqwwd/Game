using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AudioManager : BaseManager<AudioManager>
{
    private AudioSource bkMusic; //背景音乐唯一
    private float bkValue = 1;

    private GameObject audioObj = null; //全局2D音效
    private List<AudioSource> audioList = new List<AudioSource>();
    private float audioValue = 1;

    //加入循环检测，目的是自动移除播放完的音效（audio clip本身没有回调函数来告知音效已播放完）
    public AudioManager()
    {
        MonoManager.GetInstance().AddUpdateListener(Update);
    }

    private void Update()
    {
        for (int i = audioList.Count - 1; i >= 0; i--) 
        {
            if(!audioList[i].isPlaying)
            {
                GameObject.Destroy(audioList[i]);
                audioList.RemoveAt(i);
            }
        }
    }

    public void PlayBKMusic(string name)
    {
        if (bkMusic == null)
        {
            GameObject obj = new GameObject();
            obj.name = "Back Ground Music";
            bkMusic = obj.AddComponent<AudioSource>();
        }

        //异步加载背景音乐，加载完成后播放
        ResManager.GetInstance().LoadAysnc<AudioClip>(name, (clip) => 
        {
            bkMusic.clip = clip;
            bkMusic.volume = bkValue;
            bkMusic.loop = true;
            bkMusic.Play();
        });
    }

    /// <summary>
    /// 改变音量大小
    /// </summary>
    /// <param name="v"></param>
    public void ChangeBKValue(float v)
    {
        bkValue = v;
        if (bkMusic == null)
            return;
        bkMusic.volume = bkValue;
    }


    /// <summary>
    /// 暂停背景音乐
    /// </summary>
    public void PauseBKMusic()
    {
        if (bkMusic == null)
            return;
        bkMusic.Pause();
    }


    public void StopBKMusic()
    {
        if (bkMusic == null)
            return;
        bkMusic.Stop();
    }

    /// <summary>
    /// 播放音效
    /// </summary>
    public void PlayAudio(string name, bool isLoop, UnityAction<AudioSource> callBack = null)
    {
        if(audioObj == null)
        {
            audioObj = new GameObject();
            audioObj.name = "Global Audio";
        }

        //当音效资源异步加载结束后，再添加一个音效
        ResManager.GetInstance().LoadAysnc<AudioClip>(name, (clip) =>
        {
            AudioSource source = audioObj.AddComponent<AudioSource>();
            source.clip = clip;
            source.loop = isLoop;
            source.volume = audioValue;
            source.Play();
            audioList.Add(source);
            if (callBack != null)
                callBack(source);
        });
    }

    /// <summary>
    /// 改变声音大小
    /// </summary>
    /// <param name="value"></param>
    public void ChangeSoundValue(float value)
    {
        audioValue = value;
        for (int i = 0; i < audioList.Count; ++i)
        {
            audioList[i].volume = value;
        }
    }

    /// <summary>
    /// 停止音效
    /// </summary>
    public void StopAudio(AudioSource source)
    {
        if (audioList.Contains(source))
        {
            audioList.Remove(source);
            source.Stop();
            GameObject.Destroy(source);
        }
    }
}
