using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Task : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        EventManager.GetInstance().AddEventListener<Monster>("MonsterDead", TaskWaitMonsterDeadDo);
    }

    public void TaskWaitMonsterDeadDo(Monster info)
    {
        Debug.Log("记录" + info.name);
    }

    private void OnDestroy()
    {
        EventManager.GetInstance().RemoveEventListener<Monster>("MonsterDead", TaskWaitMonsterDeadDo);
    }
}
