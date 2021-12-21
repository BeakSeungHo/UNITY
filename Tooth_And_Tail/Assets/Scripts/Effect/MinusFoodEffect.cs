using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class MinusFoodEffect : MonoBehaviour
{
    // Start is called before the first frame update
    public int MinusFood;
    public float LifeTime;
    public float Speed;
    float tempLifeTime;
    public TextMeshProUGUI text;
    public bool Disable = false;
    void Start()
    {
    }

    public void Ready()
    {
        string temp = "-" + MinusFood.ToString();
        text.text = temp;
        text.color = new Color(1, 1, 1, 1);

        tempLifeTime = 0f;
    }
    private void OnEnable()
    {
        //text = transform.GetChild(0).transform.GetChild(0).GetComponent<Text>();
        //text.transform.position = gameObject.transform.parent.position;
        //string temp = "-" + MinusFood.ToString();
        //text.text = temp;
        //text.color = new Color(1,1,1,1);

        //tempLifeTime = 0f;
    }
    private void OnDisable()
    {
        tempLifeTime = 0f;
    }
    // Update is called once per frame
    void Update()
    {
        //tempLifeTime += Time.deltaTime;
        //if (LifeTime <= tempLifeTime)
        //{
        //    Disable = true;
        //}
        //else
        //{
        //    text.transform.Translate(0, Speed * Time.deltaTime, 0);
        //    text.color = new Color(1, 1, 1, 1 -( (tempLifeTime / LifeTime)));
          
        //}
    }
}
