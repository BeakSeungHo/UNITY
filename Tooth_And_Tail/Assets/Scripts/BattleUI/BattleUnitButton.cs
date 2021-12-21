using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleUnitButton : MonoBehaviour
{
    public BattleUICtrl MastUICtrl = null; // 배틀UI 컨트롤러

    public int   UnitIndex;                 // 유닛 인덱스
    public Image icon;                      // 유닛 아이콘
    public Image tint;                      // 유닛 틴트
    public Image ImpossibleProduct;         // 생산 불가 UI(자원부족)
    public Image GenTimUI;                  // 생산 시간 UI
    public RectTransform rectTransform;     // 현재 버튼의 RectTransform
    public TextMeshProUGUI UnitCountText;   // 유닛 숫자 체크

    Vector2 size2 = new Vector2(180, 180);  // 클릭시 변경될 크기

    public void OnClickBattleUnitButton()
    {
        MastUICtrl.ChangeUnitInfo(UnitIndex);
        rectTransform.sizeDelta = size2;
    }
    
    public void UnitCountUpdate()
    {
        var PlayCamp = GameManager.Instance.CommanderList[0];
        var UnitType = GameManager.Instance.PlayerUnitType(UnitIndex);

        switch (UnitType)
        {
            case CommonType.Wire:
            case CommonType.Mine:
            case CommonType.Turret:
            case CommonType.Balloon:
            case CommonType.Cannon:
                if (BuildingManager.Instance.Buildings[PlayCamp].ContainsKey(UnitType) && BuildingManager.Instance.Buildings[PlayCamp][UnitType].Count > 0)
                {
                    UnitCountText.gameObject.SetActive(true);
                    UnitCountText.text = BuildingManager.Instance.Buildings[PlayCamp][UnitType].Count.ToString();
                }
                else
                {
                    UnitCountText.gameObject.SetActive(false);
                    return;
                }
                break;
            default:
                if (BuildingManager.Instance.maxUnits[PlayCamp][UnitType] > 0)
                {
                    UnitCountText.gameObject.SetActive(true);
                    UnitCountText.text = SquadController.Instance.Squads[PlayCamp][UnitIndex].UnitList.Count.ToString() 
                                            + "/" + BuildingManager.Instance.maxUnits[PlayCamp][UnitType].ToString();
                }
                else if(SquadController.Instance.Squads[PlayCamp][UnitIndex].UnitList.Count > 0)
                {
                    UnitCountText.gameObject.SetActive(true);
                    UnitCountText.text = SquadController.Instance.Squads[PlayCamp][UnitIndex].UnitList.Count.ToString();
                }
                else
                {
                    UnitCountText.gameObject.SetActive(false);
                    return;
                }
                break;
        }
    }
}
