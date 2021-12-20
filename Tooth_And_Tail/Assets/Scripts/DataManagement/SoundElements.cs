using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;

public enum LobbySoundType { None, Back, Enter, Exit, Ready, Unready, Button, In, Out, Select, Deselect, Pause, Unpause, Rankup, End }

public enum UnitSoundType { None, Idle, Attack, Hurt, Death, Cry, Explosion, Reload, Stealth, Spit, End }

public enum ComSoundType { None, Start, Idle, Attack, Hurt, Death, Return, Build, Sell, Cry, Enterwater, End }

public enum BuildSoundType { None, Construct, Complete, Idle, Butcher, Attack, Reload, Explosion, Destroy, Plowing, End }

public enum FootSoundType { None, Dirt, Grass, Mud, Sand, Snow, Stone, Water, Wetlayer, Wood, End }

public enum EffectSoundType { None, Gun, Hammer, Fire, Fireoff, End }

public class SoundElements : ScriptableObject
{
    public Dictionary<Camp, AudioClip[]> BackSoundDic = new Dictionary<Camp, AudioClip[]>();
    public Dictionary<LobbySoundType, List<AudioClip>> LobbySoundDic = new Dictionary<LobbySoundType, List<AudioClip>>();
    public Dictionary<FootSoundType, List<AudioClip>> FootSoundDic = new Dictionary<FootSoundType, List<AudioClip>>();
    public Dictionary<EffectSoundType, List<AudioClip>> EffectSoundDic = new Dictionary<EffectSoundType, List<AudioClip>>();

    public Dictionary<Camp, Dictionary<ComSoundType, List<AudioClip>>> ComSoundDic = new Dictionary<Camp, Dictionary<ComSoundType, List<AudioClip>>>();
    public Dictionary<CommonType, Dictionary<UnitSoundType, List<AudioClip>>> UnitSoundDic = new Dictionary<CommonType, Dictionary<UnitSoundType, List<AudioClip>>>();
    public Dictionary<CommonType, Dictionary<BuildSoundType, List<AudioClip>>> BuildSoundDic = new Dictionary<CommonType, Dictionary<BuildSoundType, List<AudioClip>>>();

    string strkey;

    // string의 앞부분을 대문자로 바꾸기위한 TextInfo
    TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

