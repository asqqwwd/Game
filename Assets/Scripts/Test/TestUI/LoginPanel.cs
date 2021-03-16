using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class LoginPanel : BasePanel
{
    protected override void Awake()
    {
        //一定不能少，因为需要执行父类的awake来初始化一些信息，比如找控件，加入事件监听
        base.Awake();
        //在下面处理自己的一些初始化逻辑
    }

    void Start()
    {
        UIManager.AddCustomEventListener(GetControl<Button>("btnStart"), EventTriggerType.PointerEnter, (data) =>
        {
            Debug.Log("进入");
        });
        UIManager.AddCustomEventListener(GetControl<Button>("btnStart"), EventTriggerType.PointerExit, (data) =>
        {
            Debug.Log("离开");
        });
    }

    protected override void OnClick(string btnName)
    {
        switch (btnName)
        {
            case "btnStart":
                Debug.Log("btnStart被点击");
                break;
            case "btnQuit":
                Debug.Log("btnQuit被点击");
                break;
        }
    }

    protected override void OnValueChanged(string toggleName, bool value)
    {
        //在这来根据名字判断哪一个单选框或多选框状态变化了，当前状态就是传入的value
    }


    public void ClickStart()
    {
        Debug.Log("Game Start!");
    }

    public void ClickQuit()
    {
        Debug.Log("Game Quit!");
    }

    public void InitInfo()
    {
        Debug.Log("初始化面板数据...");
    }

    public override void ShowMe()
    {
        base.ShowMe();
        //显示面板时，想要执行的逻辑。这个函数在UI管理器中会自动帮我们调用
        //只要重写了它就会执行里面的逻辑
        /*...*/
    }
}
