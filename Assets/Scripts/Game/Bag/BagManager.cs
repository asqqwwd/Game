using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item
{
    public int id;
    public int num;

}


/// <summary>
/// 背包管理器，主要管理背包的一些公共数据和公共方法
/// </summary>
public class BagManager : BaseManager<BagManager>
{
    public List<Item> items = new List<Item>();

    /// <summary>
    /// 这是模拟获取数据的方法
    /// 实际开发应从服务器或从本地文件中读取出来的
    /// </summary>
    public void InitItemsInfo()
    {
        for( int i = 0; i < 100000; i++)
        {
            Item item = new Item();
            item.id = i;
            item.num = i;

            items.Add(item);
        }
    }
}
