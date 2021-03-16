using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    public int type = 1;
    public string name = "123";
    // Start is called before the first frame update
    void Start()
    {
        Dead();
    }

    void Dead()
    {
        Debug.Log("怪物死亡");

        //触发事件
        EventManager.GetInstance().EventTrigger("MonsterDead", this);
    }
}
