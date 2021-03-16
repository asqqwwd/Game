using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//利用空接口实现泛型在字典里的封装
public interface IEventInfo { }

public class EventInfo<T> : IEventInfo
{
    public UnityAction<T> actions;

    public EventInfo(UnityAction<T> action)
    {
        actions += action;
    }
}

/// <summary>
/// 不带泛型的参数的
/// </summary>
public class EventInfo : IEventInfo
{
    public UnityAction actions;
    public EventInfo(UnityAction action)
    {
        actions += action;
    }
}


/// <summary>
/// 事件中心 单例模式对象
/// 1. Dictionary
/// 2. 委托
/// 3. 观察者设计模式
/// 4. 泛型，里式转换原则
/// </summary>
public class EventManager : BaseManager<EventManager>
{
    //key —— 事件的名字（怪物死亡，玩家通关，etc..）
    //value —— 对应监听这个事件对应的委托函数们
    ///委托没有返回值，有一个object参数 x
    ///使用object泛型是为了通用性，会牺牲一些装箱拆箱的性能开销 x
    private Dictionary<string, IEventInfo> eventDic = new Dictionary<string, IEventInfo>();

    /// <summary>
    /// 添加事件监听
    /// </summary>
    /// <param name="name">事件的名字</param>
    /// <param name="action">准备用来处理事件的委托函数</param>
    public void AddEventListener<T>(string name, UnityAction<T> action)
    {
        //有没有对应的事件监听
        if(eventDic.ContainsKey(name))
        {
            (eventDic[name] as EventInfo<T>).actions += action;
        } else {
            eventDic.Add(name, new EventInfo<T>(action));
        }
    }

    //不需要传递参数的方法（重载）
    public void AddEventListener(string name, UnityAction action)
    {
        //有没有对应的事件监听
        if (eventDic.ContainsKey(name))
        {
            (eventDic[name] as EventInfo).actions += action;
        } else {
            eventDic.Add(name, new EventInfo(action));
        }
    }


    /// <summary>
    /// 移除对应的事件监听，对象在场景上销毁的时候应该调用，否则会造成内存泄露
    /// </summary>
    /// <param name="name">事件的名字</param>
    /// <param name="action">对应之前添加的委托函数</param>
    public void RemoveEventListener<T>(string name, UnityAction<T> action)
    {
        if (eventDic.ContainsKey(name))
        {
            (eventDic[name] as EventInfo<T>).actions -= action;
        }
    }

    //不需要参数的移出（重载）
    public void RemoveEventListener(string name, UnityAction action)
    {
        if (eventDic.ContainsKey(name))
        {
            (eventDic[name] as EventInfo).actions -= action;
        }
    }

    /// <summary>
    /// 事件触发
    /// </summary>
    /// <param name="name">哪一个名字的事件被触发了</param>
    public void EventTrigger<T>(string name, T info)
    {
        //有没有对应的事件监听
        if (eventDic.ContainsKey(name))
        {
            //eventDic[name]();
            //eventDic[name].Invoke(info);
            if ((eventDic[name] as EventInfo<T>).actions != null)
                (eventDic[name] as EventInfo<T>).actions.Invoke(info);
        }
        //没有人关心这个事件，不做处理
    }

    //不需要参数的触发（重载）
    public void EventTrigger(string name)
    {
        //有没有对应的事件监听
        if (eventDic.ContainsKey(name))
        {
            //eventDic[name]();
            //eventDic[name].Invoke(info);
            if ((eventDic[name] as EventInfo).actions != null)
                (eventDic[name] as EventInfo).actions.Invoke();
        }
        //没有人关心这个事件，不做处理
    }

    /// <summary>
    /// 清空事件中心
    /// 主要用在场景切换时（虽然每个GameObject写好OnDestroy内的逻辑已经基本能保证，但保险起见）
    /// </summary>
    public void Clear()
    {
        eventDic.Clear();
    }
}
