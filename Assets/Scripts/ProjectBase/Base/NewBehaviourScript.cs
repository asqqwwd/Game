using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    private static NewBehaviourScript instance;
    public static NewBehaviourScript GetInstance()
    {
        //继承了Mono的脚本不能够直接new
        //只能通过拖动到对象上或者通过加脚本的api: AddComponent加脚本
        //U3D内部帮助实例化
        return instance;
    }

    // Start is called before the first frame update
    void Awake()
    {
        //脚本是挂载到一个gameobject上的，因此在这里实例化
        //但注意如果脚本被挂载到多个gameobject上，instance将会等于最后一个*this*（单例模式实际已被破坏）
        instance = this;
    }
}
