using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//单例模式基类（继承Monobehaviour，需要自己保证唯一性 -> 不要拖给多个对象）
//C#泛型
//设计模式 单例
public class SingletonMono<T> : MonoBehaviour where T : MonoBehaviour //加上泛型约束
{
    private static T instance;
    public static T GetInstance()
    {
        //继承了Mono的脚本不能够直接new
        //只能通过拖动到对象上或者通过加脚本的api AddComponent加脚本
        //U3D内部帮助实例化
        return instance;
    }

    protected virtual void Awake() //声明为protected virtual是为了方便子类重载，子类的Awake函数开头要加上一句：base.Awake()
    {
        instance = this as T; //里式转换原则
    }

}
