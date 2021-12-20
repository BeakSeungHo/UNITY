using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectCampColor : MonoBehaviour
{
    // Start is called before the first frame update
    public ParticleSystem particleSystem;
    //public Camp Camp;
    void Start()
    {
        //particleSystem = gameObject.GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void ColorChange(Camp Camp, float alpha)
    {
        var main = particleSystem.main;
        switch (Camp)
        {
            case Camp.Hopper:
                main.startColor = Global.CommanderInGameColorHopper;
                break;
            case Camp.Quartermaster:
                main.startColor = Global.CommanderInGameColorQuartermaster;
                break;
            case Camp.Bellafide:
                main.startColor = Global.CommanderInGameColorBellafide;
                break;
            case Camp.Archimedes:
                main.startColor = Global.CommanderInGameColorArchimedes;
                break;
        }
    }
}
