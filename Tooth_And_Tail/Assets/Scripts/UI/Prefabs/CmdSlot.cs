using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CmdSlot : MonoBehaviour
{
    public BattleReadyWindow    MasterBattleReady;  // 유닛 선택 화면 오브젝트

    public int                  cmdType;            // 커맨더 타입
    public Image                icon;               // 커맨더 아이콘
    public Image                overlay;            // 비활성 이미지


    // 유닛 선택 화면의 아이콘 클릭
    public void OnClickBattleIcon()
    {
        MasterBattleReady.OnClickCmdIcon(cmdType);
    }
}
