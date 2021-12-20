using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class PlusFoodEffect : MonoBehaviour
{
    // Start is called before the first frame update
    public int PlusFood;
    public float LifeTime;
    public float Speed;
    public float tempLifeTime;
    public bool Disable = false;
    public TextMeshProUGUI text;
    void Start()
    {
    }
    public void Ready()
    {
        string temp = "+" + PlusFood.ToString();
        text.text = temp;
        text.color = new Color(1, 1, 1, 1);

        tempLifeTime = 0f;
    }
    private void OnEnable()
    {
        //text.transform.position = new Vector3(0,0,0);
        //string temp = "+" + PlusFood.ToString();
        //text.text = temp;
        //text.color = new Color(1, 1, 1, 1);

        //tempLifeTime = 0f;
    }
    private void OnDisable()
    {
        tempLifeTime = 0f;
    }
    // Update is called once per frame
    void Update()
    {
        //Debug.Log(tempLifeTime);
        //tempLifeTime += Time.deltaTime;
        //if (LifeTime <= tempLifeTime)
        //{
        //    Disable = true;
        //}
        //else
        //{
        //    text.transform.Translate(0, Speed * Time.deltaTime, 0);
        //    text.color = new Color(1, 1, 1, 1 - ((tempLifeTime / LifeTime)));
        //}
    }
}
