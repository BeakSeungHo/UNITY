using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WildBoarFlame : MonoBehaviour
{
    public bool Testflag1;

    public ParticleSystem Flame1;
    public ParticleSystem Flame2;
    public float Flame1StartTime;
    public float Flame1Speed;
    bool Flame1Startflag;
    bool Flame2Startflag;
    float CheckTime;
    // Start is called before the first frame update
    void Start()
    {
        Flame1Startflag = false;
        Flame2Startflag = false;
        CheckTime = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        var main = Flame1.main;
        Flame1Speed = main.startSpeedMultiplier;
        main.startLifetime = Flame1StartTime;
        if (Testflag1)
        {
            FlameStart();
        }
        else
        {
            FlameStop();
        }
    }
    void FlameStart()
    {
        if (!Flame1Startflag)
        {
            Flame1.Play();
            Flame1Startflag = true;
        }
        if (!Flame2Startflag)
        {
            CheckTime += Time.deltaTime;
            //Debug.Log(CheckTime);
            if (CheckTime >= Flame1.main.startLifetimeMultiplier * 1.5f)
            {
                //Debug.Log(Flame1.main.startLifetimeMultiplier * 1.5f);
                Flame2.Play();
                Flame2Startflag = true;
                CheckTime = 0f;
            }
        }
    }
    void FlameStop()
    {
        Flame1.Stop();

        CheckTime += Time.deltaTime;
        if (CheckTime >= Flame1.main.startLifetimeMultiplier )
        {
            Flame2.Stop();
            CheckTime = 0f;
            Flame1Startflag = false;
            Flame2Startflag = false;
        }

    }
}