    public void InitializeElement()
    {
        AudioClip[] BackAudioClips = Resources.LoadAll<AudioClip>("Sound/Back/Bellafide");
        BackSoundDic.Add(Camp.Bellafide, BackAudioClips);
        BackAudioClips = Resources.LoadAll<AudioClip>("Sound/Back/Hopper");
        BackSoundDic.Add(Camp.Hopper, BackAudioClips);
        BackAudioClips = Resources.LoadAll<AudioClip>("Sound/Back/Archimedes");
        BackSoundDic.Add(Camp.Archimedes, BackAudioClips);
        BackAudioClips = Resources.LoadAll<AudioClip>("Sound/Back/Quartermaster");
        BackSoundDic.Add(Camp.Quartermaster, BackAudioClips);
        
        AudioClip[] LobbyAudioClips = Resources.LoadAll<AudioClip>("Sound/Lobby");
        for (int i = 0; i < LobbyAudioClips.Length; ++i)
        {
            strkey = LobbyAudioClips[i].name.Substring(0, LobbyAudioClips[i].name.LastIndexOf('_'));
            strkey = strkey.Substring(strkey.LastIndexOf('_') + 1, strkey.Length - strkey.LastIndexOf('_') - 1);
            strkey = textInfo.ToTitleCase(strkey);

            if (!strkey.ToEnum<LobbySoundType>().Equals(-1) &&
                !LobbySoundDic.ContainsKey((LobbySoundType)strkey.ToEnum<LobbySoundType>()))
            {
                LobbySoundDic.Add((LobbySoundType)strkey.ToEnum<LobbySoundType>(), new List<AudioClip>());
                LobbySoundDic[(LobbySoundType)strkey.ToEnum<LobbySoundType>()].Add(LobbyAudioClips[i]);
            }
            else if (!strkey.ToEnum<UnitSoundType>().Equals(-1) &&
                LobbySoundDic.ContainsKey((LobbySoundType)strkey.ToEnum<LobbySoundType>()))
            {
                LobbySoundDic[(LobbySoundType)strkey.ToEnum<LobbySoundType>()].Add(LobbyAudioClips[i]);
            }
        }
        LobbyAudioClips = Resources.LoadAll<AudioClip>("Sound/Back/Lobby");
        LobbySoundDic.Add(LobbySoundType.Back, new List<AudioClip>());
        for (int i = 0; i < LobbyAudioClips.Length; ++i)
            LobbySoundDic[LobbySoundType.Back].Add(LobbyAudioClips[i]);

        AudioClip[] FootAudioClips = Resources.LoadAll<AudioClip>("Sound/Foot");
        for (int i = 0; i < FootAudioClips.Length; ++i)
        {
            strkey = FootAudioClips[i].name.Substring(0, FootAudioClips[i].name.LastIndexOf('_'));
            strkey = strkey.Substring(strkey.LastIndexOf('_') + 1, strkey.Length - strkey.LastIndexOf('_') - 1);
            strkey = textInfo.ToTitleCase(strkey);

            if (!strkey.ToEnum<FootSoundType>().Equals(-1) &&
                !FootSoundDic.ContainsKey((FootSoundType)strkey.ToEnum<FootSoundType>()))
            {
                FootSoundDic.Add((FootSoundType)strkey.ToEnum<FootSoundType>(), new List<AudioClip>());
                FootSoundDic[(FootSoundType)strkey.ToEnum<FootSoundType>()].Add(FootAudioClips[i]);
            }
            else if (!strkey.ToEnum<FootSoundType>().Equals(-1) &&
                FootSoundDic.ContainsKey((FootSoundType)strkey.ToEnum<FootSoundType>()))
            {
                FootSoundDic[(FootSoundType)strkey.ToEnum<FootSoundType>()].Add(FootAudioClips[i]);
            }
        }

        AudioClip[] EffectAudioClips = Resources.LoadAll<AudioClip>("Sound/Effect");
        for (int i = 0; i < EffectAudioClips.Length; ++i)
        {
            strkey = EffectAudioClips[i].name.Substring(0, EffectAudioClips[i].name.LastIndexOf('_'));
            strkey = strkey.Substring(strkey.LastIndexOf('_') + 1, strkey.Length - strkey.LastIndexOf('_') - 1);
            strkey = textInfo.ToTitleCase(strkey);

            if (!strkey.ToEnum<EffectSoundType>().Equals(-1) &&
                !EffectSoundDic.ContainsKey((EffectSoundType)strkey.ToEnum<EffectSoundType>()))
            {
                EffectSoundDic.Add((EffectSoundType)strkey.ToEnum<EffectSoundType>(), new List<AudioClip>());
                EffectSoundDic[(EffectSoundType)strkey.ToEnum<EffectSoundType>()].Add(EffectAudioClips[i]);
            }
            else if (!strkey.ToEnum<EffectSoundType>().Equals(-1) &&
                EffectSoundDic.ContainsKey((EffectSoundType)strkey.ToEnum<EffectSoundType>()))
            {
                EffectSoundDic[(EffectSoundType)strkey.ToEnum<EffectSoundType>()].Add(EffectAudioClips[i]);
            }
        }

        ComSoundDic.Add(Camp.Bellafide, new Dictionary<ComSoundType, List<AudioClip>>());
        ComSoundDic.Add(Camp.Hopper, new Dictionary<ComSoundType, List<AudioClip>>());
        ComSoundDic.Add(Camp.Archimedes, new Dictionary<ComSoundType, List<AudioClip>>());
        ComSoundDic.Add(Camp.Quartermaster, new Dictionary<ComSoundType, List<AudioClip>>());

        AudioClip[] ComAudioClips = Resources.LoadAll<AudioClip>("Sound/General");
        for (int i = 0; i < ComAudioClips.Length; ++i)
        {
            strkey = ComAudioClips[i].name.Substring(0, ComAudioClips[i].name.LastIndexOf('_'));
            strkey = strkey.Substring(strkey.LastIndexOf('_') + 1, strkey.Length - strkey.LastIndexOf('_') - 1);
            strkey = textInfo.ToTitleCase(strkey);

            if (!strkey.ToEnum<ComSoundType>().Equals(-1) &&
                !ComSoundDic[Camp.Hopper].ContainsKey((ComSoundType)strkey.ToEnum<ComSoundType>()))
            {
                ComSoundDic[Camp.Hopper].Add((ComSoundType)strkey.ToEnum<ComSoundType>(), new List<AudioClip>());
                ComSoundDic[Camp.Hopper][(ComSoundType)strkey.ToEnum<ComSoundType>()].Add(ComAudioClips[i]);
                ComSoundDic[Camp.Bellafide].Add((ComSoundType)strkey.ToEnum<ComSoundType>(), new List<AudioClip>());
                ComSoundDic[Camp.Bellafide][(ComSoundType)strkey.ToEnum<ComSoundType>()].Add(ComAudioClips[i]);
                ComSoundDic[Camp.Archimedes].Add((ComSoundType)strkey.ToEnum<ComSoundType>(), new List<AudioClip>());
                ComSoundDic[Camp.Archimedes][(ComSoundType)strkey.ToEnum<ComSoundType>()].Add(ComAudioClips[i]);
                ComSoundDic[Camp.Quartermaster].Add((ComSoundType)strkey.ToEnum<ComSoundType>(), new List<AudioClip>());
                ComSoundDic[Camp.Quartermaster][(ComSoundType)strkey.ToEnum<ComSoundType>()].Add(ComAudioClips[i]);
            }
            else if (!strkey.ToEnum<ComSoundType>().Equals(-1) &&
                ComSoundDic[Camp.Hopper].ContainsKey((ComSoundType)strkey.ToEnum<ComSoundType>()))
            {
                ComSoundDic[Camp.Hopper][(ComSoundType)strkey.ToEnum<ComSoundType>()].Add(ComAudioClips[i]);
                ComSoundDic[Camp.Bellafide][(ComSoundType)strkey.ToEnum<ComSoundType>()].Add(ComAudioClips[i]);
                ComSoundDic[Camp.Archimedes][(ComSoundType)strkey.ToEnum<ComSoundType>()].Add(ComAudioClips[i]);
                ComSoundDic[Camp.Quartermaster][(ComSoundType)strkey.ToEnum<ComSoundType>()].Add(ComAudioClips[i]);
            }
        }


        ComAudioClips = Resources.LoadAll<AudioClip>("Sound/Hopper");
        for (int i = 0; i < ComAudioClips.Length; ++i)
        {
            strkey = ComAudioClips[i].name.Substring(0, ComAudioClips[i].name.LastIndexOf('_'));
            strkey = strkey.Substring(strkey.LastIndexOf('_') + 1, strkey.Length - strkey.LastIndexOf('_') - 1);
            strkey = textInfo.ToTitleCase(strkey);

            if (!strkey.ToEnum<ComSoundType>().Equals(-1) &&
                !ComSoundDic[Camp.Hopper].ContainsKey((ComSoundType)strkey.ToEnum<ComSoundType>()))
            {
                ComSoundDic[Camp.Hopper].Add((ComSoundType)strkey.ToEnum<ComSoundType>(), new List<AudioClip>());
                ComSoundDic[Camp.Hopper][(ComSoundType)strkey.ToEnum<ComSoundType>()].Add(ComAudioClips[i]);
            }
            else if (!strkey.ToEnum<ComSoundType>().Equals(-1) &&
                ComSoundDic[Camp.Hopper].ContainsKey((ComSoundType)strkey.ToEnum<ComSoundType>()))
            {
                ComSoundDic[Camp.Hopper][(ComSoundType)strkey.ToEnum<ComSoundType>()].Add(ComAudioClips[i]);
            }
        }

        ComAudioClips = Resources.LoadAll<AudioClip>("Sound/Bellafide");
        for (int i = 0; i < ComAudioClips.Length; ++i)
        {
            strkey = ComAudioClips[i].name.Substring(0, ComAudioClips[i].name.LastIndexOf('_'));
            strkey = strkey.Substring(strkey.LastIndexOf('_') + 1, strkey.Length - strkey.LastIndexOf('_') - 1);
            strkey = textInfo.ToTitleCase(strkey);

            if (!strkey.ToEnum<ComSoundType>().Equals(-1) &&
                !ComSoundDic[Camp.Bellafide].ContainsKey((ComSoundType)strkey.ToEnum<ComSoundType>()))
            {
                ComSoundDic[Camp.Bellafide].Add((ComSoundType)strkey.ToEnum<ComSoundType>(), new List<AudioClip>());
                ComSoundDic[Camp.Bellafide][(ComSoundType)strkey.ToEnum<ComSoundType>()].Add(ComAudioClips[i]);
            }
            else if (!strkey.ToEnum<ComSoundType>().Equals(-1) &&
                ComSoundDic[Camp.Bellafide].ContainsKey((ComSoundType)strkey.ToEnum<ComSoundType>()))
            {
                ComSoundDic[Camp.Bellafide][(ComSoundType)strkey.ToEnum<ComSoundType>()].Add(ComAudioClips[i]);
            }
        }

        ComAudioClips = Resources.LoadAll<AudioClip>("Sound/Archimedes");
        for (int i = 0; i < ComAudioClips.Length; ++i)
        {
            strkey = ComAudioClips[i].name.Substring(0, ComAudioClips[i].name.LastIndexOf('_'));
            strkey = strkey.Substring(strkey.LastIndexOf('_') + 1, strkey.Length - strkey.LastIndexOf('_') - 1);
            strkey = textInfo.ToTitleCase(strkey);

            if (!strkey.ToEnum<ComSoundType>().Equals(-1) &&
                !ComSoundDic[Camp.Archimedes].ContainsKey((ComSoundType)strkey.ToEnum<ComSoundType>()))
            {
                ComSoundDic[Camp.Archimedes].Add((ComSoundType)strkey.ToEnum<ComSoundType>(), new List<AudioClip>());
                ComSoundDic[Camp.Archimedes][(ComSoundType)strkey.ToEnum<ComSoundType>()].Add(ComAudioClips[i]);
            }
            else if (!strkey.ToEnum<ComSoundType>().Equals(-1) &&
                ComSoundDic[Camp.Archimedes].ContainsKey((ComSoundType)strkey.ToEnum<ComSoundType>()))
            {
                ComSoundDic[Camp.Archimedes][(ComSoundType)strkey.ToEnum<ComSoundType>()].Add(ComAudioClips[i]);
            }
        }

        ComAudioClips = Resources.LoadAll<AudioClip>("Sound/Quartermaster");
        for (int i = 0; i < ComAudioClips.Length; ++i)
        {
            strkey = ComAudioClips[i].name.Substring(0, ComAudioClips[i].name.LastIndexOf('_'));
            strkey = strkey.Substring(strkey.LastIndexOf('_') + 1, strkey.Length - strkey.LastIndexOf('_') - 1);
            strkey = textInfo.ToTitleCase(strkey);

            if (!strkey.ToEnum<ComSoundType>().Equals(-1) &&
                !ComSoundDic[Camp.Quartermaster].ContainsKey((ComSoundType)strkey.ToEnum<ComSoundType>()))
            {
                ComSoundDic[Camp.Quartermaster].Add((ComSoundType)strkey.ToEnum<ComSoundType>(), new List<AudioClip>());
                ComSoundDic[Camp.Quartermaster][(ComSoundType)strkey.ToEnum<ComSoundType>()].Add(ComAudioClips[i]);
            }
            else if (!strkey.ToEnum<ComSoundType>().Equals(-1) &&
                ComSoundDic[Camp.Quartermaster].ContainsKey((ComSoundType)strkey.ToEnum<ComSoundType>()))
            {
                ComSoundDic[Camp.Quartermaster][(ComSoundType)strkey.ToEnum<ComSoundType>()].Add(ComAudioClips[i]);
            }
        }

        UnitSoundDic.Add(CommonType.Squirrel, new Dictionary<UnitSoundType, List<AudioClip>>());
        UnitSoundDic.Add(CommonType.Lizard, new Dictionary<UnitSoundType, List<AudioClip>>());
        UnitSoundDic.Add(CommonType.Toad, new Dictionary<UnitSoundType, List<AudioClip>>());
        UnitSoundDic.Add(CommonType.Pigeon, new Dictionary<UnitSoundType, List<AudioClip>>());
        UnitSoundDic.Add(CommonType.Mole, new Dictionary<UnitSoundType, List<AudioClip>>());
        UnitSoundDic.Add(CommonType.Mouse, new Dictionary<UnitSoundType, List<AudioClip>>());
        UnitSoundDic.Add(CommonType.Ferret, new Dictionary<UnitSoundType, List<AudioClip>>());
        UnitSoundDic.Add(CommonType.Falcon, new Dictionary<UnitSoundType, List<AudioClip>>());
        UnitSoundDic.Add(CommonType.Skunk, new Dictionary<UnitSoundType, List<AudioClip>>());
        UnitSoundDic.Add(CommonType.Chameleon, new Dictionary<UnitSoundType, List<AudioClip>>());
        UnitSoundDic.Add(CommonType.Snake, new Dictionary<UnitSoundType, List<AudioClip>>());
        UnitSoundDic.Add(CommonType.Boar, new Dictionary<UnitSoundType, List<AudioClip>>());
        UnitSoundDic.Add(CommonType.Badger, new Dictionary<UnitSoundType, List<AudioClip>>());
        UnitSoundDic.Add(CommonType.Owl, new Dictionary<UnitSoundType, List<AudioClip>>());
        UnitSoundDic.Add(CommonType.Wolf, new Dictionary<UnitSoundType, List<AudioClip>>());
        UnitSoundDic.Add(CommonType.Fox, new Dictionary<UnitSoundType, List<AudioClip>>());

        AudioClip[] UnitAudioClips = Resources.LoadAll<AudioClip>("Sound/Squirrel");
        for (int i = 0; i < UnitAudioClips.Length; ++i)
        {
            strkey = UnitAudioClips[i].name.Substring(0, UnitAudioClips[i].name.LastIndexOf('_'));
            strkey = strkey.Substring(strkey.LastIndexOf('_') + 1, strkey.Length - strkey.LastIndexOf('_') - 1);
            strkey = textInfo.ToTitleCase(strkey);

            if (!strkey.ToEnum<UnitSoundType>().Equals(-1) &&
                !UnitSoundDic[CommonType.Squirrel].ContainsKey((UnitSoundType)strkey.ToEnum<UnitSoundType>()))
            {
                UnitSoundDic[CommonType.Squirrel].Add((UnitSoundType)strkey.ToEnum<UnitSoundType>(), new List<AudioClip>());
                UnitSoundDic[CommonType.Squirrel][(UnitSoundType)strkey.ToEnum<UnitSoundType>()].Add(UnitAudioClips[i]);
            }
            else if (!strkey.ToEnum<UnitSoundType>().Equals(-1) &&
                UnitSoundDic[CommonType.Squirrel].ContainsKey((UnitSoundType)strkey.ToEnum<UnitSoundType>()))
            {
                UnitSoundDic[CommonType.Squirrel][(UnitSoundType)strkey.ToEnum<UnitSoundType>()].Add(UnitAudioClips[i]);
            }
        }

        UnitAudioClips = Resources.LoadAll<AudioClip>("Sound/Lizard");
        for (int i = 0; i < UnitAudioClips.Length; ++i)
        {
            strkey = UnitAudioClips[i].name.Substring(0, UnitAudioClips[i].name.LastIndexOf('_'));
            strkey = strkey.Substring(strkey.LastIndexOf('_') + 1, strkey.Length - strkey.LastIndexOf('_') - 1);
            strkey = textInfo.ToTitleCase(strkey);

            if (!strkey.ToEnum<UnitSoundType>().Equals(-1) &&
                !UnitSoundDic[CommonType.Lizard].ContainsKey((UnitSoundType)strkey.ToEnum<UnitSoundType>()))
            {
                UnitSoundDic[CommonType.Lizard].Add((UnitSoundType)strkey.ToEnum<UnitSoundType>(), new List<AudioClip>());
                UnitSoundDic[CommonType.Lizard][(UnitSoundType)strkey.ToEnum<UnitSoundType>()].Add(UnitAudioClips[i]);
            }
            else if (!strkey.ToEnum<UnitSoundType>().Equals(-1) &&
                UnitSoundDic[CommonType.Lizard].ContainsKey((UnitSoundType)strkey.ToEnum<UnitSoundType>()))
            {
                UnitSoundDic[CommonType.Lizard][(UnitSoundType)strkey.ToEnum<UnitSoundType>()].Add(UnitAudioClips[i]);
            }
        }

        UnitAudioClips = Resources.LoadAll<AudioClip>("Sound/Toad");
        for (int i = 0; i < UnitAudioClips.Length; ++i)
        {
            strkey = UnitAudioClips[i].name.Substring(0, UnitAudioClips[i].name.LastIndexOf('_'));
            strkey = strkey.Substring(strkey.LastIndexOf('_') + 1, strkey.Length - strkey.LastIndexOf('_') - 1);
            strkey = textInfo.ToTitleCase(strkey);

            if (!strkey.ToEnum<UnitSoundType>().Equals(-1) &&
                !UnitSoundDic[CommonType.Toad].ContainsKey((UnitSoundType)strkey.ToEnum<UnitSoundType>()))
            {
                UnitSoundDic[CommonType.Toad].Add((UnitSoundType)strkey.ToEnum<UnitSoundType>(), new List<AudioClip>());
                UnitSoundDic[CommonType.Toad][(UnitSoundType)strkey.ToEnum<UnitSoundType>()].Add(UnitAudioClips[i]);
            }
            else if (!strkey.ToEnum<UnitSoundType>().Equals(-1) &&
                UnitSoundDic[CommonType.Toad].ContainsKey((UnitSoundType)strkey.ToEnum<UnitSoundType>()))
            {
                UnitSoundDic[CommonType.Toad][(UnitSoundType)strkey.ToEnum<UnitSoundType>()].Add(UnitAudioClips[i]);
            }
        }

        UnitAudioClips = Resources.LoadAll<AudioClip>("Sound/Pigeon");
        for (int i = 0; i < UnitAudioClips.Length; ++i)
        {
            strkey = UnitAudioClips[i].name.Substring(0, UnitAudioClips[i].name.LastIndexOf('_'));
            strkey = strkey.Substring(strkey.LastIndexOf('_') + 1, strkey.Length - strkey.LastIndexOf('_') - 1);
            strkey = textInfo.ToTitleCase(strkey);

            if (!strkey.ToEnum<UnitSoundType>().Equals(-1) &&
                !UnitSoundDic[CommonType.Pigeon].ContainsKey((UnitSoundType)strkey.ToEnum<UnitSoundType>()))
            {
                UnitSoundDic[CommonType.Pigeon].Add((UnitSoundType)strkey.ToEnum<UnitSoundType>(), new List<AudioClip>());
                UnitSoundDic[CommonType.Pigeon][(UnitSoundType)strkey.ToEnum<UnitSoundType>()].Add(UnitAudioClips[i]);
            }
            else if (!strkey.ToEnum<UnitSoundType>().Equals(-1) &&
                UnitSoundDic[CommonType.Pigeon].ContainsKey((UnitSoundType)strkey.ToEnum<UnitSoundType>()))
            {
                UnitSoundDic[CommonType.Pigeon][(UnitSoundType)strkey.ToEnum<UnitSoundType>()].Add(UnitAudioClips[i]);
            }
        }

        UnitAudioClips = Resources.LoadAll<AudioClip>("Sound/Mole");
        for (int i = 0; i < UnitAudioClips.Length; ++i)
        {
            strkey = UnitAudioClips[i].name.Substring(0, UnitAudioClips[i].name.LastIndexOf('_'));
            strkey = strkey.Substring(strkey.LastIndexOf('_') + 1, strkey.Length - strkey.LastIndexOf('_') - 1);
            strkey = textInfo.ToTitleCase(strkey);

            if (!strkey.ToEnum<UnitSoundType>().Equals(-1) &&
                !UnitSoundDic[CommonType.Mole].ContainsKey((UnitSoundType)strkey.ToEnum<UnitSoundType>()))
            {
                UnitSoundDic[CommonType.Mole].Add((UnitSoundType)strkey.ToEnum<UnitSoundType>(), new List<AudioClip>());
                UnitSoundDic[CommonType.Mole][(UnitSoundType)strkey.ToEnum<UnitSoundType>()].Add(UnitAudioClips[i]);
            }
            else if (!strkey.ToEnum<UnitSoundType>().Equals(-1) &&
                UnitSoundDic[CommonType.Mole].ContainsKey((UnitSoundType)strkey.ToEnum<UnitSoundType>()))
            {
                UnitSoundDic[CommonType.Mole][(UnitSoundType)strkey.ToEnum<UnitSoundType>()].Add(UnitAudioClips[i]);
            }
        }

        UnitAudioClips = Resources.LoadAll<AudioClip>("Sound/Ferret");
        for (int i = 0; i < UnitAudioClips.Length; ++i)
        {
            strkey = UnitAudioClips[i].name.Substring(0, UnitAudioClips[i].name.LastIndexOf('_'));
            strkey = strkey.Substring(strkey.LastIndexOf('_') + 1, strkey.Length - strkey.LastIndexOf('_') - 1);
            strkey = textInfo.ToTitleCase(strkey);

            if (!strkey.ToEnum<UnitSoundType>().Equals(-1) &&
                !UnitSoundDic[CommonType.Ferret].ContainsKey((UnitSoundType)strkey.ToEnum<UnitSoundType>()))
            {
                UnitSoundDic[CommonType.Ferret].Add((UnitSoundType)strkey.ToEnum<UnitSoundType>(), new List<AudioClip>());
                UnitSoundDic[CommonType.Ferret][(UnitSoundType)strkey.ToEnum<UnitSoundType>()].Add(UnitAudioClips[i]);
            }
            else if (!strkey.ToEnum<UnitSoundType>().Equals(-1) &&
                UnitSoundDic[CommonType.Ferret].ContainsKey((UnitSoundType)strkey.ToEnum<UnitSoundType>()))
            {
                UnitSoundDic[CommonType.Ferret][(UnitSoundType)strkey.ToEnum<UnitSoundType>()].Add(UnitAudioClips[i]);
            }
        }

        UnitAudioClips = Resources.LoadAll<AudioClip>("Sound/Falcon");
        for (int i = 0; i < UnitAudioClips.Length; ++i)
        {
            strkey = UnitAudioClips[i].name.Substring(0, UnitAudioClips[i].name.LastIndexOf('_'));
            strkey = strkey.Substring(strkey.LastIndexOf('_') + 1, strkey.Length - strkey.LastIndexOf('_') - 1);
            strkey = textInfo.ToTitleCase(strkey);

            if (!strkey.ToEnum<UnitSoundType>().Equals(-1) &&
                !UnitSoundDic[CommonType.Falcon].ContainsKey((UnitSoundType)strkey.ToEnum<UnitSoundType>()))
            {
                UnitSoundDic[CommonType.Falcon].Add((UnitSoundType)strkey.ToEnum<UnitSoundType>(), new List<AudioClip>());
                UnitSoundDic[CommonType.Falcon][(UnitSoundType)strkey.ToEnum<UnitSoundType>()].Add(UnitAudioClips[i]);
            }
            else if (!strkey.ToEnum<UnitSoundType>().Equals(-1) &&
                UnitSoundDic[CommonType.Falcon].ContainsKey((UnitSoundType)strkey.ToEnum<UnitSoundType>()))
            {
                UnitSoundDic[CommonType.Falcon][(UnitSoundType)strkey.ToEnum<UnitSoundType>()].Add(UnitAudioClips[i]);
            }
        }

        UnitAudioClips = Resources.LoadAll<AudioClip>("Sound/Skunk");
        for (int i = 0; i < UnitAudioClips.Length; ++i)
        {
            strkey = UnitAudioClips[i].name.Substring(0, UnitAudioClips[i].name.LastIndexOf('_'));
            strkey = strkey.Substring(strkey.LastIndexOf('_') + 1, strkey.Length - strkey.LastIndexOf('_') - 1);
            strkey = textInfo.ToTitleCase(strkey);

            if (!strkey.ToEnum<UnitSoundType>().Equals(-1) &&
                !UnitSoundDic[CommonType.Skunk].ContainsKey((UnitSoundType)strkey.ToEnum<UnitSoundType>()))
            {
                UnitSoundDic[CommonType.Skunk].Add((UnitSoundType)strkey.ToEnum<UnitSoundType>(), new List<AudioClip>());
                UnitSoundDic[CommonType.Skunk][(UnitSoundType)strkey.ToEnum<UnitSoundType>()].Add(UnitAudioClips[i]);
            }
            else if (!strkey.ToEnum<UnitSoundType>().Equals(-1) &&
                UnitSoundDic[CommonType.Skunk].ContainsKey((UnitSoundType)strkey.ToEnum<UnitSoundType>()))
            {
                UnitSoundDic[CommonType.Skunk][(UnitSoundType)strkey.ToEnum<UnitSoundType>()].Add(UnitAudioClips[i]);
            }
        }

        UnitAudioClips = Resources.LoadAll<AudioClip>("Sound/Chameleon");
        for (int i = 0; i < UnitAudioClips.Length; ++i)
        {
            strkey = UnitAudioClips[i].name.Substring(0, UnitAudioClips[i].name.LastIndexOf('_'));
            strkey = strkey.Substring(strkey.LastIndexOf('_') + 1, strkey.Length - strkey.LastIndexOf('_') - 1);
            strkey = textInfo.ToTitleCase(strkey);

            if (!strkey.ToEnum<UnitSoundType>().Equals(-1) &&
                !UnitSoundDic[CommonType.Chameleon].ContainsKey((UnitSoundType)strkey.ToEnum<UnitSoundType>()))
            {
                UnitSoundDic[CommonType.Chameleon].Add((UnitSoundType)strkey.ToEnum<UnitSoundType>(), new List<AudioClip>());
                UnitSoundDic[CommonType.Chameleon][(UnitSoundType)strkey.ToEnum<UnitSoundType>()].Add(UnitAudioClips[i]);
            }
            else if (!strkey.ToEnum<UnitSoundType>().Equals(-1) &&
                UnitSoundDic[CommonType.Chameleon].ContainsKey((UnitSoundType)strkey.ToEnum<UnitSoundType>()))
            {
                UnitSoundDic[CommonType.Chameleon][(UnitSoundType)strkey.ToEnum<UnitSoundType>()].Add(UnitAudioClips[i]);
            }
        }

        UnitAudioClips = Resources.LoadAll<AudioClip>("Sound/Snake");
        for (int i = 0; i < UnitAudioClips.Length; ++i)
        {
            strkey = UnitAudioClips[i].name.Substring(0, UnitAudioClips[i].name.LastIndexOf('_'));
            strkey = strkey.Substring(strkey.LastIndexOf('_') + 1, strkey.Length - strkey.LastIndexOf('_') - 1);
            strkey = textInfo.ToTitleCase(strkey);

            if (!strkey.ToEnum<UnitSoundType>().Equals(-1) &&
                !UnitSoundDic[CommonType.Snake].ContainsKey((UnitSoundType)strkey.ToEnum<UnitSoundType>()))
            {
                UnitSoundDic[CommonType.Snake].Add((UnitSoundType)strkey.ToEnum<UnitSoundType>(), new List<AudioClip>());
                UnitSoundDic[CommonType.Snake][(UnitSoundType)strkey.ToEnum<UnitSoundType>()].Add(UnitAudioClips[i]);
            }
            else if (!strkey.ToEnum<UnitSoundType>().Equals(-1) &&
                UnitSoundDic[CommonType.Snake].ContainsKey((UnitSoundType)strkey.ToEnum<UnitSoundType>()))
            {
                UnitSoundDic[CommonType.Snake][(UnitSoundType)strkey.ToEnum<UnitSoundType>()].Add(UnitAudioClips[i]);
            }
        }

        UnitAudioClips = Resources.LoadAll<AudioClip>("Sound/Boar");
        for (int i = 0; i < UnitAudioClips.Length; ++i)
        {
            strkey = UnitAudioClips[i].name.Substring(0, UnitAudioClips[i].name.LastIndexOf('_'));
            strkey = strkey.Substring(strkey.LastIndexOf('_') + 1, strkey.Length - strkey.LastIndexOf('_') - 1);
            strkey = textInfo.ToTitleCase(strkey);

            if (!strkey.ToEnum<UnitSoundType>().Equals(-1) &&
                !UnitSoundDic[CommonType.Boar].ContainsKey((UnitSoundType)strkey.ToEnum<UnitSoundType>()))
            {
                UnitSoundDic[CommonType.Boar].Add((UnitSoundType)strkey.ToEnum<UnitSoundType>(), new List<AudioClip>());
                UnitSoundDic[CommonType.Boar][(UnitSoundType)strkey.ToEnum<UnitSoundType>()].Add(UnitAudioClips[i]);
            }
            else if (!strkey.ToEnum<UnitSoundType>().Equals(-1) &&
                UnitSoundDic[CommonType.Boar].ContainsKey((UnitSoundType)strkey.ToEnum<UnitSoundType>()))
            {
                UnitSoundDic[CommonType.Boar][(UnitSoundType)strkey.ToEnum<UnitSoundType>()].Add(UnitAudioClips[i]);
            }
        }

        UnitAudioClips = Resources.LoadAll<AudioClip>("Sound/Badger");
        for (int i = 0; i < UnitAudioClips.Length; ++i)
        {
            strkey = UnitAudioClips[i].name.Substring(0, UnitAudioClips[i].name.LastIndexOf('_'));
            strkey = strkey.Substring(strkey.LastIndexOf('_') + 1, strkey.Length - strkey.LastIndexOf('_') - 1);
            strkey = textInfo.ToTitleCase(strkey);

            if (!strkey.ToEnum<UnitSoundType>().Equals(-1) &&
                !UnitSoundDic[CommonType.Badger].ContainsKey((UnitSoundType)strkey.ToEnum<UnitSoundType>()))
            {
                UnitSoundDic[CommonType.Badger].Add((UnitSoundType)strkey.ToEnum<UnitSoundType>(), new List<AudioClip>());
                UnitSoundDic[CommonType.Badger][(UnitSoundType)strkey.ToEnum<UnitSoundType>()].Add(UnitAudioClips[i]);
            }
            else if (!strkey.ToEnum<UnitSoundType>().Equals(-1) &&
                UnitSoundDic[CommonType.Badger].ContainsKey((UnitSoundType)strkey.ToEnum<UnitSoundType>()))
            {
                UnitSoundDic[CommonType.Badger][(UnitSoundType)strkey.ToEnum<UnitSoundType>()].Add(UnitAudioClips[i]);
            }
        }

        UnitAudioClips = Resources.LoadAll<AudioClip>("Sound/Owl");
        for (int i = 0; i < UnitAudioClips.Length; ++i)
        {
            strkey = UnitAudioClips[i].name.Substring(0, UnitAudioClips[i].name.LastIndexOf('_'));
            strkey = strkey.Substring(strkey.LastIndexOf('_') + 1, strkey.Length - strkey.LastIndexOf('_') - 1);
            strkey = textInfo.ToTitleCase(strkey);

            if (!strkey.ToEnum<UnitSoundType>().Equals(-1) &&
                !UnitSoundDic[CommonType.Owl].ContainsKey((UnitSoundType)strkey.ToEnum<UnitSoundType>()))
            {
                UnitSoundDic[CommonType.Owl].Add((UnitSoundType)strkey.ToEnum<UnitSoundType>(), new List<AudioClip>());
                UnitSoundDic[CommonType.Owl][(UnitSoundType)strkey.ToEnum<UnitSoundType>()].Add(UnitAudioClips[i]);
            }
            else if (!strkey.ToEnum<UnitSoundType>().Equals(-1) &&
                UnitSoundDic[CommonType.Owl].ContainsKey((UnitSoundType)strkey.ToEnum<UnitSoundType>()))
            {
                UnitSoundDic[CommonType.Owl][(UnitSoundType)strkey.ToEnum<UnitSoundType>()].Add(UnitAudioClips[i]);
            }
        }

        UnitAudioClips = Resources.LoadAll<AudioClip>("Sound/Wolf");
        for (int i = 0; i < UnitAudioClips.Length; ++i)
        {
            strkey = UnitAudioClips[i].name.Substring(0, UnitAudioClips[i].name.LastIndexOf('_'));
            strkey = strkey.Substring(strkey.LastIndexOf('_') + 1, strkey.Length - strkey.LastIndexOf('_') - 1);
            strkey = textInfo.ToTitleCase(strkey);

            if (!strkey.ToEnum<UnitSoundType>().Equals(-1) &&
                !UnitSoundDic[CommonType.Wolf].ContainsKey((UnitSoundType)strkey.ToEnum<UnitSoundType>()))
            {
                UnitSoundDic[CommonType.Wolf].Add((UnitSoundType)strkey.ToEnum<UnitSoundType>(), new List<AudioClip>());
                UnitSoundDic[CommonType.Wolf][(UnitSoundType)strkey.ToEnum<UnitSoundType>()].Add(UnitAudioClips[i]);
            }
            else if (!strkey.ToEnum<UnitSoundType>().Equals(-1) &&
                UnitSoundDic[CommonType.Wolf].ContainsKey((UnitSoundType)strkey.ToEnum<UnitSoundType>()))
            {
                UnitSoundDic[CommonType.Wolf][(UnitSoundType)strkey.ToEnum<UnitSoundType>()].Add(UnitAudioClips[i]);
            }
        }

        UnitAudioClips = Resources.LoadAll<AudioClip>("Sound/Fox");
        for (int i = 0; i < UnitAudioClips.Length; ++i)
        {
            strkey = UnitAudioClips[i].name.Substring(0, UnitAudioClips[i].name.LastIndexOf('_'));
            strkey = strkey.Substring(strkey.LastIndexOf('_') + 1, strkey.Length - strkey.LastIndexOf('_') - 1);
            strkey = textInfo.ToTitleCase(strkey);

            if (!strkey.ToEnum<UnitSoundType>().Equals(-1) &&
                !UnitSoundDic[CommonType.Fox].ContainsKey((UnitSoundType)strkey.ToEnum<UnitSoundType>()))
            {
                UnitSoundDic[CommonType.Fox].Add((UnitSoundType)strkey.ToEnum<UnitSoundType>(), new List<AudioClip>());
                UnitSoundDic[CommonType.Fox][(UnitSoundType)strkey.ToEnum<UnitSoundType>()].Add(UnitAudioClips[i]);
            }
            else if (!strkey.ToEnum<UnitSoundType>().Equals(-1) &&
                UnitSoundDic[CommonType.Fox].ContainsKey((UnitSoundType)strkey.ToEnum<UnitSoundType>()))
            {
                UnitSoundDic[CommonType.Fox][(UnitSoundType)strkey.ToEnum<UnitSoundType>()].Add(UnitAudioClips[i]);
            }
        }

        UnitAudioClips = Resources.LoadAll<AudioClip>("Sound/Mouse");
        for (int i = 0; i < UnitAudioClips.Length; ++i)
        {
            strkey = UnitAudioClips[i].name.Substring(0, UnitAudioClips[i].name.LastIndexOf('_'));
            strkey = strkey.Substring(strkey.LastIndexOf('_') + 1, strkey.Length - strkey.LastIndexOf('_') - 1);
            strkey = textInfo.ToTitleCase(strkey);

            if (!strkey.ToEnum<UnitSoundType>().Equals(-1) &&
                !UnitSoundDic[CommonType.Mouse].ContainsKey((UnitSoundType)strkey.ToEnum<UnitSoundType>()))
            {
                UnitSoundDic[CommonType.Mouse].Add((UnitSoundType)strkey.ToEnum<UnitSoundType>(), new List<AudioClip>());
                UnitSoundDic[CommonType.Mouse][(UnitSoundType)strkey.ToEnum<UnitSoundType>()].Add(UnitAudioClips[i]);
            }
            else if (!strkey.ToEnum<UnitSoundType>().Equals(-1) &&
                UnitSoundDic[CommonType.Mouse].ContainsKey((UnitSoundType)strkey.ToEnum<UnitSoundType>()))
            {
                UnitSoundDic[CommonType.Mouse][(UnitSoundType)strkey.ToEnum<UnitSoundType>()].Add(UnitAudioClips[i]);
            }
        }

        BuildSoundDic.Add(CommonType.Wire, new Dictionary<BuildSoundType, List<AudioClip>>());
        BuildSoundDic.Add(CommonType.Mine, new Dictionary<BuildSoundType, List<AudioClip>>());
        BuildSoundDic.Add(CommonType.Turret, new Dictionary<BuildSoundType, List<AudioClip>>());
        BuildSoundDic.Add(CommonType.Balloon, new Dictionary<BuildSoundType, List<AudioClip>>());
        BuildSoundDic.Add(CommonType.Cannon, new Dictionary<BuildSoundType, List<AudioClip>>());
        BuildSoundDic.Add(CommonType.Gristmill, new Dictionary<BuildSoundType, List<AudioClip>>());
        BuildSoundDic.Add(CommonType.WarrenT1, new Dictionary<BuildSoundType, List<AudioClip>>());
        BuildSoundDic.Add(CommonType.WarrenT2, new Dictionary<BuildSoundType, List<AudioClip>>());
        BuildSoundDic.Add(CommonType.WarrenT3, new Dictionary<BuildSoundType, List<AudioClip>>());
        BuildSoundDic.Add(CommonType.Cabin, new Dictionary<BuildSoundType, List<AudioClip>>());
        BuildSoundDic.Add(CommonType.MoleeMerge, new Dictionary<BuildSoundType, List<AudioClip>>());
        BuildSoundDic.Add(CommonType.Pig, new Dictionary<BuildSoundType, List<AudioClip>>());
        BuildSoundDic.Add(CommonType.Farm, new Dictionary<BuildSoundType, List<AudioClip>>());
        BuildSoundDic.Add(CommonType.CampFire, new Dictionary<BuildSoundType, List<AudioClip>>());

        AudioClip[] BuildingAudioClips = Resources.LoadAll<AudioClip>("Sound/General");
        for (int i = 0; i < BuildingAudioClips.Length; ++i)
        {
            strkey = BuildingAudioClips[i].name.Substring(0, BuildingAudioClips[i].name.LastIndexOf('_'));
            strkey = strkey.Substring(strkey.LastIndexOf('_') + 1, strkey.Length - strkey.LastIndexOf('_') - 1);
            strkey = textInfo.ToTitleCase(strkey);

            if (!strkey.ToEnum<BuildSoundType>().Equals(-1) &&
                !BuildSoundDic[CommonType.Wire].ContainsKey((BuildSoundType)strkey.ToEnum<BuildSoundType>()))
            {
                BuildSoundDic[CommonType.Wire].Add((BuildSoundType)strkey.ToEnum<BuildSoundType>(), new List<AudioClip>());
                BuildSoundDic[CommonType.Wire][(BuildSoundType)strkey.ToEnum<BuildSoundType>()].Add(BuildingAudioClips[i]);
                BuildSoundDic[CommonType.Mine].Add((BuildSoundType)strkey.ToEnum<BuildSoundType>(), new List<AudioClip>());
                BuildSoundDic[CommonType.Mine][(BuildSoundType)strkey.ToEnum<BuildSoundType>()].Add(BuildingAudioClips[i]);
                BuildSoundDic[CommonType.Turret].Add((BuildSoundType)strkey.ToEnum<BuildSoundType>(), new List<AudioClip>());
                BuildSoundDic[CommonType.Turret][(BuildSoundType)strkey.ToEnum<BuildSoundType>()].Add(BuildingAudioClips[i]);
                BuildSoundDic[CommonType.Balloon].Add((BuildSoundType)strkey.ToEnum<BuildSoundType>(), new List<AudioClip>());
                BuildSoundDic[CommonType.Balloon][(BuildSoundType)strkey.ToEnum<BuildSoundType>()].Add(BuildingAudioClips[i]);
                BuildSoundDic[CommonType.Cannon].Add((BuildSoundType)strkey.ToEnum<BuildSoundType>(), new List<AudioClip>());
                BuildSoundDic[CommonType.Cannon][(BuildSoundType)strkey.ToEnum<BuildSoundType>()].Add(BuildingAudioClips[i]);
                BuildSoundDic[CommonType.WarrenT1].Add((BuildSoundType)strkey.ToEnum<BuildSoundType>(), new List<AudioClip>());
                BuildSoundDic[CommonType.WarrenT1][(BuildSoundType)strkey.ToEnum<BuildSoundType>()].Add(BuildingAudioClips[i]);
                BuildSoundDic[CommonType.WarrenT2].Add((BuildSoundType)strkey.ToEnum<BuildSoundType>(), new List<AudioClip>());
                BuildSoundDic[CommonType.WarrenT2][(BuildSoundType)strkey.ToEnum<BuildSoundType>()].Add(BuildingAudioClips[i]);
                BuildSoundDic[CommonType.WarrenT3].Add((BuildSoundType)strkey.ToEnum<BuildSoundType>(), new List<AudioClip>());
                BuildSoundDic[CommonType.WarrenT3][(BuildSoundType)strkey.ToEnum<BuildSoundType>()].Add(BuildingAudioClips[i]);
                BuildSoundDic[CommonType.MoleeMerge].Add((BuildSoundType)strkey.ToEnum<BuildSoundType>(), new List<AudioClip>());
                BuildSoundDic[CommonType.MoleeMerge][(BuildSoundType)strkey.ToEnum<BuildSoundType>()].Add(BuildingAudioClips[i]);
            }
            else if (!strkey.ToEnum<BuildSoundType>().Equals(-1) &&
                BuildSoundDic[CommonType.Wire].ContainsKey((BuildSoundType)strkey.ToEnum<BuildSoundType>()))
            {
                BuildSoundDic[CommonType.Wire][(BuildSoundType)strkey.ToEnum<BuildSoundType>()].Add(BuildingAudioClips[i]);
                BuildSoundDic[CommonType.Mine][(BuildSoundType)strkey.ToEnum<BuildSoundType>()].Add(BuildingAudioClips[i]);
                BuildSoundDic[CommonType.Turret][(BuildSoundType)strkey.ToEnum<BuildSoundType>()].Add(BuildingAudioClips[i]);
                BuildSoundDic[CommonType.Balloon][(BuildSoundType)strkey.ToEnum<BuildSoundType>()].Add(BuildingAudioClips[i]);
                BuildSoundDic[CommonType.Cannon][(BuildSoundType)strkey.ToEnum<BuildSoundType>()].Add(BuildingAudioClips[i]);
                BuildSoundDic[CommonType.WarrenT1][(BuildSoundType)strkey.ToEnum<BuildSoundType>()].Add(BuildingAudioClips[i]);
                BuildSoundDic[CommonType.WarrenT2][(BuildSoundType)strkey.ToEnum<BuildSoundType>()].Add(BuildingAudioClips[i]);
                BuildSoundDic[CommonType.WarrenT3][(BuildSoundType)strkey.ToEnum<BuildSoundType>()].Add(BuildingAudioClips[i]);
                BuildSoundDic[CommonType.MoleeMerge][(BuildSoundType)strkey.ToEnum<BuildSoundType>()].Add(BuildingAudioClips[i]);
            }
        }

        BuildingAudioClips = Resources.LoadAll<AudioClip>("Sound/Wire/Complete");
        BuildSoundDic[CommonType.Wire].Add(BuildSoundType.Complete, new List<AudioClip>());
        for (int i = 0; i < BuildingAudioClips.Length; ++i)
            BuildSoundDic[CommonType.Wire][BuildSoundType.Complete].Add(BuildingAudioClips[i]);

        BuildingAudioClips = Resources.LoadAll<AudioClip>("Sound/Mine");
        for (int i = 0; i < BuildingAudioClips.Length; ++i)
        {
            strkey = BuildingAudioClips[i].name.Substring(0, BuildingAudioClips[i].name.LastIndexOf('_'));
            strkey = strkey.Substring(strkey.LastIndexOf('_') + 1, strkey.Length - strkey.LastIndexOf('_') - 1);
            strkey = textInfo.ToTitleCase(strkey);

            if (!strkey.ToEnum<BuildSoundType>().Equals(-1) &&
                !BuildSoundDic[CommonType.Mine].ContainsKey((BuildSoundType)strkey.ToEnum<BuildSoundType>()))
            {
                BuildSoundDic[CommonType.Mine].Add((BuildSoundType)strkey.ToEnum<BuildSoundType>(), new List<AudioClip>());
                BuildSoundDic[CommonType.Mine][(BuildSoundType)strkey.ToEnum<BuildSoundType>()].Add(BuildingAudioClips[i]);
            }
            else if (!strkey.ToEnum<BuildSoundType>().Equals(-1) &&
                BuildSoundDic[CommonType.Mine].ContainsKey((BuildSoundType)strkey.ToEnum<BuildSoundType>()))
            {
                BuildSoundDic[CommonType.Mine][(BuildSoundType)strkey.ToEnum<BuildSoundType>()].Add(BuildingAudioClips[i]);
            }
        }

        BuildingAudioClips = Resources.LoadAll<AudioClip>("Sound/Turret");
        for (int i = 0; i < BuildingAudioClips.Length; ++i)
        {
            strkey = BuildingAudioClips[i].name.Substring(0, BuildingAudioClips[i].name.LastIndexOf('_'));
            strkey = strkey.Substring(strkey.LastIndexOf('_') + 1, strkey.Length - strkey.LastIndexOf('_') - 1);
            strkey = textInfo.ToTitleCase(strkey);

            if (!strkey.ToEnum<BuildSoundType>().Equals(-1) &&
                !BuildSoundDic[CommonType.Turret].ContainsKey((BuildSoundType)strkey.ToEnum<BuildSoundType>()))
            {
                BuildSoundDic[CommonType.Turret].Add((BuildSoundType)strkey.ToEnum<BuildSoundType>(), new List<AudioClip>());
                BuildSoundDic[CommonType.Turret][(BuildSoundType)strkey.ToEnum<BuildSoundType>()].Add(BuildingAudioClips[i]);
            }
            else if (!strkey.ToEnum<BuildSoundType>().Equals(-1) &&
                BuildSoundDic[CommonType.Turret].ContainsKey((BuildSoundType)strkey.ToEnum<BuildSoundType>()))
            {
                BuildSoundDic[CommonType.Turret][(BuildSoundType)strkey.ToEnum<BuildSoundType>()].Add(BuildingAudioClips[i]);
            }
        }

        BuildingAudioClips = Resources.LoadAll<AudioClip>("Sound/Balloon");
        for (int i = 0; i < BuildingAudioClips.Length; ++i)
        {
            strkey = BuildingAudioClips[i].name.Substring(0, BuildingAudioClips[i].name.LastIndexOf('_'));
            strkey = strkey.Substring(strkey.LastIndexOf('_') + 1, strkey.Length - strkey.LastIndexOf('_') - 1);
            strkey = textInfo.ToTitleCase(strkey);

            if (!strkey.ToEnum<BuildSoundType>().Equals(-1) &&
                !BuildSoundDic[CommonType.Balloon].ContainsKey((BuildSoundType)strkey.ToEnum<BuildSoundType>()))
            {
                BuildSoundDic[CommonType.Balloon].Add((BuildSoundType)strkey.ToEnum<BuildSoundType>(), new List<AudioClip>());
                BuildSoundDic[CommonType.Balloon][(BuildSoundType)strkey.ToEnum<BuildSoundType>()].Add(BuildingAudioClips[i]);
            }
            else if (!strkey.ToEnum<BuildSoundType>().Equals(-1) &&
                BuildSoundDic[CommonType.Balloon].ContainsKey((BuildSoundType)strkey.ToEnum<BuildSoundType>()))
            {
                BuildSoundDic[CommonType.Balloon][(BuildSoundType)strkey.ToEnum<BuildSoundType>()].Add(BuildingAudioClips[i]);
            }
        }

        BuildingAudioClips = Resources.LoadAll<AudioClip>("Sound/Cannon");
        for (int i = 0; i < BuildingAudioClips.Length; ++i)
        {
            strkey = BuildingAudioClips[i].name.Substring(0, BuildingAudioClips[i].name.LastIndexOf('_'));
            strkey = strkey.Substring(strkey.LastIndexOf('_') + 1, strkey.Length - strkey.LastIndexOf('_') - 1);
            strkey = textInfo.ToTitleCase(strkey);

            if (!strkey.ToEnum<BuildSoundType>().Equals(-1) &&
                !BuildSoundDic[CommonType.Cannon].ContainsKey((BuildSoundType)strkey.ToEnum<BuildSoundType>()))
            {
                BuildSoundDic[CommonType.Cannon].Add((BuildSoundType)strkey.ToEnum<BuildSoundType>(), new List<AudioClip>());
                BuildSoundDic[CommonType.Cannon][(BuildSoundType)strkey.ToEnum<BuildSoundType>()].Add(BuildingAudioClips[i]);
            }
            else if (!strkey.ToEnum<BuildSoundType>().Equals(-1) &&
                BuildSoundDic[CommonType.Cannon].ContainsKey((BuildSoundType)strkey.ToEnum<BuildSoundType>()))
            {
                BuildSoundDic[CommonType.Cannon][(BuildSoundType)strkey.ToEnum<BuildSoundType>()].Add(BuildingAudioClips[i]);
            }
        }

        AudioClip BuildingAudioClip = Resources.Load<AudioClip>("Sound/Warren/T1/un_str_t1_bldseq");
        BuildSoundDic[CommonType.WarrenT1].Add(BuildSoundType.Complete, new List<AudioClip>());
        BuildSoundDic[CommonType.WarrenT1][BuildSoundType.Complete].Add(BuildingAudioClip);
        BuildingAudioClip = Resources.Load<AudioClip>("Sound/Warren/T2/un_str_t2_bldseq");
        BuildSoundDic[CommonType.WarrenT2].Add(BuildSoundType.Complete, new List<AudioClip>());
        BuildSoundDic[CommonType.WarrenT2][BuildSoundType.Complete].Add(BuildingAudioClip);
        BuildingAudioClip = Resources.Load<AudioClip>("Sound/Warren/T3/un_str_t3_bldseq");
        BuildSoundDic[CommonType.WarrenT3].Add(BuildSoundType.Complete, new List<AudioClip>());
        BuildSoundDic[CommonType.WarrenT3][BuildSoundType.Complete].Add(BuildingAudioClip);

        BuildingAudioClips = Resources.LoadAll<AudioClip>("Sound/Gristmill");
        for (int i = 0; i < BuildingAudioClips.Length; ++i)
        {
            strkey = BuildingAudioClips[i].name.Substring(0, BuildingAudioClips[i].name.LastIndexOf('_'));
            strkey = strkey.Substring(strkey.LastIndexOf('_') + 1, strkey.Length - strkey.LastIndexOf('_') - 1);
            strkey = textInfo.ToTitleCase(strkey);

            if (!strkey.ToEnum<BuildSoundType>().Equals(-1) &&
                !BuildSoundDic[CommonType.Gristmill].ContainsKey((BuildSoundType)strkey.ToEnum<BuildSoundType>()))
            {
                BuildSoundDic[CommonType.Gristmill].Add((BuildSoundType)strkey.ToEnum<BuildSoundType>(), new List<AudioClip>());
                BuildSoundDic[CommonType.Gristmill][(BuildSoundType)strkey.ToEnum<BuildSoundType>()].Add(BuildingAudioClips[i]);
            }
            else if (!strkey.ToEnum<BuildSoundType>().Equals(-1) &&
                BuildSoundDic[CommonType.Gristmill].ContainsKey((BuildSoundType)strkey.ToEnum<BuildSoundType>()))
            {
                BuildSoundDic[CommonType.Gristmill][(BuildSoundType)strkey.ToEnum<BuildSoundType>()].Add(BuildingAudioClips[i]);
            }
        }

        BuildingAudioClips = Resources.LoadAll<AudioClip>("Sound/Cabin");
        for (int i = 0; i < BuildingAudioClips.Length; ++i)
        {
            strkey = BuildingAudioClips[i].name.Substring(0, BuildingAudioClips[i].name.LastIndexOf('_'));
            strkey = strkey.Substring(strkey.LastIndexOf('_') + 1, strkey.Length - strkey.LastIndexOf('_') - 1);
            strkey = textInfo.ToTitleCase(strkey);

            if (!strkey.ToEnum<BuildSoundType>().Equals(-1) &&
                !BuildSoundDic[CommonType.Cabin].ContainsKey((BuildSoundType)strkey.ToEnum<BuildSoundType>()))
            {
                BuildSoundDic[CommonType.Cabin].Add((BuildSoundType)strkey.ToEnum<BuildSoundType>(), new List<AudioClip>());
                BuildSoundDic[CommonType.Cabin][(BuildSoundType)strkey.ToEnum<BuildSoundType>()].Add(BuildingAudioClips[i]);
            }
            else if (!strkey.ToEnum<BuildSoundType>().Equals(-1) &&
                BuildSoundDic[CommonType.Cabin].ContainsKey((BuildSoundType)strkey.ToEnum<BuildSoundType>()))
            {
                BuildSoundDic[CommonType.Cabin][(BuildSoundType)strkey.ToEnum<BuildSoundType>()].Add(BuildingAudioClips[i]);
            }
        }

        BuildingAudioClips = Resources.LoadAll<AudioClip>("Sound/Pig");
        for (int i = 0; i < BuildingAudioClips.Length; ++i)
        {
            strkey = BuildingAudioClips[i].name.Substring(0, BuildingAudioClips[i].name.LastIndexOf('_'));
            strkey = strkey.Substring(strkey.LastIndexOf('_') + 1, strkey.Length - strkey.LastIndexOf('_') - 1);
            strkey = textInfo.ToTitleCase(strkey);

            if (!strkey.ToEnum<BuildSoundType>().Equals(-1) &&
                !BuildSoundDic[CommonType.Pig].ContainsKey((BuildSoundType)strkey.ToEnum<BuildSoundType>()))
            {
                BuildSoundDic[CommonType.Pig].Add((BuildSoundType)strkey.ToEnum<BuildSoundType>(), new List<AudioClip>());
                BuildSoundDic[CommonType.Pig][(BuildSoundType)strkey.ToEnum<BuildSoundType>()].Add(BuildingAudioClips[i]);
            }
            else if (!strkey.ToEnum<BuildSoundType>().Equals(-1) &&
                BuildSoundDic[CommonType.Pig].ContainsKey((BuildSoundType)strkey.ToEnum<BuildSoundType>()))
            {
                BuildSoundDic[CommonType.Pig][(BuildSoundType)strkey.ToEnum<BuildSoundType>()].Add(BuildingAudioClips[i]);
            }
        }

        BuildingAudioClips = Resources.LoadAll<AudioClip>("Sound/CampFire");
        for (int i = 0; i < BuildingAudioClips.Length; ++i)
        {
            strkey = BuildingAudioClips[i].name.Substring(0, BuildingAudioClips[i].name.LastIndexOf('_'));
            strkey = strkey.Substring(strkey.LastIndexOf('_') + 1, strkey.Length - strkey.LastIndexOf('_') - 1);
            strkey = textInfo.ToTitleCase(strkey);

            if (!strkey.ToEnum<BuildSoundType>().Equals(-1) &&
                !BuildSoundDic[CommonType.CampFire].ContainsKey((BuildSoundType)strkey.ToEnum<BuildSoundType>()))
            {
                BuildSoundDic[CommonType.CampFire].Add((BuildSoundType)strkey.ToEnum<BuildSoundType>(), new List<AudioClip>());
                BuildSoundDic[CommonType.CampFire][(BuildSoundType)strkey.ToEnum<BuildSoundType>()].Add(BuildingAudioClips[i]);
            }
            else if (!strkey.ToEnum<BuildSoundType>().Equals(-1) &&
                BuildSoundDic[CommonType.CampFire].ContainsKey((BuildSoundType)strkey.ToEnum<BuildSoundType>()))
            {
                BuildSoundDic[CommonType.CampFire][(BuildSoundType)strkey.ToEnum<BuildSoundType>()].Add(BuildingAudioClips[i]);
            }
        }
    }
   
