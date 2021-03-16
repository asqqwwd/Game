using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//单例模式基类（继承Monobehaviour，不需要拖动对象，想用直接创建子类调用GetInstance）
public class SingletonAutoMono<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    public static T GetInstance()
    {
        if (instance == null)
        {
            //注意过了场景这个obj会被删除
            GameObject obj = new GameObject();
            //让这个单例模式对象过场景不移除
            //因为单例模式对象往往是存在整个程序生命周期的
            DontDestroyOnLoad(obj);
            //设置对象名为脚本名
            obj.name = typeof(T).ToString();
            instance = obj.AddComponent<T>();
        }
        return instance;
    }

    //不再需要重写函数
    /*
    protected virtual void Awake()
    {
        instance = this as T;
    }
    */
}
