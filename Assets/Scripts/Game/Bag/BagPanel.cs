using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BagPanel : BasePanel
{
    //履带对象 需要通过它得到可视范围位置，还要把动态创建的格子设置为它的子对象
    public RectTransform content;
    //可视范围高度
    public int viewPortH;

    private Dictionary<int, GameObject> nowShowItems = new Dictionary<int, GameObject>(); //当前显示的格子对象
    private int oldMinIndex = -1;
    private int oldMaxIndex = -1;

    protected override void Awake()
    {
        base.Awake();
        //content = (GetControl<ScrollRect>("Content").gameObject.transform as RectTransform);
        viewPortH = 925;
    }

    // Update is called once per frame
    void Update()
    {
        //显示面板时，更新格子信息
        CheckShowOrHide(); //
    }

    public override void ShowMe()
    {
        base.ShowMe();
        //应该要初始化履带的长度
        content.sizeDelta = new Vector2(0, Mathf.CeilToInt(BagManager.GetInstance().items.Count / 3f) * 190); //向上取整
        
        CheckShowOrHide();
    }

    /// <summary>
    /// 更新格子显示的方法
    /// </summary>
    void CheckShowOrHide()
    {
        if (content.anchoredPosition.y < 0) 
            return;

        //检测哪些格子应该显示出来（格子高度180+空隙10）
        int minIndex = (int)(content.anchoredPosition.y / 190) * 3;
        int maxIndex = (int)((content.anchoredPosition.y + viewPortH) / 190) * 3 + 2; //我想要一排显示3个，因此最后一个索引要+2
        if (maxIndex >= BagManager.GetInstance().items.Count)
            maxIndex = BagManager.GetInstance().items.Count - 1;

        //根据上一次索引和这一次新索引判断哪些该被移除
        //删除上半截溢出的物体
        for (int i = oldMinIndex; i < minIndex; i++)
        {
            if (nowShowItems.ContainsKey(i))
            {
                if (nowShowItems[i] != null)
                    PoolManager.GetInstance().PushObj("UI/BagItem", nowShowItems[i]);

                //如果异步加载比较慢，但是拖屏很快，字典里已经创建了键但还没有放进对象
                nowShowItems.Remove(i);
            }
        }

        //删除下半截溢出的物体
        for (int i = maxIndex + 1; i <= oldMaxIndex; i++)
        {
            if (nowShowItems.ContainsKey(i))
            {
                if (nowShowItems[i] != null)
                    PoolManager.GetInstance().PushObj("UI/BagItem", nowShowItems[i]);

                //如果异步加载比较慢，但是拖屏很快，字典里已经创建了键但还没有放进对象
                nowShowItems.Remove(i);
            }
        }

        //更新缓存索引
        oldMinIndex = minIndex;
        oldMaxIndex = maxIndex;

        for (int i = minIndex; i <= maxIndex; i++)
        {
            if (nowShowItems.ContainsKey(i))
                continue;

            int index = i;
            nowShowItems.Add(index, null); //因为异步加载 先占坑
            PoolManager.GetInstance().GetObj("UI/BagItem", (obj) =>
            {
                //当格子创建出来后我们要做什么
                //设置它的父对象
                obj.transform.SetParent(content);
                //重置相对缩放大小
                obj.transform.localScale = Vector3.one;
                //重置位置 -> 注意异步加载，用i会出问题
                obj.transform.localPosition = new Vector3((index % 3) * 240, -index / 3 * 190); //y坐标往下走的负的
                //更新格子信息
                obj.GetComponent<BagItem>().InitItemInfo(BagManager.GetInstance().items[index]);
                //判断有无这个坑
                if (nowShowItems.ContainsKey(index))
                    nowShowItems[index] = obj;
                //如果没有但是创建出来了对象，应该放回缓存池
                else
                    PoolManager.GetInstance().PushObj("UI/BagItem", obj);
            });
        }
    }
}
