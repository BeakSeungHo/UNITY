using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    public ParticleSystem[] particleSystem;
    public bool testFlag = false;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (testFlag)
        {
            ParticlePlay();
            testFlag = false;
        }
    }
    public void ParticleRestart()
    {

        for (int i = 0; i < particleSystem.Length; i++)
        {
            particleSystem[i].Stop();
            particleSystem[i].Play();
        }

    }
    public void ParticleStop()
    {

        for (int i = 0; i < particleSystem.Length; i++)
        {
            particleSystem[i].Stop();
        }

    }
    public void ParticlePlay()
    {
        for (int i = 0; i < particleSystem.Length; i++)
        {
            particleSystem[i].Play();
        }
    }
}