    public void InitializeElement(int count, ref bool endflag)
    {
        AudioClip[] BackAudioClips;
        AudioClip[] LobbyAudioClips;
        AudioClip[] FootAudioClips;
        AudioClip[] EffectAudioClips;
        AudioClip[] ComAudioClips;
        AudioClip[] UnitAudioClips;
        AudioClip[] BuildingAudioClips;
        switch (count)
        {
            case 0:
                BackAudioClips = Resources.LoadAll<AudioClip>("Sound/Back/Bellafide");
                BackSoundDic.Add(Camp.Bellafide, BackAudioClips);
                break;
            case 1:
                BackAudioClips = Resources.LoadAll<AudioClip>("Sound/Back/Hopper");
                BackSoundDic.Add(Camp.Hopper, BackAudioClips);
                break;
            case 2:
                BackAudioClips = Resources.LoadAll<AudioClip>("Sound/Back/Archimedes");
                BackSoundDic.Add(Camp.Archimedes, BackAudioClips);
                break;
            case 3:
                BackAudioClips = Resources.LoadAll<AudioClip>("Sound/Back/Quartermaster");
                BackSoundDic.Add(Camp.Quartermaster, BackAudioClips);
                break;
            case 4:
                LobbyAudioClips = Resources.LoadAll<AudioClip>("Sound/Lobby");
                for (int i = 0; i < LobbyAudioClips.Length; ++i)
                {
                    strkey = LobbyAudioClips[i].name.Substring(0, LobbyAudioClips[i].name.LastIndexOf('_'));
                    strkey = strkey.Substring(strkey.LastIndexOf('_') + 1, strkey.Length - strkey.LastIndexOf('_') - 1);
                    strkey = textInfo.ToTitleCase(strkey);

                    if (!strkey.ToEnum<LobbySoundType>().Equals(-1) &&
                        !LobbySoundDic.ContainsKey((LobbySoundType)strkey.ToEnum<LobbySoundType>()))
                    {
                        LobbySoundDic.Add((LobbySoundType)strkey.ToEnum<LobbySoundType>(), new List<AudioClip>());
                        LobbySoundDic[(LobbySoundType)strkey.ToEnum<LobbySoundType>()].Add(LobbyAudioClips[i]);
                    }
                    else if (!strkey.ToEnum<UnitSoundType>().Equals(-1) &&
                        LobbySoundDic.ContainsKey((LobbySoundType)strkey.ToEnum<LobbySoundType>()))
                    {
                        LobbySoundDic[(LobbySoundType)strkey.ToEnum<LobbySoundType>()].Add(LobbyAudioClips[i]);
                    }
                }
                break;
            case 5:
                LobbyAudioClips = Resources.LoadAll<AudioClip>("Sound/Back/Lobby");
                LobbySoundDic.Add(LobbySoundType.Back, new List<AudioClip>());
                for (int i = 0; i < LobbyAudioClips.Length; ++i)
                    LobbySoundDic[LobbySoundType.Back].Add(LobbyAudioClips[i]);
                break;
            case 6:
                FootAudioClips = Resources.LoadAll<AudioClip>("Sound/Foot");
                for (int i = 0; i < FootAudioClips.Length; ++i)
                {
                    strkey = FootAudioClips[i].name.Substring(0, FootAudioClips[i].name.LastIndexOf('_'));
                    strkey = strkey.Substring(strkey.LastIndexOf('_') + 1, strkey.Length - strkey.LastIndexOf('_') - 1);
                    strkey = textInfo.ToTitleCase(strkey);

                    if (!strkey.ToEnum<FootSoundType>().Equals(-1) &&
                        !FootSoundDic.ContainsKey((FootSoundType)strkey.ToEnum<FootSoundType>()))
                    {
                        FootSoundDic.Add((FootSoundType)strkey.ToEnum<FootSoundType>(), new List<AudioClip>());
                        FootSoundDic[(FootSoundType)strkey.ToEnum<FootSoundType>()].Add(FootAudioClips[i]);
                    }
                    else if (!strkey.ToEnum<FootSoundType>().Equals(-1) &&
                        FootSoundDic.ContainsKey((FootSoundType)strkey.ToEnum<FootSoundType>()))
                    {
                        FootSoundDic[(FootSoundType)strkey.ToEnum<FootSoundType>()].Add(FootAudioClips[i]);
                    }
                }
                break;
            case 7:
                EffectAudioClips = Resources.LoadAll<AudioClip>("Sound/Effect");
                for (int i = 0; i < EffectAudioClips.Length; ++i)
                {
                    strkey = EffectAudioClips[i].name.Substring(0, EffectAudioClips[i].name.LastIndexOf('_'));
                    strkey = strkey.Substring(strkey.LastIndexOf('_') + 1, strkey.Length - strkey.LastIndexOf('_') - 1);
                    strkey = textInfo.ToTitleCase(strkey);

                    if (!strkey.ToEnum<EffectSoundType>().Equals(-1) &&
                        !EffectSoundDic.ContainsKey((EffectSoundType)strkey.ToEnum<EffectSoundType>()))
                    {
                        EffectSoundDic.Add((EffectSoundType)strkey.ToEnum<EffectSoundType>(), new List<AudioClip>());
                        EffectSoundDic[(EffectSoundType)strkey.ToEnum<EffectSoundType>()].Add(EffectAudioClips[i]);
                    }
                    else if (!strkey.ToEnum<EffectSoundType>().Equals(-1) &&
                        EffectSoundDic.ContainsKey((EffectSoundType)strkey.ToEnum<EffectSoundType>()))
                    {
                        EffectSoundDic[(EffectSoundType)strkey.ToEnum<EffectSoundType>()].Add(EffectAudioClips[i]);
                    }
                }
                break;
            case 8:
                ComSoundDic.Add(Camp.Bellafide, new Dictionary<ComSoundType, List<AudioClip>>());
                ComSoundDic.Add(Camp.Hopper, new Dictionary<ComSoundType, List<AudioClip>>());
                ComSoundDic.Add(Camp.Archimedes, new Dictionary<ComSoundType, List<AudioClip>>());
                ComSoundDic.Add(Camp.Quartermaster, new Dictionary<ComSoundType, List<AudioClip>>());
                break;
            case 9:
                ComAudioClips = Resources.LoadAll<AudioClip>("Sound/General");
                for (int i = 0; i < ComAudioClips.Length; ++i)
                {
                    strkey = ComAudioClips[i].name.Substring(0, ComAudioClips[i].name.LastIndexOf('_'));
                    strkey = strkey.Substring(strkey.LastIndexOf('_') + 1, strkey.Length - strkey.LastIndexOf('_') - 1);
                    strkey = textInfo.ToTitleCase(strkey);

                    if (!strkey.ToEnum<ComSoundType>().Equals(-1) &&
                        !ComSoundDic[Camp.Hopper].ContainsKey((ComSoundType)strkey.ToEnum<ComSoundType>()))
                    {
                        ComSoundDic[Camp.Hopper].Add((ComSoundType)strkey.ToEnum<ComSoundType>(), new List<AudioClip>());
                        ComSoundDic[Camp.Hopper][(ComSoundType)strkey.ToEnum<ComSoundType>()].Add(ComAudioClips[i]);
                        ComSoundDic[Camp.Bellafide].Add((ComSoundType)strkey.ToEnum<ComSoundType>(), new List<AudioClip>());
                        ComSoundDic[Camp.Bellafide][(ComSoundType)strkey.ToEnum<ComSoundType>()].Add(ComAudioClips[i]);
                        ComSoundDic[Camp.Archimedes].Add((ComSoundType)strkey.ToEnum<ComSoundType>(), new List<AudioClip>());
                        ComSoundDic[Camp.Archimedes][(ComSoundType)strkey.ToEnum<ComSoundType>()].Add(ComAudioClips[i]);
                        ComSoundDic[Camp.Quartermaster].Add((ComSoundType)strkey.ToEnum<ComSoundType>(), new List<AudioClip>());
                        ComSoundDic[Camp.Quartermaster][(ComSoundType)strkey.ToEnum<ComSoundType>()].Add(ComAudioClips[i]);
                    }
                    else if (!strkey.ToEnum<ComSoundType>().Equals(-1) &&
                        ComSoundDic[Camp.Hopper].ContainsKey((ComSoundType)strkey.ToEnum<ComSoundType>()))
                    {
                        ComSoundDic[Camp.Hopper][(ComSoundType)strkey.ToEnum<ComSoundType>()].Add(ComAudioClips[i]);
                        ComSoundDic[Camp.Bellafide][(ComSoundType)strkey.ToEnum<ComSoundType>()].Add(ComAudioClips[i]);
                        ComSoundDic[Camp.Archimedes][(ComSoundType)strkey.ToEnum<ComSoundType>()].Add(ComAudioClips[i]);
                        ComSoundDic[Camp.Quartermaster][(ComSoundType)strkey.ToEnum<ComSoundType>()].Add(ComAudioClips[i]);
                    }
                }
                break;
            case 10:
                ComAudioClips = Resources.LoadAll<AudioClip>("Sound/Hopper");
                for (int i = 0; i < ComAudioClips.Length; ++i)
                {
                    strkey = ComAudioClips[i].name.Substring(0, ComAudioClips[i].name.LastIndexOf('_'));
                    strkey = strkey.Substring(strkey.LastIndexOf('_') + 1, strkey.Length - strkey.LastIndexOf('_') - 1);
                    strkey = textInfo.ToTitleCase(strkey);

                    if (!strkey.ToEnum<ComSoundType>().Equals(-1) &&
                        !ComSoundDic[Camp.Hopper].ContainsKey((ComSoundType)strkey.ToEnum<ComSoundType>()))
                    {
                        ComSoundDic[Camp.Hopper].Add((ComSoundType)strkey.ToEnum<ComSoundType>(), new List<AudioClip>());
                        ComSoundDic[Camp.Hopper][(ComSoundType)strkey.ToEnum<ComSoundType>()].Add(ComAudioClips[i]);
                    }
                    else if (!strkey.ToEnum<ComSoundType>().Equals(-1) &&
                        ComSoundDic[Camp.Hopper].ContainsKey((ComSoundType)strkey.ToEnum<ComSoundType>()))
                    {
                        ComSoundDic[Camp.Hopper][(ComSoundType)strkey.ToEnum<ComSoundType>()].Add(ComAudioClips[i]);
                    }
                }
                break;
            case 11:
                ComAudioClips = Resources.LoadAll<AudioClip>("Sound/Bellafide");
                for (int i = 0; i < ComAudioClips.Length; ++i)
                {
                    strkey = ComAudioClips[i].name.Substring(0, ComAudioClips[i].name.LastIndexOf('_'));
                    strkey = strkey.Substring(strkey.LastIndexOf('_') + 1, strkey.Length - strkey.LastIndexOf('_') - 1);
                    strkey = textInfo.ToTitleCase(strkey);

                    if (!strkey.ToEnum<ComSoundType>().Equals(-1) &&
                        !ComSoundDic[Camp.Bellafide].ContainsKey((ComSoundType)strkey.ToEnum<ComSoundType>()))
                    {
                        ComSoundDic[Camp.Bellafide].Add((ComSoundType)strkey.ToEnum<ComSoundType>(), new List<AudioClip>());
                        ComSoundDic[Camp.Bellafide][(ComSoundType)strkey.ToEnum<ComSoundType>()].Add(ComAudioClips[i]);
                    }
                    else if (!strkey.ToEnum<ComSoundType>().Equals(-1) &&
                        ComSoundDic[Camp.Bellafide].ContainsKey((ComSoundType)strkey.ToEnum<ComSoundType>()))
                    {
                        ComSoundDic[Camp.Bellafide][(ComSoundType)strkey.ToEnum<ComSoundType>()].Add(ComAudioClips[i]);
                    }
                }
                break;
            case 12:
                ComAudioClips = Resources.LoadAll<AudioClip>("Sound/Archimedes");
                for (int i = 0; i < ComAudioClips.Length; ++i)
                {
                    strkey = ComAudioClips[i].name.Substring(0, ComAudioClips[i].name.LastIndexOf('_'));
                    strkey = strkey.Substring(strkey.LastIndexOf('_') + 1, strkey.Length - strkey.LastIndexOf('_') - 1);
                    strkey = textInfo.ToTitleCase(strkey);

                    if (!strkey.ToEnum<ComSoundType>().Equals(-1) &&
                        !ComSoundDic[Camp.Archimedes].ContainsKey((ComSoundType)strkey.ToEnum<ComSoundType>()))
                    {
                        ComSoundDic[Camp.Archimedes].Add((ComSoundType)strkey.ToEnum<ComSoundType>(), new List<AudioClip>());
                        ComSoundDic[Camp.Archimedes][(ComSoundType)strkey.ToEnum<ComSoundType>()].Add(ComAudioClips[i]);
                    }
                    else if (!strkey.ToEnum<ComSoundType>().Equals(-1) &&
                        ComSoundDic[Camp.Archimedes].ContainsKey((ComSoundType)strkey.ToEnum<ComSoundType>()))
                    {
                        ComSoundDic[Camp.Archimedes][(ComSoundType)strkey.ToEnum<ComSoundType>()].Add(ComAudioClips[i]);
                    }
                }
                break;
            case 13:
                ComAudioClips = Resources.LoadAll<AudioClip>("Sound/Quartermaster");
                for (int i = 0; i < ComAudioClips.Length; ++i)
                {
                    strkey = ComAudioClips[i].name.Substring(0, ComAudioClips[i].name.LastIndexOf('_'));
                    strkey = strkey.Substring(strkey.LastIndexOf('_') + 1, strkey.Length - strkey.LastIndexOf('_') - 1);
                    strkey = textInfo.ToTitleCase(strkey);

                    if (!strkey.ToEnum<ComSoundType>().Equals(-1) &&
                        !ComSoundDic[Camp.Quartermaster].ContainsKey((ComSoundType)strkey.ToEnum<ComSoundType>()))
                    {
                        ComSoundDic[Camp.Quartermaster].Add((ComSoundType)strkey.ToEnum<ComSoundType>(), new List<AudioClip>());
                        ComSoundDic[Camp.Quartermaster][(ComSoundType)strkey.ToEnum<ComSoundType>()].Add(ComAudioClips[i]);
                    }
                    else if (!strkey.ToEnum<ComSoundType>().Equals(-1) &&
                        ComSoundDic[Camp.Quartermaster].ContainsKey((ComSoundType)strkey.ToEnum<ComSoundType>()))
                    {
                        ComSoundDic[Camp.Quartermaster][(ComSoundType)strkey.ToEnum<ComSoundType>()].Add(ComAudioClips[i]);
                    }
                }
                break;
            case 14:
                UnitSoundDic.Add(CommonType.Squirrel, new Dictionary<UnitSoundType, List<AudioClip>>());
                UnitSoundDic.Add(CommonType.Lizard, new Dictionary<UnitSoundType, List<AudioClip>>());
                UnitSoundDic.Add(CommonType.Toad, new Dictionary<UnitSoundType, List<AudioClip>>());
                UnitSoundDic.Add(CommonType.Pigeon, new Dictionary<UnitSoundType, List<AudioClip>>());
                UnitSoundDic.Add(CommonType.Mole, new Dictionary<UnitSoundType, List<AudioClip>>());
                UnitSoundDic.Add(CommonType.Mouse, new Dictionary<UnitSoundType, List<AudioClip>>());
                UnitSoundDic.Add(CommonType.Ferret, new Dictionary<UnitSoundType, List<AudioClip>>());
                UnitSoundDic.Add(CommonType.Falcon, new Dictionary<UnitSoundType, List<AudioClip>>());
                UnitSoundDic.Add(CommonType.Skunk, new Dictionary<UnitSoundType, List<AudioClip>>());
                UnitSoundDic.Add(CommonType.Chameleon, new Dictionary<UnitSoundType, List<AudioClip>>());
                UnitSoundDic.Add(CommonType.Snake, new Dictionary<UnitSoundType, List<AudioClip>>());
                UnitSoundDic.Add(CommonType.Boar, new Dictionary<UnitSoundType, List<AudioClip>>());
                UnitSoundDic.Add(CommonType.Badger, new Dictionary<UnitSoundType, List<AudioClip>>());
                UnitSoundDic.Add(CommonType.Owl, new Dictionary<UnitSoundType, List<AudioClip>>());
                UnitSoundDic.Add(CommonType.Wolf, new Dictionary<UnitSoundType, List<AudioClip>>());
                UnitSoundDic.Add(CommonType.Fox, new Dictionary<UnitSoundType, List<AudioClip>>());
                break;
            case 15:
                UnitAudioClips = Resources.LoadAll<AudioClip>("Sound/Squirrel");
                for (int i = 0; i < UnitAudioClips.Length; ++i)
                {
                    strkey = UnitAudioClips[i].name.Substring(0, UnitAudioClips[i].name.LastIndexOf('_'));
                    strkey = strkey.Substring(strkey.LastIndexOf('_') + 1, strkey.Length - strkey.LastIndexOf('_') - 1);
                    strkey = textInfo.ToTitleCase(strkey);

                    if (!strkey.ToEnum<UnitSoundType>().Equals(-1) &&
                        !UnitSoundDic[CommonType.Squirrel].ContainsKey((UnitSoundType)strkey.ToEnum<UnitSoundType>()))
                    {
                        UnitSoundDic[CommonType.Squirrel].Add((UnitSoundType)strkey.ToEnum<UnitSoundType>(), new List<AudioClip>());
                        UnitSoundDic[CommonType.Squirrel][(UnitSoundType)strkey.ToEnum<UnitSoundType>()].Add(UnitAudioClips[i]);
                    }
                    else if (!strkey.ToEnum<UnitSoundType>().Equals(-1) &&
                        UnitSoundDic[CommonType.Squirrel].ContainsKey((UnitSoundType)strkey.ToEnum<UnitSoundType>()))
                    {
                        UnitSoundDic[CommonType.Squirrel][(UnitSoundType)strkey.ToEnum<UnitSoundType>()].Add(UnitAudioClips[i]);
                    }
                }
                break;
            case 16:
                UnitAudioClips = Resources.LoadAll<AudioClip>("Sound/Lizard");
                for (int i = 0; i < UnitAudioClips.Length; ++i)
                {
                    strkey = UnitAudioClips[i].name.Substring(0, UnitAudioClips[i].name.LastIndexOf('_'));
                    strkey = strkey.Substring(strkey.LastIndexOf('_') + 1, strkey.Length - strkey.LastIndexOf('_') - 1);
                    strkey = textInfo.ToTitleCase(strkey);

                    if (!strkey.ToEnum<UnitSoundType>().Equals(-1) &&
                        !UnitSoundDic[CommonType.Lizard].ContainsKey((UnitSoundType)strkey.ToEnum<UnitSoundType>()))
                    {
                        UnitSoundDic[CommonType.Lizard].Add((UnitSoundType)strkey.ToEnum<UnitSoundType>(), new List<AudioClip>());
                        UnitSoundDic[CommonType.Lizard][(UnitSoundType)strkey.ToEnum<UnitSoundType>()].Add(UnitAudioClips[i]);
                    }
                    else if (!strkey.ToEnum<UnitSoundType>().Equals(-1) &&
                        UnitSoundDic[CommonType.Lizard].ContainsKey((UnitSoundType)strkey.ToEnum<UnitSoundType>()))
                    {
                        UnitSoundDic[CommonType.Lizard][(UnitSoundType)strkey.ToEnum<UnitSoundType>()].Add(UnitAudioClips[i]);
                    }
                }
                break;
            case 17:
                UnitAudioClips = Resources.LoadAll<AudioClip>("Sound/Toad");
                for (int i = 0; i < UnitAudioClips.Length; ++i)
                {
                    strkey = UnitAudioClips[i].name.Substring(0, UnitAudioClips[i].name.LastIndexOf('_'));
                    strkey = strkey.Substring(strkey.LastIndexOf('_') + 1, strkey.Length - strkey.LastIndexOf('_') - 1);
                    strkey = textInfo.ToTitleCase(strkey);

                    if (!strkey.ToEnum<UnitSoundType>().Equals(-1) &&
                        !UnitSoundDic[CommonType.Toad].ContainsKey((UnitSoundType)strkey.ToEnum<UnitSoundType>()))
                    {
                        UnitSoundDic[CommonType.Toad].Add((UnitSoundType)strkey.ToEnum<UnitSoundType>(), new List<AudioClip>());
                        UnitSoundDic[CommonType.Toad][(UnitSoundType)strkey.ToEnum<UnitSoundType>()].Add(UnitAudioClips[i]);
                    }
                    else if (!strkey.ToEnum<UnitSoundType>().Equals(-1) &&
                        UnitSoundDic[CommonType.Toad].ContainsKey((UnitSoundType)strkey.ToEnum<UnitSoundType>()))
                    {
                        UnitSoundDic[CommonType.Toad][(UnitSoundType)strkey.ToEnum<UnitSoundType>()].Add(UnitAudioClips[i]);
                    }
                }
                break;
            case 18:
                UnitAudioClips = Resources.LoadAll<AudioClip>("Sound/Pigeon");
                for (int i = 0; i < UnitAudioClips.Length; ++i)
                {
                    strkey = UnitAudioClips[i].name.Substring(0, UnitAudioClips[i].name.LastIndexOf('_'));
                    strkey = strkey.Substring(strkey.LastIndexOf('_') + 1, strkey.Length - strkey.LastIndexOf('_') - 1);
                    strkey = textInfo.ToTitleCase(strkey);

                    if (!strkey.ToEnum<UnitSoundType>().Equals(-1) &&
                        !UnitSoundDic[CommonType.Pigeon].ContainsKey((UnitSoundType)strkey.ToEnum<UnitSoundType>()))
                    {
                        UnitSoundDic[CommonType.Pigeon].Add((UnitSoundType)strkey.ToEnum<UnitSoundType>(), new List<AudioClip>());
                        UnitSoundDic[CommonType.Pigeon][(UnitSoundType)strkey.ToEnum<UnitSoundType>()].Add(UnitAudioClips[i]);
                    }
                    else if (!strkey.ToEnum<UnitSoundType>().Equals(-1) &&
                        UnitSoundDic[CommonType.Pigeon].ContainsKey((UnitSoundType)strkey.ToEnum<UnitSoundType>()))
                    {
                        UnitSoundDic[CommonType.Pigeon][(UnitSoundType)strkey.ToEnum<UnitSoundType>()].Add(UnitAudioClips[i]);
                    }
                }
                break;
            case 19:
                UnitAudioClips = Resources.LoadAll<AudioClip>("Sound/Mole");
                for (int i = 0; i < UnitAudioClips.Length; ++i)
                {
                    strkey = UnitAudioClips[i].name.Substring(0, UnitAudioClips[i].name.LastIndexOf('_'));
                    strkey = strkey.Substring(strkey.LastIndexOf('_') + 1, strkey.Length - strkey.LastIndexOf('_') - 1);
                    strkey = textInfo.ToTitleCase(strkey);

                    if (!strkey.ToEnum<UnitSoundType>().Equals(-1) &&
                        !UnitSoundDic[CommonType.Mole].ContainsKey((UnitSoundType)strkey.ToEnum<UnitSoundType>()))
                    {
                        UnitSoundDic[CommonType.Mole].Add((UnitSoundType)strkey.ToEnum<UnitSoundType>(), new List<AudioClip>());
                        UnitSoundDic[CommonType.Mole][(UnitSoundType)strkey.ToEnum<UnitSoundType>()].Add(UnitAudioClips[i]);
                    }
                    else if (!strkey.ToEnum<UnitSoundType>().Equals(-1) &&
                        UnitSoundDic[CommonType.Mole].ContainsKey((UnitSoundType)strkey.ToEnum<UnitSoundType>()))
                    {
                        UnitSoundDic[CommonType.Mole][(UnitSoundType)strkey.ToEnum<UnitSoundType>()].Add(UnitAudioClips[i]);
                    }
                }
                break;
            case 20:
                UnitAudioClips = Resources.LoadAll<AudioClip>("Sound/Ferret");
                for (int i = 0; i < UnitAudioClips.Length; ++i)
                {
                    strkey = UnitAudioClips[i].name.Substring(0, UnitAudioClips[i].name.LastIndexOf('_'));
                    strkey = strkey.Substring(strkey.LastIndexOf('_') + 1, strkey.Length - strkey.LastIndexOf('_') - 1);
                    strkey = textInfo.ToTitleCase(strkey);

                    if (!strkey.ToEnum<UnitSoundType>().Equals(-1) &&
                        !UnitSoundDic[CommonType.Ferret].ContainsKey((UnitSoundType)strkey.ToEnum<UnitSoundType>()))
                    {
                        UnitSoundDic[CommonType.Ferret].Add((UnitSoundType)strkey.ToEnum<UnitSoundType>(), new List<AudioClip>());
                        UnitSoundDic[CommonType.Ferret][(UnitSoundType)strkey.ToEnum<UnitSoundType>()].Add(UnitAudioClips[i]);
                    }
                    else if (!strkey.ToEnum<UnitSoundType>().Equals(-1) &&
                        UnitSoundDic[CommonType.Ferret].ContainsKey((UnitSoundType)strkey.ToEnum<UnitSoundType>()))
                    {
                        UnitSoundDic[CommonType.Ferret][(UnitSoundType)strkey.ToEnum<UnitSoundType>()].Add(UnitAudioClips[i]);
                    }
                }
                break;
            case 21:
                UnitAudioClips = Resources.LoadAll<AudioClip>("Sound/Falcon");
                for (int i = 0; i < UnitAudioClips.Length; ++i)
                {
                    strkey = UnitAudioClips[i].name.Substring(0, UnitAudioClips[i].name.LastIndexOf('_'));
                    strkey = strkey.Substring(strkey.LastIndexOf('_') + 1, strkey.Length - strkey.LastIndexOf('_') - 1);
                    strkey = textInfo.ToTitleCase(strkey);

                    if (!strkey.ToEnum<UnitSoundType>().Equals(-1) &&
                        !UnitSoundDic[CommonType.Falcon].ContainsKey((UnitSoundType)strkey.ToEnum<UnitSoundType>()))
                    {
                        UnitSoundDic[CommonType.Falcon].Add((UnitSoundType)strkey.ToEnum<UnitSoundType>(), new List<AudioClip>());
                        UnitSoundDic[CommonType.Falcon][(UnitSoundType)strkey.ToEnum<UnitSoundType>()].Add(UnitAudioClips[i]);
                    }
                    else if (!strkey.ToEnum<UnitSoundType>().Equals(-1) &&
                        UnitSoundDic[CommonType.Falcon].ContainsKey((UnitSoundType)strkey.ToEnum<UnitSoundType>()))
                    {
                        UnitSoundDic[CommonType.Falcon][(UnitSoundType)strkey.ToEnum<UnitSoundType>()].Add(UnitAudioClips[i]);
                    }
                }
                break;
            case 22:
                UnitAudioClips = Resources.LoadAll<AudioClip>("Sound/Skunk");
                for (int i = 0; i < UnitAudioClips.Length; ++i)
                {
                    strkey = UnitAudioClips[i].name.Substring(0, UnitAudioClips[i].name.LastIndexOf('_'));
                    strkey = strkey.Substring(strkey.LastIndexOf('_') + 1, strkey.Length - strkey.LastIndexOf('_') - 1);
                    strkey = textInfo.ToTitleCase(strkey);

                    if (!strkey.ToEnum<UnitSoundType>().Equals(-1) &&
                        !UnitSoundDic[CommonType.Skunk].ContainsKey((UnitSoundType)strkey.ToEnum<UnitSoundType>()))
                    {
                        UnitSoundDic[CommonType.Skunk].Add((UnitSoundType)strkey.ToEnum<UnitSoundType>(), new List<AudioClip>());
                        UnitSoundDic[CommonType.Skunk][(UnitSoundType)strkey.ToEnum<UnitSoundType>()].Add(UnitAudioClips[i]);
                    }
                    else if (!strkey.ToEnum<UnitSoundType>().Equals(-1) &&
                        UnitSoundDic[CommonType.Skunk].ContainsKey((UnitSoundType)strkey.ToEnum<UnitSoundType>()))
                    {
                        UnitSoundDic[CommonType.Skunk][(UnitSoundType)strkey.ToEnum<UnitSoundType>()].Add(UnitAudioClips[i]);
                    }
                }
                break;
            case 23:
                UnitAudioClips = Resources.LoadAll<AudioClip>("Sound/Chameleon");
                for (int i = 0; i < UnitAudioClips.Length; ++i)
                {
                    strkey = UnitAudioClips[i].name.Substring(0, UnitAudioClips[i].name.LastIndexOf('_'));
                    strkey = strkey.Substring(strkey.LastIndexOf('_') + 1, strkey.Length - strkey.LastIndexOf('_') - 1);
                    strkey = textInfo.ToTitleCase(strkey);

                    if (!strkey.ToEnum<UnitSoundType>().Equals(-1) &&
                        !UnitSoundDic[CommonType.Chameleon].ContainsKey((UnitSoundType)strkey.ToEnum<UnitSoundType>()))
                    {
                        UnitSoundDic[CommonType.Chameleon].Add((UnitSoundType)strkey.ToEnum<UnitSoundType>(), new List<AudioClip>());
                        UnitSoundDic[CommonType.Chameleon][(UnitSoundType)strkey.ToEnum<UnitSoundType>()].Add(UnitAudioClips[i]);
                    }
                    else if (!strkey.ToEnum<UnitSoundType>().Equals(-1) &&
                        UnitSoundDic[CommonType.Chameleon].ContainsKey((UnitSoundType)strkey.ToEnum<UnitSoundType>()))
                    {
                        UnitSoundDic[CommonType.Chameleon][(UnitSoundType)strkey.ToEnum<UnitSoundType>()].Add(UnitAudioClips[i]);
                    }
                }
                break;
            case 24:
                UnitAudioClips = Resources.LoadAll<AudioClip>("Sound/Snake");
                for (int i = 0; i < UnitAudioClips.Length; ++i)
                {
                    strkey = UnitAudioClips[i].name.Substring(0, UnitAudioClips[i].name.LastIndexOf('_'));
                    strkey = strkey.Substring(strkey.LastIndexOf('_') + 1, strkey.Length - strkey.LastIndexOf('_') - 1);
                    strkey = textInfo.ToTitleCase(strkey);

                    if (!strkey.ToEnum<UnitSoundType>().Equals(-1) &&
                        !UnitSoundDic[CommonType.Snake].ContainsKey((UnitSoundType)strkey.ToEnum<UnitSoundType>()))
                    {
                        UnitSoundDic[CommonType.Snake].Add((UnitSoundType)strkey.ToEnum<UnitSoundType>(), new List<AudioClip>());
                        UnitSoundDic[CommonType.Snake][(UnitSoundType)strkey.ToEnum<UnitSoundType>()].Add(UnitAudioClips[i]);
                    }
                    else if (!strkey.ToEnum<UnitSoundType>().Equals(-1) &&
                        UnitSoundDic[CommonType.Snake].ContainsKey((UnitSoundType)strkey.ToEnum<UnitSoundType>()))
                    {
                        UnitSoundDic[CommonType.Snake][(UnitSoundType)strkey.ToEnum<UnitSoundType>()].Add(UnitAudioClips[i]);
                    }
                }
                break;
            case 25:
                UnitAudioClips = Resources.LoadAll<AudioClip>("Sound/Boar");
                for (int i = 0; i < UnitAudioClips.Length; ++i)
                {
                    strkey = UnitAudioClips[i].name.Substring(0, UnitAudioClips[i].name.LastIndexOf('_'));
                    strkey = strkey.Substring(strkey.LastIndexOf('_') + 1, strkey.Length - strkey.LastIndexOf('_') - 1);
                    strkey = textInfo.ToTitleCase(strkey);

                    if (!strkey.ToEnum<UnitSoundType>().Equals(-1) &&
                        !UnitSoundDic[CommonType.Boar].ContainsKey((UnitSoundType)strkey.ToEnum<UnitSoundType>()))
                    {
                        UnitSoundDic[CommonType.Boar].Add((UnitSoundType)strkey.ToEnum<UnitSoundType>(), new List<AudioClip>());
                        UnitSoundDic[CommonType.Boar][(UnitSoundType)strkey.ToEnum<UnitSoundType>()].Add(UnitAudioClips[i]);
                    }
                    else if (!strkey.ToEnum<UnitSoundType>().Equals(-1) &&
                        UnitSoundDic[CommonType.Boar].ContainsKey((UnitSoundType)strkey.ToEnum<UnitSoundType>()))
                    {
                        UnitSoundDic[CommonType.Boar][(UnitSoundType)strkey.ToEnum<UnitSoundType>()].Add(UnitAudioClips[i]);
                    }
                }
                break;
            case 26:
                UnitAudioClips = Resources.LoadAll<AudioClip>("Sound/Badger");
                for (int i = 0; i < UnitAudioClips.Length; ++i)
                {
                    strkey = UnitAudioClips[i].name.Substring(0, UnitAudioClips[i].name.LastIndexOf('_'));
                    strkey = strkey.Substring(strkey.LastIndexOf('_') + 1, strkey.Length - strkey.LastIndexOf('_') - 1);
                    strkey = textInfo.ToTitleCase(strkey);

                    if (!strkey.ToEnum<UnitSoundType>().Equals(-1) &&
                        !UnitSoundDic[CommonType.Badger].ContainsKey((UnitSoundType)strkey.ToEnum<UnitSoundType>()))
                    {
                        UnitSoundDic[CommonType.Badger].Add((UnitSoundType)strkey.ToEnum<UnitSoundType>(), new List<AudioClip>());
                        UnitSoundDic[CommonType.Badger][(UnitSoundType)strkey.ToEnum<UnitSoundType>()].Add(UnitAudioClips[i]);
                    }
                    else if (!strkey.ToEnum<UnitSoundType>().Equals(-1) &&
                        UnitSoundDic[CommonType.Badger].ContainsKey((UnitSoundType)strkey.ToEnum<UnitSoundType>()))
                    {
                        UnitSoundDic[CommonType.Badger][(UnitSoundType)strkey.ToEnum<UnitSoundType>()].Add(UnitAudioClips[i]);
                    }
                }
                break;
            case 27:
                UnitAudioClips = Resources.LoadAll<AudioClip>("Sound/Owl");
                for (int i = 0; i < UnitAudioClips.Length; ++i)
                {
                    strkey = UnitAudioClips[i].name.Substring(0, UnitAudioClips[i].name.LastIndexOf('_'));
                    strkey = strkey.Substring(strkey.LastIndexOf('_') + 1, strkey.Length - strkey.LastIndexOf('_') - 1);
                    strkey = textInfo.ToTitleCase(strkey);

                    if (!strkey.ToEnum<UnitSoundType>().Equals(-1) &&
                        !UnitSoundDic[CommonType.Owl].ContainsKey((UnitSoundType)strkey.ToEnum<UnitSoundType>()))
                    {
                        UnitSoundDic[CommonType.Owl].Add((UnitSoundType)strkey.ToEnum<UnitSoundType>(), new List<AudioClip>());
                        UnitSoundDic[CommonType.Owl][(UnitSoundType)strkey.ToEnum<UnitSoundType>()].Add(UnitAudioClips[i]);
                    }
                    else if (!strkey.ToEnum<UnitSoundType>().Equals(-1) &&
                        UnitSoundDic[CommonType.Owl].ContainsKey((UnitSoundType)strkey.ToEnum<UnitSoundType>()))
                    {
                        UnitSoundDic[CommonType.Owl][(UnitSoundType)strkey.ToEnum<UnitSoundType>()].Add(UnitAudioClips[i]);
                    }
                }
                break;
            case 28:
                UnitAudioClips = Resources.LoadAll<AudioClip>("Sound/Wolf");
                for (int i = 0; i < UnitAudioClips.Length; ++i)
                {
                    strkey = UnitAudioClips[i].name.Substring(0, UnitAudioClips[i].name.LastIndexOf('_'));
                    strkey = strkey.Substring(strkey.LastIndexOf('_') + 1, strkey.Length - strkey.LastIndexOf('_') - 1);
                    strkey = textInfo.ToTitleCase(strkey);

                    if (!strkey.ToEnum<UnitSoundType>().Equals(-1) &&
                        !UnitSoundDic[CommonType.Wolf].ContainsKey((UnitSoundType)strkey.ToEnum<UnitSoundType>()))
                    {
                        UnitSoundDic[CommonType.Wolf].Add((UnitSoundType)strkey.ToEnum<UnitSoundType>(), new List<AudioClip>());
                        UnitSoundDic[CommonType.Wolf][(UnitSoundType)strkey.ToEnum<UnitSoundType>()].Add(UnitAudioClips[i]);
                    }
                    else if (!strkey.ToEnum<UnitSoundType>().Equals(-1) &&
                        UnitSoundDic[CommonType.Wolf].ContainsKey((UnitSoundType)strkey.ToEnum<UnitSoundType>()))
                    {
                        UnitSoundDic[CommonType.Wolf][(UnitSoundType)strkey.ToEnum<UnitSoundType>()].Add(UnitAudioClips[i]);
                    }
                }
                break;
            case 29:
                UnitAudioClips = Resources.LoadAll<AudioClip>("Sound/Fox");
                for (int i = 0; i < UnitAudioClips.Length; ++i)
                {
                    strkey = UnitAudioClips[i].name.Substring(0, UnitAudioClips[i].name.LastIndexOf('_'));
                    strkey = strkey.Substring(strkey.LastIndexOf('_') + 1, strkey.Length - strkey.LastIndexOf('_') - 1);
                    strkey = textInfo.ToTitleCase(strkey);

                    if (!strkey.ToEnum<UnitSoundType>().Equals(-1) &&
                        !UnitSoundDic[CommonType.Fox].ContainsKey((UnitSoundType)strkey.ToEnum<UnitSoundType>()))
                    {
                        UnitSoundDic[CommonType.Fox].Add((UnitSoundType)strkey.ToEnum<UnitSoundType>(), new List<AudioClip>());
                        UnitSoundDic[CommonType.Fox][(UnitSoundType)strkey.ToEnum<UnitSoundType>()].Add(UnitAudioClips[i]);
                    }
                    else if (!strkey.ToEnum<UnitSoundType>().Equals(-1) &&
                        UnitSoundDic[CommonType.Fox].ContainsKey((UnitSoundType)strkey.ToEnum<UnitSoundType>()))
                    {
                        UnitSoundDic[CommonType.Fox][(UnitSoundType)strkey.ToEnum<UnitSoundType>()].Add(UnitAudioClips[i]);
                    }
                }
                break;
            case 30:
                UnitAudioClips = Resources.LoadAll<AudioClip>("Sound/Mouse");
                for (int i = 0; i < UnitAudioClips.Length; ++i)
                {
                    strkey = UnitAudioClips[i].name.Substring(0, UnitAudioClips[i].name.LastIndexOf('_'));
                    strkey = strkey.Substring(strkey.LastIndexOf('_') + 1, strkey.Length - strkey.LastIndexOf('_') - 1);
                    strkey = textInfo.ToTitleCase(strkey);

                    if (!strkey.ToEnum<UnitSoundType>().Equals(-1) &&
                        !UnitSoundDic[CommonType.Mouse].ContainsKey((UnitSoundType)strkey.ToEnum<UnitSoundType>()))
                    {
                        UnitSoundDic[CommonType.Mouse].Add((UnitSoundType)strkey.ToEnum<UnitSoundType>(), new List<AudioClip>());
                        UnitSoundDic[CommonType.Mouse][(UnitSoundType)strkey.ToEnum<UnitSoundType>()].Add(UnitAudioClips[i]);
                    }
                    else if (!strkey.ToEnum<UnitSoundType>().Equals(-1) &&
                        UnitSoundDic[CommonType.Mouse].ContainsKey((UnitSoundType)strkey.ToEnum<UnitSoundType>()))
                    {
                        UnitSoundDic[CommonType.Mouse][(UnitSoundType)strkey.ToEnum<UnitSoundType>()].Add(UnitAudioClips[i]);
                    }
                }
                break;
            case 31:
                BuildSoundDic.Add(CommonType.Wire, new Dictionary<BuildSoundType, List<AudioClip>>());
                BuildSoundDic.Add(CommonType.Mine, new Dictionary<BuildSoundType, List<AudioClip>>());
                BuildSoundDic.Add(CommonType.Turret, new Dictionary<BuildSoundType, List<AudioClip>>());
                BuildSoundDic.Add(CommonType.Balloon, new Dictionary<BuildSoundType, List<AudioClip>>());
                BuildSoundDic.Add(CommonType.Cannon, new Dictionary<BuildSoundType, List<AudioClip>>());
                BuildSoundDic.Add(CommonType.Gristmill, new Dictionary<BuildSoundType, List<AudioClip>>());
                BuildSoundDic.Add(CommonType.WarrenT1, new Dictionary<BuildSoundType, List<AudioClip>>());
                BuildSoundDic.Add(CommonType.WarrenT2, new Dictionary<BuildSoundType, List<AudioClip>>());
                BuildSoundDic.Add(CommonType.WarrenT3, new Dictionary<BuildSoundType, List<AudioClip>>());
                BuildSoundDic.Add(CommonType.Cabin, new Dictionary<BuildSoundType, List<AudioClip>>());
                BuildSoundDic.Add(CommonType.MoleeMerge, new Dictionary<BuildSoundType, List<AudioClip>>());
                BuildSoundDic.Add(CommonType.Pig, new Dictionary<BuildSoundType, List<AudioClip>>());
                BuildSoundDic.Add(CommonType.Farm, new Dictionary<BuildSoundType, List<AudioClip>>());
                BuildSoundDic.Add(CommonType.CampFire, new Dictionary<BuildSoundType, List<AudioClip>>());
                break;
            case 32:
                BuildingAudioClips = Resources.LoadAll<AudioClip>("Sound/General");
                for (int i = 0; i < BuildingAudioClips.Length; ++i)
                {
                    strkey = BuildingAudioClips[i].name.Substring(0, BuildingAudioClips[i].name.LastIndexOf('_'));
                    strkey = strkey.Substring(strkey.LastIndexOf('_') + 1, strkey.Length - strkey.LastIndexOf('_') - 1);
                    strkey = textInfo.ToTitleCase(strkey);

                    if (!strkey.ToEnum<BuildSoundType>().Equals(-1) &&
                        !BuildSoundDic[CommonType.Wire].ContainsKey((BuildSoundType)strkey.ToEnum<BuildSoundType>()))
                    {
                        BuildSoundDic[CommonType.Wire].Add((BuildSoundType)strkey.ToEnum<BuildSoundType>(), new List<AudioClip>());
                        BuildSoundDic[CommonType.Wire][(BuildSoundType)strkey.ToEnum<BuildSoundType>()].Add(BuildingAudioClips[i]);
                        BuildSoundDic[CommonType.Mine].Add((BuildSoundType)strkey.ToEnum<BuildSoundType>(), new List<AudioClip>());
                        BuildSoundDic[CommonType.Mine][(BuildSoundType)strkey.ToEnum<BuildSoundType>()].Add(BuildingAudioClips[i]);
                        BuildSoundDic[CommonType.Turret].Add((BuildSoundType)strkey.ToEnum<BuildSoundType>(), new List<AudioClip>());
                        BuildSoundDic[CommonType.Turret][(BuildSoundType)strkey.ToEnum<BuildSoundType>()].Add(BuildingAudioClips[i]);
                        BuildSoundDic[CommonType.Balloon].Add((BuildSoundType)strkey.ToEnum<BuildSoundType>(), new List<AudioClip>());
                        BuildSoundDic[CommonType.Balloon][(BuildSoundType)strkey.ToEnum<BuildSoundType>()].Add(BuildingAudioClips[i]);
                        BuildSoundDic[CommonType.Cannon].Add((BuildSoundType)strkey.ToEnum<BuildSoundType>(), new List<AudioClip>());
                        BuildSoundDic[CommonType.Cannon][(BuildSoundType)strkey.ToEnum<BuildSoundType>()].Add(BuildingAudioClips[i]);
                        BuildSoundDic[CommonType.WarrenT1].Add((BuildSoundType)strkey.ToEnum<BuildSoundType>(), new List<AudioClip>());
                        BuildSoundDic[CommonType.WarrenT1][(BuildSoundType)strkey.ToEnum<BuildSoundType>()].Add(BuildingAudioClips[i]);
                        BuildSoundDic[CommonType.WarrenT2].Add((BuildSoundType)strkey.ToEnum<BuildSoundType>(), new List<AudioClip>());
                        BuildSoundDic[CommonType.WarrenT2][(BuildSoundType)strkey.ToEnum<BuildSoundType>()].Add(BuildingAudioClips[i]);
                        BuildSoundDic[CommonType.WarrenT3].Add((BuildSoundType)strkey.ToEnum<BuildSoundType>(), new List<AudioClip>());
                        BuildSoundDic[CommonType.WarrenT3][(BuildSoundType)strkey.ToEnum<BuildSoundType>()].Add(BuildingAudioClips[i]);
                        BuildSoundDic[CommonType.MoleeMerge].Add((BuildSoundType)strkey.ToEnum<BuildSoundType>(), new List<AudioClip>());
                        BuildSoundDic[CommonType.MoleeMerge][(BuildSoundType)strkey.ToEnum<BuildSoundType>()].Add(BuildingAudioClips[i]);
                    }
                    else if (!strkey.ToEnum<BuildSoundType>().Equals(-1) &&
                        BuildSoundDic[CommonType.Wire].ContainsKey((BuildSoundType)strkey.ToEnum<BuildSoundType>()))
                    {
                        BuildSoundDic[CommonType.Wire][(BuildSoundType)strkey.ToEnum<BuildSoundType>()].Add(BuildingAudioClips[i]);
                        BuildSoundDic[CommonType.Mine][(BuildSoundType)strkey.ToEnum<BuildSoundType>()].Add(BuildingAudioClips[i]);
                        BuildSoundDic[CommonType.Turret][(BuildSoundType)strkey.ToEnum<BuildSoundType>()].Add(BuildingAudioClips[i]);
                        BuildSoundDic[CommonType.Balloon][(BuildSoundType)strkey.ToEnum<BuildSoundType>()].Add(BuildingAudioClips[i]);
                        BuildSoundDic[CommonType.Cannon][(BuildSoundType)strkey.ToEnum<BuildSoundType>()].Add(BuildingAudioClips[i]);
                        BuildSoundDic[CommonType.WarrenT1][(BuildSoundType)strkey.ToEnum<BuildSoundType>()].Add(BuildingAudioClips[i]);
                        BuildSoundDic[CommonType.WarrenT2][(BuildSoundType)strkey.ToEnum<BuildSoundType>()].Add(BuildingAudioClips[i]);
                        BuildSoundDic[CommonType.WarrenT3][(BuildSoundType)strkey.ToEnum<BuildSoundType>()].Add(BuildingAudioClips[i]);
                        BuildSoundDic[CommonType.MoleeMerge][(BuildSoundType)strkey.ToEnum<BuildSoundType>()].Add(BuildingAudioClips[i]);
                    }
                }
                break;
            case 33:
                BuildingAudioClips = Resources.LoadAll<AudioClip>("Sound/Wire/Complete");
                BuildSoundDic[CommonType.Wire].Add(BuildSoundType.Complete, new List<AudioClip>());
                for (int i = 0; i < BuildingAudioClips.Length; ++i)
                    BuildSoundDic[CommonType.Wire][BuildSoundType.Complete].Add(BuildingAudioClips[i]);

                BuildingAudioClips = Resources.LoadAll<AudioClip>("Sound/Mine");
                for (int i = 0; i < BuildingAudioClips.Length; ++i)
                {
                    strkey = BuildingAudioClips[i].name.Substring(0, BuildingAudioClips[i].name.LastIndexOf('_'));
                    strkey = strkey.Substring(strkey.LastIndexOf('_') + 1, strkey.Length - strkey.LastIndexOf('_') - 1);
                    strkey = textInfo.ToTitleCase(strkey);

                    if (!strkey.ToEnum<BuildSoundType>().Equals(-1) &&
                        !BuildSoundDic[CommonType.Mine].ContainsKey((BuildSoundType)strkey.ToEnum<BuildSoundType>()))
                    {
                        BuildSoundDic[CommonType.Mine].Add((BuildSoundType)strkey.ToEnum<BuildSoundType>(), new List<AudioClip>());
                        BuildSoundDic[CommonType.Mine][(BuildSoundType)strkey.ToEnum<BuildSoundType>()].Add(BuildingAudioClips[i]);
                    }
                    else if (!strkey.ToEnum<BuildSoundType>().Equals(-1) &&
                        BuildSoundDic[CommonType.Mine].ContainsKey((BuildSoundType)strkey.ToEnum<BuildSoundType>()))
                    {
                        BuildSoundDic[CommonType.Mine][(BuildSoundType)strkey.ToEnum<BuildSoundType>()].Add(BuildingAudioClips[i]);
                    }
                }
                break;
            case 34:
                BuildingAudioClips = Resources.LoadAll<AudioClip>("Sound/Turret");
                for (int i = 0; i < BuildingAudioClips.Length; ++i)
                {
                    strkey = BuildingAudioClips[i].name.Substring(0, BuildingAudioClips[i].name.LastIndexOf('_'));
                    strkey = strkey.Substring(strkey.LastIndexOf('_') + 1, strkey.Length - strkey.LastIndexOf('_') - 1);
                    strkey = textInfo.ToTitleCase(strkey);

                    if (!strkey.ToEnum<BuildSoundType>().Equals(-1) &&
                        !BuildSoundDic[CommonType.Turret].ContainsKey((BuildSoundType)strkey.ToEnum<BuildSoundType>()))
                    {
                        BuildSoundDic[CommonType.Turret].Add((BuildSoundType)strkey.ToEnum<BuildSoundType>(), new List<AudioClip>());
                        BuildSoundDic[CommonType.Turret][(BuildSoundType)strkey.ToEnum<BuildSoundType>()].Add(BuildingAudioClips[i]);
                    }
                    else if (!strkey.ToEnum<BuildSoundType>().Equals(-1) &&
                        BuildSoundDic[CommonType.Turret].ContainsKey((BuildSoundType)strkey.ToEnum<BuildSoundType>()))
                    {
                        BuildSoundDic[CommonType.Turret][(BuildSoundType)strkey.ToEnum<BuildSoundType>()].Add(BuildingAudioClips[i]);
                    }
                }
                break;
            case 35:
                BuildingAudioClips = Resources.LoadAll<AudioClip>("Sound/Balloon");
                for (int i = 0; i < BuildingAudioClips.Length; ++i)
                {
                    strkey = BuildingAudioClips[i].name.Substring(0, BuildingAudioClips[i].name.LastIndexOf('_'));
                    strkey = strkey.Substring(strkey.LastIndexOf('_') + 1, strkey.Length - strkey.LastIndexOf('_') - 1);
                    strkey = textInfo.ToTitleCase(strkey);

                    if (!strkey.ToEnum<BuildSoundType>().Equals(-1) &&
                        !BuildSoundDic[CommonType.Balloon].ContainsKey((BuildSoundType)strkey.ToEnum<BuildSoundType>()))
                    {
                        BuildSoundDic[CommonType.Balloon].Add((BuildSoundType)strkey.ToEnum<BuildSoundType>(), new List<AudioClip>());
                        BuildSoundDic[CommonType.Balloon][(BuildSoundType)strkey.ToEnum<BuildSoundType>()].Add(BuildingAudioClips[i]);
                    }
                    else if (!strkey.ToEnum<BuildSoundType>().Equals(-1) &&
                        BuildSoundDic[CommonType.Balloon].ContainsKey((BuildSoundType)strkey.ToEnum<BuildSoundType>()))
                    {
                        BuildSoundDic[CommonType.Balloon][(BuildSoundType)strkey.ToEnum<BuildSoundType>()].Add(BuildingAudioClips[i]);
                    }
                }
                break;
            case 36:
                BuildingAudioClips = Resources.LoadAll<AudioClip>("Sound/Cannon");
                for (int i = 0; i < BuildingAudioClips.Length; ++i)
                {
                    strkey = BuildingAudioClips[i].name.Substring(0, BuildingAudioClips[i].name.LastIndexOf('_'));
                    strkey = strkey.Substring(strkey.LastIndexOf('_') + 1, strkey.Length - strkey.LastIndexOf('_') - 1);
                    strkey = textInfo.ToTitleCase(strkey);

                    if (!strkey.ToEnum<BuildSoundType>().Equals(-1) &&
                        !BuildSoundDic[CommonType.Cannon].ContainsKey((BuildSoundType)strkey.ToEnum<BuildSoundType>()))
                    {
                        BuildSoundDic[CommonType.Cannon].Add((BuildSoundType)strkey.ToEnum<BuildSoundType>(), new List<AudioClip>());
                        BuildSoundDic[CommonType.Cannon][(BuildSoundType)strkey.ToEnum<BuildSoundType>()].Add(BuildingAudioClips[i]);
                    }
                    else if (!strkey.ToEnum<BuildSoundType>().Equals(-1) &&
                        BuildSoundDic[CommonType.Cannon].ContainsKey((BuildSoundType)strkey.ToEnum<BuildSoundType>()))
                    {
                        BuildSoundDic[CommonType.Cannon][(BuildSoundType)strkey.ToEnum<BuildSoundType>()].Add(BuildingAudioClips[i]);
                    }
                }
                break;
            case 37:
                AudioClip BuildingAudioClip = Resources.Load<AudioClip>("Sound/Warren/T1/un_str_t1_bldseq");
                BuildSoundDic[CommonType.WarrenT1].Add(BuildSoundType.Complete, new List<AudioClip>());
                BuildSoundDic[CommonType.WarrenT1][BuildSoundType.Complete].Add(BuildingAudioClip);
                BuildingAudioClip = Resources.Load<AudioClip>("Sound/Warren/T2/un_str_t2_bldseq");
                BuildSoundDic[CommonType.WarrenT2].Add(BuildSoundType.Complete, new List<AudioClip>());
                BuildSoundDic[CommonType.WarrenT2][BuildSoundType.Complete].Add(BuildingAudioClip);
                BuildingAudioClip = Resources.Load<AudioClip>("Sound/Warren/T3/un_str_t3_bldseq");
                BuildSoundDic[CommonType.WarrenT3].Add(BuildSoundType.Complete, new List<AudioClip>());
                BuildSoundDic[CommonType.WarrenT3][BuildSoundType.Complete].Add(BuildingAudioClip);
                break;
            case 38:
                BuildingAudioClips = Resources.LoadAll<AudioClip>("Sound/Gristmill");
                for (int i = 0; i < BuildingAudioClips.Length; ++i)
                {
                    strkey = BuildingAudioClips[i].name.Substring(0, BuildingAudioClips[i].name.LastIndexOf('_'));
                    strkey = strkey.Substring(strkey.LastIndexOf('_') + 1, strkey.Length - strkey.LastIndexOf('_') - 1);
                    strkey = textInfo.ToTitleCase(strkey);

                    if (!strkey.ToEnum<BuildSoundType>().Equals(-1) &&
                        !BuildSoundDic[CommonType.Gristmill].ContainsKey((BuildSoundType)strkey.ToEnum<BuildSoundType>()))
                    {
                        BuildSoundDic[CommonType.Gristmill].Add((BuildSoundType)strkey.ToEnum<BuildSoundType>(), new List<AudioClip>());
                        BuildSoundDic[CommonType.Gristmill][(BuildSoundType)strkey.ToEnum<BuildSoundType>()].Add(BuildingAudioClips[i]);
                    }
                    else if (!strkey.ToEnum<BuildSoundType>().Equals(-1) &&
                        BuildSoundDic[CommonType.Gristmill].ContainsKey((BuildSoundType)strkey.ToEnum<BuildSoundType>()))
                    {
                        BuildSoundDic[CommonType.Gristmill][(BuildSoundType)strkey.ToEnum<BuildSoundType>()].Add(BuildingAudioClips[i]);
                    }
                }
                break;
            case 39:
                BuildingAudioClips = Resources.LoadAll<AudioClip>("Sound/Cabin");
                for (int i = 0; i < BuildingAudioClips.Length; ++i)
                {
                    strkey = BuildingAudioClips[i].name.Substring(0, BuildingAudioClips[i].name.LastIndexOf('_'));
                    strkey = strkey.Substring(strkey.LastIndexOf('_') + 1, strkey.Length - strkey.LastIndexOf('_') - 1);
                    strkey = textInfo.ToTitleCase(strkey);

                    if (!strkey.ToEnum<BuildSoundType>().Equals(-1) &&
                        !BuildSoundDic[CommonType.Cabin].ContainsKey((BuildSoundType)strkey.ToEnum<BuildSoundType>()))
                    {
                        BuildSoundDic[CommonType.Cabin].Add((BuildSoundType)strkey.ToEnum<BuildSoundType>(), new List<AudioClip>());
                        BuildSoundDic[CommonType.Cabin][(BuildSoundType)strkey.ToEnum<BuildSoundType>()].Add(BuildingAudioClips[i]);
                    }
                    else if (!strkey.ToEnum<BuildSoundType>().Equals(-1) &&
                        BuildSoundDic[CommonType.Cabin].ContainsKey((BuildSoundType)strkey.ToEnum<BuildSoundType>()))
                    {
                        BuildSoundDic[CommonType.Cabin][(BuildSoundType)strkey.ToEnum<BuildSoundType>()].Add(BuildingAudioClips[i]);
                    }
                }
                break;
            case 40:
                BuildingAudioClips = Resources.LoadAll<AudioClip>("Sound/Pig");
                for (int i = 0; i < BuildingAudioClips.Length; ++i)
                {
                    strkey = BuildingAudioClips[i].name.Substring(0, BuildingAudioClips[i].name.LastIndexOf('_'));
                    strkey = strkey.Substring(strkey.LastIndexOf('_') + 1, strkey.Length - strkey.LastIndexOf('_') - 1);
                    strkey = textInfo.ToTitleCase(strkey);

                    if (!strkey.ToEnum<BuildSoundType>().Equals(-1) &&
                        !BuildSoundDic[CommonType.Pig].ContainsKey((BuildSoundType)strkey.ToEnum<BuildSoundType>()))
                    {
                        BuildSoundDic[CommonType.Pig].Add((BuildSoundType)strkey.ToEnum<BuildSoundType>(), new List<AudioClip>());
                        BuildSoundDic[CommonType.Pig][(BuildSoundType)strkey.ToEnum<BuildSoundType>()].Add(BuildingAudioClips[i]);
                    }
                    else if (!strkey.ToEnum<BuildSoundType>().Equals(-1) &&
                        BuildSoundDic[CommonType.Pig].ContainsKey((BuildSoundType)strkey.ToEnum<BuildSoundType>()))
                    {
                        BuildSoundDic[CommonType.Pig][(BuildSoundType)strkey.ToEnum<BuildSoundType>()].Add(BuildingAudioClips[i]);
                    }
                }
                break;
            case 41:
                BuildingAudioClips = Resources.LoadAll<AudioClip>("Sound/CampFire");
                for (int i = 0; i < BuildingAudioClips.Length; ++i)
                {
                    strkey = BuildingAudioClips[i].name.Substring(0, BuildingAudioClips[i].name.LastIndexOf('_'));
                    strkey = strkey.Substring(strkey.LastIndexOf('_') + 1, strkey.Length - strkey.LastIndexOf('_') - 1);
                    strkey = textInfo.ToTitleCase(strkey);

                    if (!strkey.ToEnum<BuildSoundType>().Equals(-1) &&
                        !BuildSoundDic[CommonType.CampFire].ContainsKey((BuildSoundType)strkey.ToEnum<BuildSoundType>()))
                    {
                        BuildSoundDic[CommonType.CampFire].Add((BuildSoundType)strkey.ToEnum<BuildSoundType>(), new List<AudioClip>());
                        BuildSoundDic[CommonType.CampFire][(BuildSoundType)strkey.ToEnum<BuildSoundType>()].Add(BuildingAudioClips[i]);
                    }
                    else if (!strkey.ToEnum<BuildSoundType>().Equals(-1) &&
                        BuildSoundDic[CommonType.CampFire].ContainsKey((BuildSoundType)strkey.ToEnum<BuildSoundType>()))
                    {
                        BuildSoundDic[CommonType.CampFire][(BuildSoundType)strkey.ToEnum<BuildSoundType>()].Add(BuildingAudioClips[i]);
                    }
                }
                break;
            case 42:
                endflag = true;
                break;
        }
    }

}
