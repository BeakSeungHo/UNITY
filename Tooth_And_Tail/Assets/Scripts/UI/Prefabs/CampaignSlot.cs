using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class CampaignSlot : MonoBehaviour
{
    public CampaignWindow   MasterCampaign;

    public Camp             chapter;
    public bool             isUnlocked;
    public Image            portraitLock;
    public TextMeshProUGUI  lockText;
    public TextMeshProUGUI  progressText;

    public int              progress;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UnlockChapter()
    {
        isUnlocked = true;
        portraitLock.gameObject.SetActive(false);
        lockText.gameObject.SetActive(false);

        progress = 0;
        progressText.gameObject.SetActive(true);
        progressText.text = "진행도  " + progress.ToString() + " / 5";
    }

    public void OnClickChapter()
    {
        if (isUnlocked)
            MasterCampaign.chapterInfo.gameObject.SetActive(true);
        else
            Debug.Log("잠겨진 챕터입니다");
    }
}
