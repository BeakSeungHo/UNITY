using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPCanvas : MonoBehaviour
{
    public CommonBase Base = null;
    public Character character = null;
    public HPBar hpbar = null;
    public Status status = null;
    public CanvasGroup Group = null;

    float CheckTime = 0f;

    public DamageFontObject DamageFont;
    void Awake()
    {
        Base = transform.parent.parent.GetComponent<CommonBase>();
        character = transform.parent.parent.GetComponent<Character>();

        gameObject.transform.localScale = new Vector2(1 / (1080f / 2.5f), 1 / (1080f / 2.5f));

        SetPosition();
    }

    public void SetActiveUI(bool flag)
    {
        hpbar.gameObject.SetActive(flag);
        DamageFont.text.enabled = flag;
    }

    public void Ready()
    {
        if (Base == null)
            return;
        hpbar.Ready();
        status.Ready();
        SetPosition();
        DamageFont.Ready((int)Base.Type);
    }

    void SetPosition()
    {
        switch (Base.Type)
        {
            case CommonType.Commander:
                gameObject.transform.localPosition = new Vector3(0, 0.3f, 0);
                gameObject.transform.localScale = gameObject.transform.localScale * 1.5f;
                break;
            case CommonType.Squirrel:
            case CommonType.Lizard:
            case CommonType.Toad:
            case CommonType.Mole:
            case CommonType.Fox:
                gameObject.transform.localPosition = new Vector3(0, 0.05f, 0);
                break;
            case CommonType.Ferret:
            case CommonType.Falcon:
            case CommonType.Boar:
                gameObject.transform.localPosition = new Vector3(0, 0.2f, 0);
                break;
            case CommonType.Skunk:
            case CommonType.Snake:
                gameObject.transform.localPosition = new Vector3(0, 0.1f, 0);
                break;
            case CommonType.Pigeon:
                gameObject.transform.localPosition = new Vector3(0, 0.15f, 0);
                break;
            case CommonType.Balloon:
                gameObject.transform.localPosition = new Vector3(0, 0.4f, 0);
                break;
            case CommonType.Owl:
                gameObject.transform.localPosition = new Vector3(0, 0.3f, 0);
                break;
            case CommonType.Badger:
            case CommonType.Wolf:
                gameObject.transform.localPosition = new Vector3(0, 0.25f, 0);
                break;
            case CommonType.Gristmill:
                gameObject.transform.localPosition = new Vector3(0, 0.15f, 0);
                break;
            case CommonType.Farm:
            case CommonType.Chameleon:
                gameObject.transform.localPosition = new Vector3(0, 0, 0);
                break;
            case CommonType.Cabin:
            case CommonType.CampFire:
                gameObject.transform.localPosition = new Vector3(0, 0.45f, 0);
                break;
            case CommonType.WarrenT1:
            case CommonType.WarrenT2:
            case CommonType.WarrenT3:
                gameObject.transform.localPosition = new Vector3(0, 0.5f, 0);
                gameObject.transform.localScale = gameObject.transform.localScale * 0.8f;
                break;
            case CommonType.MoleeMerge:
                gameObject.transform.localPosition = new Vector3(0, 0.4f, 0);
                gameObject.transform.localScale = gameObject.transform.localScale * 0.8f;
                break;
            case CommonType.Turret:
            case CommonType.Wire:
            case CommonType.Cannon:
                gameObject.transform.localPosition = new Vector3(0, 0.35f, 0);
                break;
            case CommonType.Mine:
                gameObject.transform.localPosition = new Vector3(0, 0.2f, 0);
                break;
        }
    }

    private void OnEnable()
    {
        Group.alpha = 0f;
    }

    void Update()
    {
        CheckDisapper();
    }

    void CheckDisapper()
    {
        if (character.HP >= Base.GetData(Base.Type).MaxHp)
        {
            if (Base.MyCamp == GameManager.Instance.CommanderList[0] &&
                (Base.Type == CommonType.WarrenT1 || Base.Type == CommonType.WarrenT2 || 
                Base.Type == CommonType.WarrenT3 || Base.Type == CommonType.MoleeMerge))
                return;

            CheckTime += Time.deltaTime;
            if (CheckTime > 0.25f)
            {
                CheckTime = 0f;
                Group.alpha -= 0.25f;

                if (Group.alpha < 0)
                    Group.alpha = 0;
            }
        }
        else if (character.HP <= 0.01 || (character.Base.MyCamp == Camp.End &&
                                          character.Base.Type != CommonType.Cabin))
            Group.alpha = 0f;
        else
            Group.alpha = 1f;
    }
}
