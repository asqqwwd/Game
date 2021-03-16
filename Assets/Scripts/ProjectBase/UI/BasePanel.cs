using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 面板基类
/// 帮助我们通过代码快速找到所有的子控件
/// 方便我们在子类中处理逻辑
/// 节约找控件的工作量
/// </summary>
public class BasePanel : MonoBehaviour
{
    //如果找到基类所有控件，那么需要一个字典去存储它
    //通过里式转换原则，来存储所有的控件（UIBehaviour是所有UI控件的基类）
     private Dictionary<string, List<UIBehaviour>> controlDic = new Dictionary<string, List<UIBehaviour>>();

    // Start is called before the first frame update
    protected virtual void Awake()
    {
        FindChildrenControl<Button>();
        FindChildrenControl<Toggle>();

        FindChildrenControl<Image>();
        FindChildrenControl<Text>();

        FindChildrenControl<ScrollRect>();
        FindChildrenControl<Slider>();
        FindChildrenControl<InputField>();
        /*...*/
    }
    
    /// <summary>
    /// 找到子对象的对应控件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    private void FindChildrenControl<T>() where T : UIBehaviour
    {
        T[] controls = this.GetComponentsInChildren<T>();
        for (int i = 0; i < controls.Length; i++)
        {
            string objName = controls[i].gameObject.name;
            if (controlDic.ContainsKey(objName))
                controlDic[objName].Add(controls[i]);
            else
                controlDic.Add(objName, new List<UIBehaviour>() { controls[i] });

            if(controls[i] is Button)
            {
                (controls[i] as Button).onClick.AddListener( () =>
                {//用无参的lambda函数包裹住原来的有参委托函数
                    OnClick(objName);
                });
            }
            //如果是单选框或者多选框
            else if(controls[i] is Toggle)
            {
                (controls[i] as Toggle).onValueChanged.AddListener((value) =>
                {
                    OnValueChanged(objName, value);
                });
            }

        }
    }

    /// <summary>
    /// 得到对应名字的对应控件脚本
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="controlName"></param>
    /// <returns></returns>
    // Unity不会允许出现一个gameobject下挂两个同类型的UIBehaviour
    protected T GetControl<T>(string controlName) where T: UIBehaviour
    {
        if (controlDic.ContainsKey(controlName))
        {
            for(int i=0; i<controlDic[controlName].Count; i++)
            {
                if (controlDic[controlName][i] is T)
                    return controlDic[controlName][i] as T;
            }
        }
        
        return null;
    }

    protected virtual void OnClick(string btnName)
    {

    }

    protected virtual void OnValueChanged(string toggleName, bool value)
    {

    }


    /// <summary>
    /// 显示该UI组件时要做的事
    /// </summary>
    public virtual void ShowMe()
    {
        
    }

    /// <summary>
    /// 隐藏该UI组件时要做的事
    /// </summary>
    public virtual void HideMe()
    {

    }

}
