using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class TintElements : ScriptableObject
{
    public Dictionary<string, Sprite[]> TintDic = new Dictionary<string, Sprite[]>();
    string strkey;

    public void InitializeElement()
    {
        //string strkey;

        // 틴트 Multiple texture로 경로 문제로 노가다
        // 건물
        Sprite[] UnitTintSprite = Resources.LoadAll<Sprite>("Tints/Build/moleemergeflag_tint");

        strkey = UnitTintSprite[0].name.Substring(0, UnitTintSprite[0].name.LastIndexOf("_t"));
        TintDic.Add(strkey, UnitTintSprite);

        UnitTintSprite = Resources.LoadAll<Sprite>("Tints/Build/structure_artillery_tint");

        strkey = UnitTintSprite[0].name.Substring(0, UnitTintSprite[0].name.LastIndexOf("_t"));
        TintDic.Add(strkey, UnitTintSprite);

        UnitTintSprite = Resources.LoadAll<Sprite>("Tints/Build/structure_balloon_tint");

        strkey = UnitTintSprite[0].name.Substring(0, UnitTintSprite[0].name.LastIndexOf("_t"));
        TintDic.Add(strkey, UnitTintSprite);

        UnitTintSprite = Resources.LoadAll<Sprite>("Tints/Build/structure_barbedwire_tint");

        strkey = UnitTintSprite[0].name.Substring(0, UnitTintSprite[0].name.LastIndexOf("_t"));
        TintDic.Add(strkey, UnitTintSprite);

        UnitTintSprite = Resources.LoadAll<Sprite>("Tints/Build/structure_landmine_tint");

        strkey = UnitTintSprite[0].name.Substring(0, UnitTintSprite[0].name.LastIndexOf("_t"));
        TintDic.Add(strkey, UnitTintSprite);

        UnitTintSprite = Resources.LoadAll<Sprite>("Tints/Build/structure_machinegun_tint");

        strkey = UnitTintSprite[0].name.Substring(0, UnitTintSprite[0].name.LastIndexOf("_t"));
        TintDic.Add(strkey, UnitTintSprite);

        UnitTintSprite = Resources.LoadAll<Sprite>("Tints/Build/structure_warrens1_tint");

        strkey = UnitTintSprite[0].name.Substring(0, UnitTintSprite[0].name.LastIndexOf("_t"));
        TintDic.Add(strkey, UnitTintSprite);

        UnitTintSprite = Resources.LoadAll<Sprite>("Tints/Build/structure_warrens2_tint");

        strkey = UnitTintSprite[0].name.Substring(0, UnitTintSprite[0].name.LastIndexOf("_t"));
        TintDic.Add(strkey, UnitTintSprite);

        UnitTintSprite = Resources.LoadAll<Sprite>("Tints/Build/structure_warrens3_tint");

        strkey = UnitTintSprite[0].name.Substring(0, UnitTintSprite[0].name.LastIndexOf("_t"));
        TintDic.Add(strkey, UnitTintSprite);

        UnitTintSprite = Resources.LoadAll<Sprite>("Tints/Build/warren_manhole_tier1_tint");

        strkey = UnitTintSprite[0].name.Substring(0, UnitTintSprite[0].name.LastIndexOf("_t"));
        TintDic.Add(strkey, UnitTintSprite);

        UnitTintSprite = Resources.LoadAll<Sprite>("Tints/Build/warren_manhole_tier2_tint");

        strkey = UnitTintSprite[0].name.Substring(0, UnitTintSprite[0].name.LastIndexOf("_t"));
        TintDic.Add(strkey, UnitTintSprite);

        UnitTintSprite = Resources.LoadAll<Sprite>("Tints/Build/warren_manhole_tier3_tint");

        strkey = UnitTintSprite[0].name.Substring(0, UnitTintSprite[0].name.LastIndexOf("_t"));
        TintDic.Add(strkey, UnitTintSprite);

        UnitTintSprite = Resources.LoadAll<Sprite>("Tints/Build/windmill_base_tint");

        strkey = UnitTintSprite[0].name.Substring(0, UnitTintSprite[0].name.LastIndexOf("_t"));
        TintDic.Add(strkey, UnitTintSprite);

        UnitTintSprite = Resources.LoadAll<Sprite>("Tints/Build/windmill_topper_tint");

        strkey = UnitTintSprite[0].name.Substring(0, UnitTintSprite[0].name.LastIndexOf("_t"));
        TintDic.Add(strkey, UnitTintSprite);

        // 커맨더
        UnitTintSprite = Resources.LoadAll<Sprite>("Tints/Commanders/commander_capitalists_tint");

        strkey = UnitTintSprite[0].name.Substring(0, UnitTintSprite[0].name.LastIndexOf("_t"));
        TintDic.Add(strkey, UnitTintSprite);

        UnitTintSprite = Resources.LoadAll<Sprite>("Tints/Commanders/commander_clergy_tint");

        strkey = UnitTintSprite[0].name.Substring(0, UnitTintSprite[0].name.LastIndexOf("_t"));
        TintDic.Add(strkey, UnitTintSprite);

        UnitTintSprite = Resources.LoadAll<Sprite>("Tints/Commanders/commander_commoners_tint");

        strkey = UnitTintSprite[0].name.Substring(0, UnitTintSprite[0].name.LastIndexOf("_t"));
        TintDic.Add(strkey, UnitTintSprite);

        UnitTintSprite = Resources.LoadAll<Sprite>("Tints/Commanders/commander_military_tint");

        strkey = UnitTintSprite[0].name.Substring(0, UnitTintSprite[0].name.LastIndexOf("_t"));
        TintDic.Add(strkey, UnitTintSprite);

        // 유닛
        UnitTintSprite = Resources.LoadAll<Sprite>("Tints/Units/badger_tint");

        strkey = UnitTintSprite[0].name.Substring(0, UnitTintSprite[0].name.LastIndexOf("_t"));
        TintDic.Add(strkey, UnitTintSprite);

        UnitTintSprite = Resources.LoadAll<Sprite>("Tints/Units/boar_tint");

        strkey = UnitTintSprite[0].name.Substring(0, UnitTintSprite[0].name.LastIndexOf("_t"));
        TintDic.Add(strkey, UnitTintSprite);

        UnitTintSprite = Resources.LoadAll<Sprite>("Tints/Units/chameleon_tint");

        strkey = UnitTintSprite[0].name.Substring(0, UnitTintSprite[0].name.LastIndexOf("_t"));
        TintDic.Add(strkey, UnitTintSprite);

        UnitTintSprite = Resources.LoadAll<Sprite>("Tints/Units/falcon_tint");

        strkey = UnitTintSprite[0].name.Substring(0, UnitTintSprite[0].name.LastIndexOf("_t"));
        TintDic.Add(strkey, UnitTintSprite);

        UnitTintSprite = Resources.LoadAll<Sprite>("Tints/Units/ferret_tint");

        strkey = UnitTintSprite[0].name.Substring(0, UnitTintSprite[0].name.LastIndexOf("_t"));
        TintDic.Add(strkey, UnitTintSprite);

        UnitTintSprite = Resources.LoadAll<Sprite>("Tints/Units/fox_tint");

        strkey = UnitTintSprite[0].name.Substring(0, UnitTintSprite[0].name.LastIndexOf("_t"));
        TintDic.Add(strkey, UnitTintSprite);

        UnitTintSprite = Resources.LoadAll<Sprite>("Tints/Units/lizard_tint");

        strkey = UnitTintSprite[0].name.Substring(0, UnitTintSprite[0].name.LastIndexOf("_t"));
        TintDic.Add(strkey, UnitTintSprite);

        UnitTintSprite = Resources.LoadAll<Sprite>("Tints/Units/mole_tint");

        strkey = UnitTintSprite[0].name.Substring(0, UnitTintSprite[0].name.LastIndexOf("_t"));
        TintDic.Add(strkey, UnitTintSprite);

        UnitTintSprite = Resources.LoadAll<Sprite>("Tints/Units/mouse_tint");

        strkey = UnitTintSprite[0].name.Substring(0, UnitTintSprite[0].name.LastIndexOf("_t"));
        TintDic.Add(strkey, UnitTintSprite);

        UnitTintSprite = Resources.LoadAll<Sprite>("Tints/Units/owl_tint");

        strkey = UnitTintSprite[0].name.Substring(0, UnitTintSprite[0].name.LastIndexOf("_t"));
        TintDic.Add(strkey, UnitTintSprite);

        UnitTintSprite = Resources.LoadAll<Sprite>("Tints/Units/pig_tint");

        strkey = UnitTintSprite[0].name.Substring(0, UnitTintSprite[0].name.LastIndexOf("_t"));
        TintDic.Add(strkey, UnitTintSprite);

        UnitTintSprite = Resources.LoadAll<Sprite>("Tints/Units/pigeon_tint");

        strkey = UnitTintSprite[0].name.Substring(0, UnitTintSprite[0].name.LastIndexOf("_t"));
        TintDic.Add(strkey, UnitTintSprite);

        UnitTintSprite = Resources.LoadAll<Sprite>("Tints/Units/skunk_tint");

        strkey = UnitTintSprite[0].name.Substring(0, UnitTintSprite[0].name.LastIndexOf("_t"));
        TintDic.Add(strkey, UnitTintSprite);

        UnitTintSprite = Resources.LoadAll<Sprite>("Tints/Units/snake_tint");

        strkey = UnitTintSprite[0].name.Substring(0, UnitTintSprite[0].name.LastIndexOf("_t"));
        TintDic.Add(strkey, UnitTintSprite);

        UnitTintSprite = Resources.LoadAll<Sprite>("Tints/Units/squirrel_tint");

        strkey = UnitTintSprite[0].name.Substring(0, UnitTintSprite[0].name.LastIndexOf("_t"));
        TintDic.Add(strkey, UnitTintSprite);

        UnitTintSprite = Resources.LoadAll<Sprite>("Tints/Units/toad_tint");

        strkey = UnitTintSprite[0].name.Substring(0, UnitTintSprite[0].name.LastIndexOf("_t"));
        TintDic.Add(strkey, UnitTintSprite);

        UnitTintSprite = Resources.LoadAll<Sprite>("Tints/Units/wolf_tint");

        strkey = UnitTintSprite[0].name.Substring(0, UnitTintSprite[0].name.LastIndexOf("_t"));
        TintDic.Add(strkey, UnitTintSprite);

        UnitTintSprite = Resources.LoadAll<Sprite>("Textures/UI/Icon");

        for (int i = 0; i < UnitTintSprite.Length; ++i)
        {
            if (UnitTintSprite[i].name.LastIndexOf("_tint") > 0)
            {
                strkey = UnitTintSprite[i].name.Substring(0, UnitTintSprite[i].name.LastIndexOf("_t"));

                TintDic.Add(strkey, Resources.LoadAll<Sprite>("Textures/UI/Icon/" + UnitTintSprite[i].name));
            }
        }

        UnitTintSprite = Resources.LoadAll<Sprite>("Textures/UI/Portrait/Cropped");

        for (int i = 0; i < UnitTintSprite.Length; ++i)
        {
            if (UnitTintSprite[i].name.LastIndexOf("_tint") > 0)
            {
                strkey = UnitTintSprite[i].name.Substring(0, UnitTintSprite[i].name.LastIndexOf("_t"));
                TintDic.Add(strkey, Resources.LoadAll<Sprite>("Textures/UI/Portrait/Cropped/" + UnitTintSprite[i].name));
            }
        }

        UnitTintSprite = Resources.LoadAll<Sprite>("Textures/UI/Portrait/Small");

        for (int i = 0; i < UnitTintSprite.Length; ++i)
        {
            if (UnitTintSprite[i].name.LastIndexOf("_tint") > 0)
            {
                strkey = UnitTintSprite[i].name.Substring(0, UnitTintSprite[i].name.LastIndexOf("_t"));
                TintDic.Add(strkey, Resources.LoadAll<Sprite>("Textures/UI/Portrait/Small/" + UnitTintSprite[i].name));
            }
        }
    }
    public void InitializeElement_Build()
    {
        // 건물
        Sprite[] UnitTintSprite = Resources.LoadAll<Sprite>("Tints/Build/moleemergeflag_tint");

        strkey = UnitTintSprite[0].name.Substring(0, UnitTintSprite[0].name.LastIndexOf("_t"));
        TintDic.Add(strkey, UnitTintSprite);

        UnitTintSprite = Resources.LoadAll<Sprite>("Tints/Build/structure_artillery_tint");

        strkey = UnitTintSprite[0].name.Substring(0, UnitTintSprite[0].name.LastIndexOf("_t"));
        TintDic.Add(strkey, UnitTintSprite);

        UnitTintSprite = Resources.LoadAll<Sprite>("Tints/Build/structure_balloon_tint");

        strkey = UnitTintSprite[0].name.Substring(0, UnitTintSprite[0].name.LastIndexOf("_t"));
        TintDic.Add(strkey, UnitTintSprite);

        UnitTintSprite = Resources.LoadAll<Sprite>("Tints/Build/structure_barbedwire_tint");

        strkey = UnitTintSprite[0].name.Substring(0, UnitTintSprite[0].name.LastIndexOf("_t"));
        TintDic.Add(strkey, UnitTintSprite);

        UnitTintSprite = Resources.LoadAll<Sprite>("Tints/Build/structure_landmine_tint");

        strkey = UnitTintSprite[0].name.Substring(0, UnitTintSprite[0].name.LastIndexOf("_t"));
        TintDic.Add(strkey, UnitTintSprite);

        UnitTintSprite = Resources.LoadAll<Sprite>("Tints/Build/structure_machinegun_tint");

        strkey = UnitTintSprite[0].name.Substring(0, UnitTintSprite[0].name.LastIndexOf("_t"));
        TintDic.Add(strkey, UnitTintSprite);

        UnitTintSprite = Resources.LoadAll<Sprite>("Tints/Build/structure_warrens1_tint");

        strkey = UnitTintSprite[0].name.Substring(0, UnitTintSprite[0].name.LastIndexOf("_t"));
        TintDic.Add(strkey, UnitTintSprite);

        UnitTintSprite = Resources.LoadAll<Sprite>("Tints/Build/structure_warrens2_tint");

        strkey = UnitTintSprite[0].name.Substring(0, UnitTintSprite[0].name.LastIndexOf("_t"));
        TintDic.Add(strkey, UnitTintSprite);

        UnitTintSprite = Resources.LoadAll<Sprite>("Tints/Build/structure_warrens3_tint");

        strkey = UnitTintSprite[0].name.Substring(0, UnitTintSprite[0].name.LastIndexOf("_t"));
        TintDic.Add(strkey, UnitTintSprite);

        UnitTintSprite = Resources.LoadAll<Sprite>("Tints/Build/warren_manhole_tier1_tint");

        strkey = UnitTintSprite[0].name.Substring(0, UnitTintSprite[0].name.LastIndexOf("_t"));
        TintDic.Add(strkey, UnitTintSprite);

        UnitTintSprite = Resources.LoadAll<Sprite>("Tints/Build/warren_manhole_tier2_tint");

        strkey = UnitTintSprite[0].name.Substring(0, UnitTintSprite[0].name.LastIndexOf("_t"));
        TintDic.Add(strkey, UnitTintSprite);

        UnitTintSprite = Resources.LoadAll<Sprite>("Tints/Build/warren_manhole_tier3_tint");

        strkey = UnitTintSprite[0].name.Substring(0, UnitTintSprite[0].name.LastIndexOf("_t"));
        TintDic.Add(strkey, UnitTintSprite);

        UnitTintSprite = Resources.LoadAll<Sprite>("Tints/Build/windmill_base_tint");

        strkey = UnitTintSprite[0].name.Substring(0, UnitTintSprite[0].name.LastIndexOf("_t"));
        TintDic.Add(strkey, UnitTintSprite);

        UnitTintSprite = Resources.LoadAll<Sprite>("Tints/Build/windmill_topper_tint");

        strkey = UnitTintSprite[0].name.Substring(0, UnitTintSprite[0].name.LastIndexOf("_t"));
        TintDic.Add(strkey, UnitTintSprite);
    }
    public void InitializeElement_Commander()
    {
        Sprite[] UnitTintSprite;
        UnitTintSprite = Resources.LoadAll<Sprite>("Tints/Commanders/commander_capitalists_tint");

        strkey = UnitTintSprite[0].name.Substring(0, UnitTintSprite[0].name.LastIndexOf("_t"));
        TintDic.Add(strkey, UnitTintSprite);

        UnitTintSprite = Resources.LoadAll<Sprite>("Tints/Commanders/commander_clergy_tint");

        strkey = UnitTintSprite[0].name.Substring(0, UnitTintSprite[0].name.LastIndexOf("_t"));
        TintDic.Add(strkey, UnitTintSprite);

        UnitTintSprite = Resources.LoadAll<Sprite>("Tints/Commanders/commander_commoners_tint");

        strkey = UnitTintSprite[0].name.Substring(0, UnitTintSprite[0].name.LastIndexOf("_t"));
        TintDic.Add(strkey, UnitTintSprite);

        UnitTintSprite = Resources.LoadAll<Sprite>("Tints/Commanders/commander_military_tint");

        strkey = UnitTintSprite[0].name.Substring(0, UnitTintSprite[0].name.LastIndexOf("_t"));
        TintDic.Add(strkey, UnitTintSprite);
    }
    public void InitializeElement_Unit()
    {
        Sprite[] UnitTintSprite;
        UnitTintSprite = Resources.LoadAll<Sprite>("Tints/Units/badger_tint");

        strkey = UnitTintSprite[0].name.Substring(0, UnitTintSprite[0].name.LastIndexOf("_t"));
        TintDic.Add(strkey, UnitTintSprite);

        UnitTintSprite = Resources.LoadAll<Sprite>("Tints/Units/boar_tint");

        strkey = UnitTintSprite[0].name.Substring(0, UnitTintSprite[0].name.LastIndexOf("_t"));
        TintDic.Add(strkey, UnitTintSprite);

        UnitTintSprite = Resources.LoadAll<Sprite>("Tints/Units/chameleon_tint");

        strkey = UnitTintSprite[0].name.Substring(0, UnitTintSprite[0].name.LastIndexOf("_t"));
        TintDic.Add(strkey, UnitTintSprite);

        UnitTintSprite = Resources.LoadAll<Sprite>("Tints/Units/falcon_tint");

        strkey = UnitTintSprite[0].name.Substring(0, UnitTintSprite[0].name.LastIndexOf("_t"));
        TintDic.Add(strkey, UnitTintSprite);

        UnitTintSprite = Resources.LoadAll<Sprite>("Tints/Units/ferret_tint");

        strkey = UnitTintSprite[0].name.Substring(0, UnitTintSprite[0].name.LastIndexOf("_t"));
        TintDic.Add(strkey, UnitTintSprite);

        UnitTintSprite = Resources.LoadAll<Sprite>("Tints/Units/fox_tint");

        strkey = UnitTintSprite[0].name.Substring(0, UnitTintSprite[0].name.LastIndexOf("_t"));
        TintDic.Add(strkey, UnitTintSprite);

        UnitTintSprite = Resources.LoadAll<Sprite>("Tints/Units/lizard_tint");

        strkey = UnitTintSprite[0].name.Substring(0, UnitTintSprite[0].name.LastIndexOf("_t"));
        TintDic.Add(strkey, UnitTintSprite);

        UnitTintSprite = Resources.LoadAll<Sprite>("Tints/Units/mole_tint");

        strkey = UnitTintSprite[0].name.Substring(0, UnitTintSprite[0].name.LastIndexOf("_t"));
        TintDic.Add(strkey, UnitTintSprite);

        UnitTintSprite = Resources.LoadAll<Sprite>("Tints/Units/mouse_tint");

        strkey = UnitTintSprite[0].name.Substring(0, UnitTintSprite[0].name.LastIndexOf("_t"));
        TintDic.Add(strkey, UnitTintSprite);

        UnitTintSprite = Resources.LoadAll<Sprite>("Tints/Units/owl_tint");

        strkey = UnitTintSprite[0].name.Substring(0, UnitTintSprite[0].name.LastIndexOf("_t"));
        TintDic.Add(strkey, UnitTintSprite);

        UnitTintSprite = Resources.LoadAll<Sprite>("Tints/Units/pig_tint");

        strkey = UnitTintSprite[0].name.Substring(0, UnitTintSprite[0].name.LastIndexOf("_t"));
        TintDic.Add(strkey, UnitTintSprite);

        UnitTintSprite = Resources.LoadAll<Sprite>("Tints/Units/pigeon_tint");

        strkey = UnitTintSprite[0].name.Substring(0, UnitTintSprite[0].name.LastIndexOf("_t"));
        TintDic.Add(strkey, UnitTintSprite);

        UnitTintSprite = Resources.LoadAll<Sprite>("Tints/Units/skunk_tint");

        strkey = UnitTintSprite[0].name.Substring(0, UnitTintSprite[0].name.LastIndexOf("_t"));
        TintDic.Add(strkey, UnitTintSprite);

        UnitTintSprite = Resources.LoadAll<Sprite>("Tints/Units/snake_tint");

        strkey = UnitTintSprite[0].name.Substring(0, UnitTintSprite[0].name.LastIndexOf("_t"));
        TintDic.Add(strkey, UnitTintSprite);

        UnitTintSprite = Resources.LoadAll<Sprite>("Tints/Units/squirrel_tint");

        strkey = UnitTintSprite[0].name.Substring(0, UnitTintSprite[0].name.LastIndexOf("_t"));
        TintDic.Add(strkey, UnitTintSprite);

        UnitTintSprite = Resources.LoadAll<Sprite>("Tints/Units/toad_tint");

        strkey = UnitTintSprite[0].name.Substring(0, UnitTintSprite[0].name.LastIndexOf("_t"));
        TintDic.Add(strkey, UnitTintSprite);

        UnitTintSprite = Resources.LoadAll<Sprite>("Tints/Units/wolf_tint");

        strkey = UnitTintSprite[0].name.Substring(0, UnitTintSprite[0].name.LastIndexOf("_t"));
        TintDic.Add(strkey, UnitTintSprite);


    }
    public void InitializeElement_UI()
    {
        Sprite[] UnitTintSprite;
        UnitTintSprite = Resources.LoadAll<Sprite>("Textures/UI/Icon");

        for (int i = 0; i < UnitTintSprite.Length; ++i)
        {
            if (UnitTintSprite[i].name.LastIndexOf("_tint") > 0)
            {
                strkey = UnitTintSprite[i].name.Substring(0, UnitTintSprite[i].name.LastIndexOf("_t"));

                TintDic.Add(strkey, Resources.LoadAll<Sprite>("Textures/UI/Icon/" + UnitTintSprite[i].name));
            }
        }

        UnitTintSprite = Resources.LoadAll<Sprite>("Textures/UI/Portrait/Cropped");

        for (int i = 0; i < UnitTintSprite.Length; ++i)
        {
            if (UnitTintSprite[i].name.LastIndexOf("_tint") > 0)
            {
                strkey = UnitTintSprite[i].name.Substring(0, UnitTintSprite[i].name.LastIndexOf("_t"));
                TintDic.Add(strkey, Resources.LoadAll<Sprite>("Textures/UI/Portrait/Cropped/" + UnitTintSprite[i].name));
            }
        }

        UnitTintSprite = Resources.LoadAll<Sprite>("Textures/UI/Portrait/Small");

        for (int i = 0; i < UnitTintSprite.Length; ++i)
        {
            if (UnitTintSprite[i].name.LastIndexOf("_tint") > 0)
            {
                strkey = UnitTintSprite[i].name.Substring(0, UnitTintSprite[i].name.LastIndexOf("_t"));
                TintDic.Add(strkey, Resources.LoadAll<Sprite>("Textures/UI/Portrait/Small/" + UnitTintSprite[i].name));
            }
        }
    }
}
