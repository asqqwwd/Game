using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 资源加载模块
/// 1. 异步加载
/// 2. 委托和lambda表达式
/// 3. 协程
/// 4. 泛型
/// </summary>
public class ResManager : BaseManager<ResManager>
{
    //同步加载资源
    public T Load<T>(string name) where T : Object
    {
        T res = Resources.Load<T>(name);
        //如果对象是gameobject类型的，直接实例化后返回出去，外部直接使用即可
        if (res is GameObject)
            return GameObject.Instantiate(res);
        else //TextAsset, Adutio Clip, etc..
            return res;        
    }

    //异步加载资源
    public void LoadAysnc<T>(string name, UnityAction<T> callback) where T : Object
    {
        //开启异步加载的协程
        MonoManager.GetInstance().StartCoroutine(ReallyLoadAsync(name, callback));
    }

    //真正的协同程序函数，用于开启异步加载对应的资源
    private IEnumerator ReallyLoadAsync<T>(string name, UnityAction<T> callback) where T : Object
    {
        ResourceRequest r = Resources.LoadAsync<T>(name);
        yield return r;

        if (r.asset is GameObject)
            callback(GameObject.Instantiate(r.asset) as T);
        else
            callback(r.asset as T);
    }
}
