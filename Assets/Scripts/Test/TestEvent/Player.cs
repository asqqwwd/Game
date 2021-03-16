using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        EventManager.GetInstance().AddEventListener<Monster>("MonsterDead", MonsterDeadDo); //带参数的消息监听
        EventManager.GetInstance().AddEventListener("Win", Win); //不带参数的消息监听
    }

    public void MonsterDeadDo(Monster info)
    {
        Debug.Log("玩家得到奖励" + info.name);
    }

    public void Win()
    {
        Debug.Log("玩家胜利");
    }

    private void OnDestroy()
    {
        EventManager.GetInstance().RemoveEventListener<Monster>("MonsterDead", MonsterDeadDo);
    }
}
