using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestInput : MonoBehaviour
{
    
    void OnEnable()
    {
        //开启输入检测
        InputManager.GetInstance().StartOrEndCheck(true);
        EventManager.GetInstance().AddEventListener<KeyCode>("某键按下", CheckInputDown);
        EventManager.GetInstance().AddEventListener<KeyCode>("某键抬起", CheckInputUp);
    }

    //一定要记得在销毁对象时撤销监听
    void OnDisable()
    {
        //关闭输入检测
        InputManager.GetInstance().StartOrEndCheck(false);
        EventManager.GetInstance().RemoveEventListener<KeyCode>("某键按下", CheckInputDown);
        EventManager.GetInstance().RemoveEventListener<KeyCode>("某键抬起", CheckInputUp);
    }


    private void CheckInputDown(KeyCode key)
    {
        switch (key)
        {
            case KeyCode.W:
                /*你要做的事*/
                break;
            default:
                /*...*/
                break;
        }
        Debug.Log("123 Input key: "+ key.ToString() + " down.");
    }

    private void CheckInputUp(KeyCode key)
    {
        switch (key)
        {
            case KeyCode.W:
                /*你要做的事*/
                break;
            default:
                /*...*/
                break;
        }
        Debug.Log("456 Input key: " + key.ToString() + " up.");
    }

}
