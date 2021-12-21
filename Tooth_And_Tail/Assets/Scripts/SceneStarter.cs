using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

//////////////////////////////////////////////////////////
////
////
////	Name : sbk
////	Date : 2020.08.13.
////	
////	Desc.
////		게임의 시작을 담당하는 Scene을 실행시키는 메인 스크립트.
////		게임을 시작 시키는 스크립트이자 각종 인게임 외적 설정 값들을
////		정의하고 관리하는 스크립트.
////
////
////		작업자들이 알아야할 사항
////		1. 사용하는 리소스는 모두 리소시스 폴더 안에 있어야한다!!
////				- 작업이 필요한 리소스는 작업한 후에는 전부 리소시스 폴더에 넣어서 관리해주기 바람..
////				  리소시스 폴더 안에서 하위 폴더 생성해서 관리
////		2. UI 쪽은 UI 레이어 꼭 설정하자. 오브젝트 Tag 옆에 있음..
////
//////////////////////////////////////////////////////////////



public class SceneStarter : MonoBehaviour
{

    //
    //
    ////
    ////

    private static SceneStarter instance = null;

    // 각 세팅 값들은 여기서 정의해줄 수도 있고
    // 오브젝트에서 정의할 수 있다.
    // 보통 오브젝트에 박아서 수정이 편하게 설정한다.
    public CommonElements commonElements = null;
    public UserElements userElements = null;
    public AnimatorElements animatorElements = null;
    public GameElements gameElements = null;
    public TintElements tintElements = null;
    public UIElements uIElements = null;
    public ReinforceElements reinforceElements = null;
    public StatisticElements statisticElements = null;
    public SoundElements soundElements = null;
    ////
    ////
    //
    //
    bool loadEnd = false;
    bool loadReinforcesEnd = false;
    public bool LoadEnd { get { return loadEnd; } }
    public bool LoadReinforcesEnd { get { return loadReinforcesEnd; } }


    // 초기화 부분에서 주로 사용하는 MonoBehaviour 함수들..

    // Awake 함수는 해당 오브젝트의 Active가 On 될 때 1회 실행된다.
    // Start 함수는 오브젝트에 붙어 있는 해당 스크립트가 enable 될때 1회 실행된다.
    // OnEnable 함수는 스크립트가 Enable 될 때마다 실행된다.
    // 함수 실행 순서는 Awake - OnEnable - Start 라고 보면 된다.
    // Awake와 Start가 주로 많이 쓰이며 보통은 오브젝트를 기준으로 관리하기 때문에 Awake를 쓰는데
    // 스크립트가 2개 박혀있다거나 오브젝트에 박혀있는 다른 컴포넌트들의 세팅을 필요로 한다면 무조건 Start를 써야한다!!


    void Awake()
	{
        // 로그를 찍어뒀으니 콘솔창에서 순서를 확인할 수 있음
        // 스크립트랑 오브젝트를 꺼보면서 확인해보자.
        if (null != Instance)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        // 필요한 설정 값들의 설정을 해준다.
        //userElements.InitializeElement();
        //animatorElements.InitializeElement();
        //LoadManager.LoadScene("Lobby");
        //gameElements.InitializeElement();
        //tintElements.InitializeElement();
        //uIElements.InitializeElement();
        //reinforceElements.InitializeElement();
        //statisticElements.InitializeElement();
        //soundElements.InitializeElement();
        //loadEnd = true;
        //
        // 각종 세팅값을 혹시 변경할 경우도 있을 수 있으므로 혹시 몰라서 객체 유지 시킴..
        // 만약 세팅 후 값 변경 등을 막거나 해줄 필요가 없다면 유지시킬 필요가 없다.

        DontDestroyOnLoad(gameObject);
	}
    enum InitializeElement
    {
        Animator, User, Game, Tint_Build, Tint_Commander, Tint_UI, Tint_Unit, UI, Statistic, Sound
    }
    public void Ready(int num)
    {
        switch ((InitializeElement)num)
        {
            case InitializeElement.Animator:
                animatorElements.InitializeElement();
                break;
            case InitializeElement.User:
                userElements.InitializeElement();
                break;
            case InitializeElement.Game:
                gameElements.InitializeElement();
                break;
            case InitializeElement.Tint_Build:
                tintElements.InitializeElement_Build();
                break;
            case InitializeElement.Tint_Commander:
                tintElements.InitializeElement_Commander();
                break;
            case InitializeElement.Tint_UI:
                tintElements.InitializeElement_UI();
                break;
            case InitializeElement.Tint_Unit:
                tintElements.InitializeElement_Unit();
                break;
            case InitializeElement.UI:
                uIElements.InitializeElement();
                break;
            case InitializeElement.Statistic:
                statisticElements.InitializeElement();
                break;
            default:
                soundElements.InitializeElement((num - (int)InitializeElement.Sound), ref loadEnd);
                break;
        }
    }
    void OnDisable()
    {
        userElements.Save();
    }
    
    public void OnClick()
	{
        // Start 버튼을 누르면 씬을 넘어가기 위한 버튼 함수..
        //SceneManager.LoadScene("Lobby");
        LoadManager.LoadScene("Lobby");
    }

    public CommonData GetData(CommonType type)
    {
        return commonElements.CommonDataList[(int)type];
    }
    public static SceneStarter Instance
    {
        get
        {
            if (null == instance)
                return null;
            else
                return instance;
        }
    }
    public void ReadyReinforce()
    {
        reinforceElements.InitializeElement();
        loadReinforcesEnd = true;
    }
}
