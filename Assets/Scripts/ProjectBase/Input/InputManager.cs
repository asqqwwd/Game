using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 输入控制模块
/// 1. Input类，U3D自带
/// 2. 事件中心模块
/// 3. 公共Mono模块的使用
/// </summary>
public class InputManager : BaseManager<InputManager>
{
    private bool isStart = false;

    /// <summary>
    /// 构造函数中添加Update监听
    /// </summary>
    public InputManager()
    {
        MonoManager.GetInstance().AddUpdateListener(MyUpdate);
    }

    /// <summary>
    /// 是否开启或关闭输入检测
    /// </summary>
    public void StartOrEndCheck(bool isOpen)
    {
        isStart = isOpen;
    }

    private void MyUpdate()
    {
        //没有开启输入检测就直接return
        if (!isStart)
            return;
        CheckKeyCode(KeyCode.W);
        CheckKeyCode(KeyCode.A);
        CheckKeyCode(KeyCode.S);
        CheckKeyCode(KeyCode.D);
    }

    /// <summary>
    /// 用来检测按键抬起按下分发事件的
    /// </summary>
    /// <param name="key"></param>
    private void CheckKeyCode(KeyCode key)
    {
        //事件中心模块，分发按下抬起事件
        if (Input.GetKeyDown(key))
            EventManager.GetInstance().EventTrigger("某键按下", key);

        //事件中心模块，分发按下抬起事件
        if (Input.GetKeyUp(key))
            EventManager.GetInstance().EventTrigger("某键抬起", key);
    }
}
