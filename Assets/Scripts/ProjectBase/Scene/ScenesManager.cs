using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/// <summary>
/// 场景切换模块
/// 1. 场景异步加载
/// 2. 协程
/// 3. 委托
/// </summary>
public class ScenesManager : BaseManager<ScenesManager>
{
    /// <summary>
    /// 切换场景：同步加载
    /// </summary>
    /// <param name="name"></param>
    public void LoadScene(string name, UnityAction func)
    {
        //场景同步加载
        SceneManager.LoadScene(name);
        //加载完成过后才会去执行func
        func();
    }

    /// <summary>
    /// 提供给外部的异步加载的接口方法
    /// </summary>
    /// <param name="name"></param>
    /// <param name="func"></param>
    public void LoadSceneAsync(string name, UnityAction func)
    {
        MonoManager.GetInstance().StartCoroutine(ReallyLoadSceneAsync(name, func));
    }

    /// <summary>
    /// 协程异步加载场景
    /// </summary>
    /// <param name="name"></param>
    /// <param name="func"></param>
    /// <returns></returns>
    private IEnumerator ReallyLoadSceneAsync(string name, UnityAction func)
    {
        AsyncOperation ao = SceneManager.LoadSceneAsync(name);
        //可以得到场景加载的一个进度
        while (!ao.isDone)
        {
            //事件中心，向外分发进度情况，外面想用就用
            EventManager.GetInstance().EventTrigger("进度条更新", ao.progress);
            //这里去更新进度条
            yield return ao.progress;
        }
        //加载完成后才回去执行func
        func();
    }
}
