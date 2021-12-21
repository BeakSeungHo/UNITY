using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading;
public class LoadManager : MonoBehaviour
{
    public Animator CharacterAni;
    public RectTransform Character;
    public RectTransform BackGround;
    public RectTransform TextMessage;
    static string nextScene;
    // Start is called before the first frame update
    void Start()
    {
        BackGroundReady();
        SpriteReady();
        StartCoroutine(LoadScene());
        Thread t1 = new Thread(new ThreadStart(LoadingReinForce));
        t1.Start();
    }

    // Update is called once per frame
    void Update()
    {

    }
    void BackGroundReady()
    {
        BackGround.sizeDelta = new Vector2(Screen.width, Screen.height);
    }
    void SpriteReady()
    {
        Character.transform.localPosition = new Vector2((Screen.width * 0.5f) * 0.8f, -(Screen.height * 0.5f) * 0.8f);
        //CharacterAni.runtimeAnimatorController = SceneStarter.Instance.animatorElements.UnitAniDic[0];
        CharacterAni.SetBool("Run", true);

        TextMessage.transform.localPosition = new Vector2((Screen.width * 0.5f) * 0.8f - 185f, -(Screen.height * 0.5f) * 0.8f - 25f);
    }

    public static void LoadScene(string sceneName)
    {
        nextScene = sceneName;

        SceneManager.LoadScene("LoadingScene");
    }



    IEnumerator LoadScene()
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);

        op.allowSceneActivation = false;
        int num = 0;
        float tempTime = 0.1f;
        while (!op.isDone)
        {

            yield return new WaitForSeconds(tempTime);
            if (SceneStarter.Instance.LoadEnd && SceneStarter.Instance.LoadReinforcesEnd)
            {
                op.allowSceneActivation = true;
                yield break;
            }
            SceneStarter.Instance.Ready(num);
            num++;
        }
    }

    void LoadingReinForce()
    {
        SceneStarter.Instance.ReadyReinforce();
        return;
    }
}
