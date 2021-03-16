using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAudio : MonoBehaviour
{
    GUIStyle s;
    float v = 0;
    AudioSource source;

    void OnGUI()
    {
        if (GUI.Button(new Rect(0, 0, 100, 100), "播放音乐"))
        {
            v = 0;
            AudioManager.GetInstance().ChangeBKValue(0);
            AudioManager.GetInstance().PlayBKMusic("Audio/BGM/Main Theme");
        }
            

        if (GUI.Button(new Rect(0, 100, 100, 100), "暂停音乐"))
            AudioManager.GetInstance().PauseBKMusic();

        if (GUI.Button(new Rect(0, 200, 100, 100), "停止音乐"))
            AudioManager.GetInstance().StopBKMusic();

        //v = GUI.Slider(new Rect(0, 300, 100, 50), v, 1, 1, 0, s, s1, true, 0);

        v += Time.deltaTime / 100;
        AudioManager.GetInstance().ChangeBKValue(v);

        //音效
        if (GUI.Button(new Rect(0, 300, 100, 100), "播放音效"))
            AudioManager.GetInstance().PlayAudio("Audio/Sound/egg", false, (s)=> 
            {
                source = s;
            });


        if (GUI.Button(new Rect(0, 400, 100, 100), "停止音效"))
        {
            AudioManager.GetInstance().StopAudio(source);
            source = null;
        }
        


    }
}
