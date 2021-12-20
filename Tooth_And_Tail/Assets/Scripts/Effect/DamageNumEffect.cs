using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class DamageNumEffect : MonoBehaviour
{
    // Start is called before the first frame update
    // Start is called before the first frame update
    public int Damage;
    public float LifeTime;
    public float Speed;
    public float Power;
    float tempLifeTime;
    public TextMeshProUGUI text;
    public bool Disable = false;
    float Dir;
    public float Acc;
    void Start()
    {
    }
    private void OnEnable()
    {

        //text.transform.position = new Vector3(0, 0, 0);
        text.text = Damage.ToString();
        text.color = new Color(1, 1, 1, 1);
        Dir = 1f;
        Acc = 0f;
        tempLifeTime = 0f;
    }
    private void OnDisable()
    {
        tempLifeTime = 0f;
    }
    // Update is called once per frame
    void Update()
    {
        //tempLifeTime += Time.deltaTime;
        //if (LifeTime * 0.3f <= tempLifeTime)
        //{
        //    Dir = -1f;
        //}
        //if (LifeTime <= tempLifeTime)
        //{
        //    Disable = true;
        //}
        //else
        //{
        //    //Debug.Log(Speed * Time.deltaTime * DIr);
        //    Acc += 0.5f * Time.deltaTime;
        //    text.transform.Translate(0, Speed * ConvertGravity(Power, Acc), 0);
        //    text.color = new Color(1, 1, 1, 1 - ((tempLifeTime / LifeTime)));            
        //}

        //vPos.y += CPhysicMgr::ConvertGravity(&vPos, m_fJumpPower, m_fAcc);

    }
    public float ConvertGravity(float fPower, float acc)
    {
        const float fGravity = 9.8f;

        const float fHalf = 0.5f;

        const float fPI = 3.141592f;

        float fUpForce = fPower * Mathf.Sin(90 * fPI / 180f);
        float fDownForce = Mathf.Pow(acc, 2f) * fGravity * fHalf;

        return fUpForce - fDownForce;
    }
}
