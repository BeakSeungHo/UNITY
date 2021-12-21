using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shadow : MonoBehaviour
{
    public SpriteRenderer SpriteRenderer;
    public CommonBase CommonBase;
    public Character Character;
    bool SetFalg = false;
    SpriteRenderer ShadowRenderer;
    public Commander Commander;
    // Start is called before the first frame update
    void Start()
    {
        ShadowRenderer = gameObject.GetComponent<SpriteRenderer>();
    }
    private void OnEnable()
    {
        SetFalg = false;
        transform.localScale = new Vector3(0f, 0f, 0f);
    }
    // Update is called once per frame
    void Update()
    {
        if (SpriteRenderer.sprite != null)
        {
            if (!SetFalg)
            {
                Vector3 tempPos = new Vector3(0, 0, 0);
                //tempPos.y -= SpriteRenderer.size.y / 2;
                //transform.localScale = new Vector3(1f, 1f, 1f);
                switch (CommonBase.Type)
                {
                    case CommonType.Error:
                        break;
                    case CommonType.Squirrel:
                    case CommonType.Lizard:
                    case CommonType.Toad:
                    case CommonType.Mole:
                        transform.localPosition = tempPos;
                        transform.localScale = new Vector3(1f, 1f, 1f);
                        transform.localScale *= 0.12f;
                        break;
                    case CommonType.Pigeon:
                    case CommonType.Falcon:
                        tempPos.y -= 0.025f;
                        transform.localPosition = tempPos;
                        transform.localScale = new Vector3(1f, 1f, 1f);
                        transform.localScale *= 0.07f;
                        break;
                    case CommonType.Ferret:
                    case CommonType.Skunk:
                    case CommonType.Snake:
                        transform.localPosition = tempPos;
                        transform.localScale = new Vector3(1f, 1f, 1f);
                        transform.localScale *= 0.15f;
                        break;
                    case CommonType.Chameleon:
                        transform.localPosition = tempPos;
                        transform.localScale = new Vector3(1f, 1f, 1f);
                        transform.localScale *= 0.15f;
                        ShadowRenderer.color = SpriteRenderer.color;
                        break;
                    case CommonType.Boar:
                    case CommonType.Badger:
                    case CommonType.Wolf:
                    case CommonType.Fox:
                        transform.localPosition = tempPos;
                        transform.localScale = new Vector3(1f, 1f, 1f);
                        transform.localScale *= 0.3f;
                        break;
                    case CommonType.Owl:
                        tempPos.y -= 0.025f;
                        transform.localPosition = tempPos;
                        transform.localScale = new Vector3(1f, 1f, 1f);
                        transform.localScale *= 0.15f;
                        break;
                    case CommonType.Mouse:
                        transform.localPosition = tempPos;
                        transform.localScale = new Vector3(1f, 1f, 1f);
                        transform.localScale *= 0.07f;
                        break;
                    case CommonType.Pig:
                        break;
                    case CommonType.Commander:
                        transform.localScale = new Vector3(1f, 1f, 1f);
                        transform.localScale *= 0.2f;
                        Commander = Character.gameObject.GetComponent<Commander>();
                        break;
                    case CommonType.End:
                        break;
                }
                SetFalg = true;
            }
        }
        if (FogOfWar.Instance.CheckTileAlpha(Character.gameObject.transform.position, GameManager.Instance.CommanderList[0]))
        {
            if (Character.IsOnWater)
                ShadowRenderer.enabled = false;
            else
            {
                ShadowRenderer.enabled = true;
                if (!ReferenceEquals(Commander, null))
                {
                    if (CommonBase.Type == CommonType.Commander)
                    {
                        if (Commander.commanderFSM.curState == CommanderFSM.STATE.RESPAWN || Commander.commanderFSM.curState == CommanderFSM.STATE.RETURN)
                        {
                            ShadowRenderer.enabled = false;
                        }
                        else
                            ShadowRenderer.enabled = true;
                    }
                }
            }
        }
        else
        {
            ShadowRenderer.enabled = false;
        }
    }
}
