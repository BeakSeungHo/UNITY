using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ExtendBattleUICtrl : MonoBehaviour
{
    public enum ExtendUI { BuildUI, SellUI }

    public TextMeshProUGUI Text;
    public BuildingBase    buildingBase;
    public ExtendUI        ExtendUIType;

    private void Start()
    {
        gameObject.transform.localPosition = new Vector3(0, 0.25f, 0);

        gameObject.transform.localScale = new Vector2(1 / (1080f / 2.5f), 1 / (1080f / 2.5f));

        if (ExtendUIType == ExtendUI.SellUI)
            SetPositionScale();
    }
    
    public void SetPositionScale()
    {
        switch (buildingBase.Base.Type)
        {
            case CommonType.Farm:
                gameObject.transform.localPosition = new Vector3(0, 0.4f, 0);
                gameObject.transform.localScale = gameObject.transform.localScale * 0.5f;
                break;
            case CommonType.Gristmill:
                gameObject.transform.localPosition = new Vector3(0, 0.5f, 0);
                gameObject.transform.localScale = gameObject.transform.localScale * 0.5f;
                break;
            case CommonType.WarrenT1:
            case CommonType.WarrenT2:
            case CommonType.WarrenT3:
                gameObject.transform.localPosition = new Vector3(0, 0.1f, 0);
                gameObject.transform.localScale = gameObject.transform.localScale * 0.5f;
                break;
            case CommonType.MoleeMerge:
            case CommonType.Wire:
            case CommonType.Mine:
            case CommonType.Turret:
            case CommonType.Balloon:
            case CommonType.Cannon:
                gameObject.transform.localPosition = new Vector3(0, 0, 0);
                gameObject.transform.localScale = gameObject.transform.localScale * 0.5f;
                break;
        }
    }
    
    public void ChangeLocalPosition(Vector3 newPosition)
    {
        gameObject.transform.localPosition = newPosition;
    }

    public void ChangeLocalScale(Vector2 newScale)
    {
        gameObject.transform.localScale = newScale;
    }

    public void ChangeExtendUIText(string NewText)
    {
        Text.text = NewText;
    }

    ////판매 UI Text 변경함수
    //public void ChangeSellUIText()
    //{
    //    switch (buildingBase.Base.Type)
    //    {
    //        case CommonType.WarrenT1:
    //        case CommonType.WarrenT2:
    //        case CommonType.WarrenT3:
    //        case CommonType.MoleeMerge:
    //            var data = SceneStarter.Instance.commonElements.CommonDataList[(int)buildingBase.Product_Type];
    //            switch (buildingBase.GetCurState())
    //            {
    //                case BuildingState.Idle:
    //                case BuildingState.Attack:
    //                case BuildingState.Construct:
    //                case BuildingState.Production:
    //                    Text.text = data.Animal + "\n" + "판매" + "\n" + "<color=#00ff00>" + "(" + data.BuildCost + ")" + "</color>";
    //                    break;
    //            }
    //            break;
    //        case CommonType.CampFire:
    //            data = SceneStarter.Instance.commonElements.CommonDataList[(int)buildingBase.Base.Type];
    //            switch (buildingBase.GetCurState())
    //            {
    //                case BuildingState.Idle:
    //                case BuildingState.Attack:
    //                case BuildingState.Construct:
    //                    Text.text = data.Name + "\n" + "판매" + "\n" + "<color=#00ff00>" + "(" + data.BuildCost + ")" + "</color>";
    //                    break;
    //            }
    //            break;
    //        default:
    //            data = SceneStarter.Instance.commonElements.CommonDataList[(int)buildingBase.Base.Type];
    //            switch (buildingBase.GetCurState())
    //            {
    //                case BuildingState.Idle:
    //                case BuildingState.Attack:
    //                case BuildingState.Construct:
    //                case BuildingState.Production:
    //                    Text.text = data.Name + "\n" + "판매" + "\n" + "<color=#00ff00>" + "(" + data.BuildCost + ")" + "</color>";
    //                    break;
    //            }
    //            break;
    //    }
    //}

    ////판매 UI Text 변경함수(Farm)
    //public void ChangeSellUIText(Farm.FarmState farmState)
    //{
    //    switch (farmState)
    //    {
    //        case Farm.FarmState.Idle:
    //        case Farm.FarmState.Cultivation:
    //            Text.text = "농장\n취소\n" + "<color=#00ff00>" + "(60)" + "</color>";
    //            break;
    //    }
    //}

    //빌드 UI Text 변경함수
    public void ChangeBuildUIText(CommonType BaseType)
    {
        var Camp = GameManager.Instance.CommanderList[0];
        
        switch(BaseType)
        {
            case CommonType.Gristmill:
                if (GameManager.Instance.CampFoodDic[Camp] > 60)
                    Text.text = "제분소 건설" + "<color=#00ff00>" + "(60)" + "</color>";
                break;
            case CommonType.Farm:
                if (GameManager.Instance.CampFoodDic[Camp] < 60)
                    Text.text = "농장 예약" + "<color=#ff0000>" + "(60)" + "</color>";
                else
                    Text.text = "농장 건설" + "<color=#00ff00>" + "(60)" + "</color>";
                break;
            case CommonType.CampFire:
                if (GameManager.Instance.CampFoodDic[Camp] > 60)
                    Text.text = "캠프파이어\n건설" + "<color=#00ff00>" + "(60)" + "</color>";
                break;
        }
    }

    public void OnClickButtonSell()
    {
        buildingBase.Sell();
    }
    
}
