using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 缓存池模块 PoolData可以认为是一类物体的Pool，而PoolMgr是所有Pools的管理者
/// 1. Dictionary List
/// 2. GameObject和Resources两个公共类中的API
/// </summary>
public class PoolData
{
    //缓存池中 对象挂载的父节点
    public GameObject fatherObj;
    //对象的容器
    public List<GameObject> poolList;

    public PoolData(GameObject obj, GameObject poolObj)
    {
        //给缓存池创建一个父对象，并且把它作为我们pool对象的子物体
        fatherObj = new GameObject(obj.name);
        fatherObj.transform.parent = poolObj.transform;
        poolList = new List<GameObject>() {  };
    }

    /// <summary>
    /// 往缓存池压东西
    /// </summary>
    /// <param name="obj"></param>
    public void PushObj(GameObject obj)
    {
        //失活
        obj.SetActive(false);
        //存起来
        poolList.Add(obj);
        //设置父对象
        obj.transform.SetParent(fatherObj.transform);
        //obj.transform.parent = fatherObj.transform;
    }

    /// <summary>
    /// 从抽屉里取出东西
    /// </summary>
    public GameObject GetObj()
    {
        GameObject obj = null;
        //取出第一个
        obj = poolList[0];
        poolList.RemoveAt(0);
        //激活让其显示
        obj.SetActive(true);
        //断开父子关系
        obj.transform.SetParent(null);
        /* Warning:
         * Parent of RectTransform is being set with parent property. 
         * Consider using the SetParent method instead, with the worldPositionStays argument set to false.
         * This will retain local orientation and scale rather than world orientation and scale, which can prevent common UI scaling issues.
         * UnityEngine.Transform:set_parent(Transform)
         */
        //obj.transform.parent = null;

        return obj;
    }
}

//缓存池可以减少GC次数，可以认为是在用部分内存换取CPU效率
public class PoolManager : BaseManager<PoolManager>
{
    //缓存池容器
    public Dictionary<string, PoolData> poolDic = new Dictionary<string, PoolData>();

    private GameObject poolObj;
    //从池子拿东西的方法
    public void GetObj(string name, UnityAction<GameObject> callBack)
    {
        //GameObject obj = null;
        if(poolDic.ContainsKey(name) && poolDic[name].poolList.Count > 0) //有这个缓存池 && 池子里有东西
        {
            //obj = poolDic[name].GetObj();
            callBack(poolDic[name].GetObj());
        } else {
            //通过异步加载资源创建对象给外部用
            ResManager.GetInstance().LoadAysnc<GameObject>(name, (o) =>
            {
                o.name = name;
                callBack(o);
            });
            //obj = GameObject.Instantiate(Resources.Load<GameObject>(name));
            //把对象名字改成和池子名
            //obj.name = name;
        }
        //return obj;
    }

    //把东西放回池子
    public void PushObj(string name, GameObject obj)
    {
        if (poolObj == null)
            poolObj = new GameObject("Pool");

        //第一次放东西需要创建缓存池
        if (!poolDic.ContainsKey(name))
            poolDic.Add(name, new PoolData(obj, poolObj));

        poolDic[name].PushObj(obj);
    }

    //清空缓存池的方法（切场景时用）
    public void Clear()
    {
        poolDic.Clear();
        poolObj = null;
    }
}
