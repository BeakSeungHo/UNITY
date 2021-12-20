using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class LevelUpPopUp : MonoBehaviour
{
    public TextMeshProUGUI      level;
    public TextMeshProUGUI      maxBf;
    public TextMeshProUGUI      maxAf;
    public TextMeshProUGUI      curBf;
    public TextMeshProUGUI      curAf;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        level.text = SceneStarter.Instance.userElements.UserData.UserLevel.ToString();

        maxBf.text = (SceneStarter.Instance.userElements.UserData.UserMaxStamina - 5).ToString();
        maxAf.text = SceneStarter.Instance.userElements.UserData.UserMaxStamina.ToString();

        curBf.text = (SceneStarter.Instance.userElements.UserData.UserCurStamina
                        - SceneStarter.Instance.userElements.UserData.UserMaxStamina).ToString();
        curAf.text = SceneStarter.Instance.userElements.UserData.UserCurStamina.ToString();
    }

    // 레벨 업 화면 닫기
    public void OnClickConfirmBtn()
    {
        this.gameObject.SetActive(false);
    }
}
