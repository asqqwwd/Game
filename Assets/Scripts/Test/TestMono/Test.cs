using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTest
{
    public TestTest()
    {
        MonoManager.GetInstance().StartCoroutine(Test123());
    }

    public void Update() //测试Mono Update
    {
        Debug.Log("TestTest");
    }

    IEnumerator Test123() //测试协程
    {
        yield return new WaitForSeconds(1f);
        Debug.Log("123123123");
    }
}

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        TestTest t = new TestTest();
        MonoManager.GetInstance().AddUpdateListener(t.Update);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
