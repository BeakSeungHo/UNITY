using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//[ExecuteInEditMode]
public class TeamColor : MonoBehaviour
{
    public enum TYPE { INGAME, UI, INGAMEUI };
    [SerializeField] private Color ComColor = Color.white;

    private CommonBase Base;
    public Camp Camp;

    private SpriteRenderer SpriteRender;
    public Sprite TintSprite;
    private Sprite[] TintSpriteAll;
    [SerializeField] private string TextureName = "";
    [SerializeField] private int TextureNameCount = 0;
    MaterialPropertyBlock MaterialProp;

    public Material UnitMaterial;
    public bool LoadFlag;

    public TYPE type;
    // UI용
    private BattleReadyWindow BattleReadyWindow;
    string tempNum;

    public bool ReLoad = false;
    void OnEnable()
    {
    }
    private void Start()
    {
        SpriteRender = transform.parent.gameObject.GetComponent<SpriteRenderer>();
        SpriteRender.material = UnitMaterial;

        TintSprite = GetComponent<Sprite>();

        switch (type)
        {
            case TYPE.INGAME:
                Base = transform.parent.transform.parent.gameObject.GetComponent<CommonBase>();
                Camp = Base.MyCamp;
                break;
            case TYPE.UI:
                BattleReadyWindow = transform.parent.parent.parent.parent.gameObject.GetComponent<BattleReadyWindow>();
                break;
            case TYPE.INGAMEUI:
                Camp = GameManager.Instance.CommanderList[0];
                break;
        }

        MaterialProp = new MaterialPropertyBlock();


        LoadFlag = false;
    }
    void OnDisable()
    {
        //UpdateColor();
        LoadFlag = false;
    }

    void Update()
    {
        SetColor();
        if (SpriteRender.sprite != null)
        {
            if (!ReLoad)
            {
                switch (type)
                {
                    case TYPE.INGAME:
                        if (!LoadFlag)
                        {
                            TexLoadStart();
                            LoadFlag = true;
                        }
                        Camp = Base.MyCamp;
                        break;
                    case TYPE.UI:
                    case TYPE.INGAMEUI:
                        TexLoadStart();
                        break;
                }
                TintTextureLoad();
                UpdateColor();
            }
        }

    }
    private void LateUpdate()
    {
        if (ReLoad)
        {
            TexLoadStart();
            ReLoad = false;
        }
    }
    //바뀐 색을 쉐이더에 전달
    void UpdateColor()
    {
        //MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        if (MaterialProp != null)
        {
            SpriteRender.GetPropertyBlock(MaterialProp);

            MaterialProp.SetColor("_TeamColor", ComColor);
            MaterialProp.SetColor("_SpriteColor", SpriteRender.color);
            MaterialProp.SetTexture("_TintTex", TintSprite.texture);
            MaterialProp.SetInt("_RenderState", 1);

            SpriteRender.SetPropertyBlock(MaterialProp);
        }
    }
    //받아온 Tint Texture을 Material에 등록
    void TintTextureLoad()
    {
        switch (type)
        {
            case TYPE.INGAME:
                tempNum = SpriteRender.sprite.name.Substring(TextureNameCount, SpriteRender.sprite.name.Length - TextureNameCount);
                int index = 0;
                if (int.TryParse(tempNum, out index))
                {
                    if (TintSpriteAll.Length <= index || index < 0)
                        Debug.Log("index is out of range, index : " + index + " - sprite Name : " + SpriteRender.sprite.name + " TextureName : " + TextureName);
                    else
                        TintSprite = TintSpriteAll[index];
                }
                else
                {
                    Debug.Log("int.TryParse failed - sprite Name : " + SpriteRender.sprite.name + " TextureName : " + TextureName);
                }

                break;
            case TYPE.UI:
            case TYPE.INGAMEUI:
                TintSprite = TintSpriteAll[0];
                break;
        }
        //SpriteRender.sharedMaterial.SetTexture("_TintTex", TintSprite.texture);
    }
    //Tint Texture를 찾아서 등록
    void TexName()
    {
        int count = 0;
        int tempStringCount = SpriteRender.sprite.name.Length;

        switch (type)
        {
            case TYPE.INGAME:
                while (true)
                {
                    string tempFind = SpriteRender.sprite.name.Substring(tempStringCount - (count + 2), 1);
                    if (tempFind == "_")
                    {
                        TextureName = SpriteRender.sprite.name.Substring(0, tempStringCount - (count + 2));

                        TextureNameCount = tempStringCount - (count + 1);
                        TintSpriteAll = SceneStarter.Instance.tintElements.TintDic[TextureName];
                        break;
                    }
                    else
                        count++;
                }
                break;
            case TYPE.UI:
            case TYPE.INGAMEUI:
                TextureName = SpriteRender.sprite.name;
                //Debug.Log(TextureName);
                TintSpriteAll = SceneStarter.Instance.tintElements.TintDic[TextureName];
                break;
        }
    }
    void SetColor()
    {
        switch (type)
        {
            case TYPE.INGAME:
            case TYPE.INGAMEUI:
                switch (Camp)
                {
                    case Camp.Hopper:
                        ComColor = Global.CommanderInGameColorHopper;
                        break;
                    case Camp.Quartermaster:
                        ComColor = Global.CommanderInGameColorQuartermaster;
                        break;
                    case Camp.Bellafide:
                        ComColor = Global.CommanderInGameColorBellafide;
                        break;
                    case Camp.Archimedes:
                        ComColor = Global.CommanderInGameColorArchimedes;
                        break;
                }
                break;
            case TYPE.UI:
                switch (BattleReadyWindow.curCmdType)
                {
                    case 0:
                        ComColor = Global.CommanderUIColorBellafide;
                        break;
                    case 1:
                        ComColor = Global.CommanderUIColorHopper;
                        break;
                    case 2:
                        ComColor = Global.CommanderUIColorQuartermaster;
                        break;
                    case 3:
                        ComColor = Global.CommanderUIColorArchimedes;
                        break;
                }
                break;
        }
    }
    public void TexLoadStart()
    {
        TexName();
        TintTextureLoad();
        UpdateColor();
    }
}
