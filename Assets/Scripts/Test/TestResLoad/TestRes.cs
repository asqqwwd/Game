using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRes : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GameObject obj = ResManager.GetInstance().Load<GameObject>("Test/Cube");
            obj.transform.localScale = Vector3.one * 2;
        }
        if (Input.GetMouseButtonDown(1))
        {
            //异步加载出真正的对象至少要等1帧，所以不能马上得到返回值，因此需要一个协程
            ResManager.GetInstance().LoadAysnc<GameObject>("Test/Sphere", (obj) => { //lambda表达式
                //做一些资源真正加载出来后想做的事情
                obj.transform.localScale = Vector3.one * 2;
            });
        }
    }

    //private void DoSth(GameObject obj)
    //{
        //做一些资源真正加载出来后想做的事情
    //    obj.transform.localScale = Vector3.one * 2;
    //}
}
