﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 格子类对象，他是放在背包对象里的一个一个道具格子
/// 主要用来显示单组道具信息的
/// </summary>
public class BagItem : BasePanel
{
    /// <summary>
    /// 这个方法是用于初始化道具格子信息
    /// </summary>
    public void InitItemInfo(Item info)
    {
        //先读取道具表
        //根据表中数据来更新信息
        //更新图标
        //更新名字

        //更新道具数量 -> 这里仅获得它的txtNum并修改数值
        GetControl<Text>("txtNum").text = info.num.ToString();

    }
}
