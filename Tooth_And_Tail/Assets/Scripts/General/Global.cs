using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Global
{
    public static readonly int[] DirX = { 1, -1, 0, 0, 1, 1, -1, -1 };
    public static readonly int[] DirY = { 0, 0, 1, -1, 1, -1, 1, -1 };
    public static readonly Vector3Int[] LookDir = { new Vector3Int(1, 1, 0), new Vector3Int(-1, -1, 0), new Vector3Int(-1, 1, 0), new Vector3Int(1, -1, 0) };
    public static readonly Vector3Int[] UpTiles = { new Vector3Int(0, 0, 0), new Vector3Int(1, 0, 0), new Vector3Int(0, 1, 0), new Vector3Int(1, 1, 0) };

    public static readonly Vector3Int InvalidTilePos = new Vector3Int(-999, -999, -999);
    public static readonly Vector3 InvalidWorldPos = new Vector3(0, 0, -999);

    public static readonly float SnapshotInterval = 5.0f;
    public static readonly int GraphTimeInterval = 30;

    public static readonly int TimeAttackPlayTime = 300;
    public static readonly int CampaignPhase2Time = 180;
    public static readonly int CampaignPlayTime = 300;

    public static readonly float StaminaChargeTime = 180f;
    public static readonly int StaminaMulti = 10;
    public static readonly int StaminaCampaign = 15;

    public static readonly float BoosterPercent = 0.2f;

    public static readonly int EndMoveUISpeed = 2400;
    public static readonly int EndMoveUIVibeSpeed = 1000;

    public static readonly int StaminaPortionEffect = 20;
    public static readonly int GoldPortionEffect = 1000;
    public static readonly int JewelPortionEffect = 5;

    public static readonly Color GraphColorBellafide = new Color(0, 0, 1);
    public static readonly Color GraphColorHopper = new Color(1, 0, 0);
    public static readonly Color GraphColorQuartermaster = new Color(0, 1, 0);
    public static readonly Color GraphColorArchimedes = new Color(1, 1, 0);

    public static readonly Color CommanderButtonColorBellafide = new Color(0, 0.5f, 1, 1);
    public static readonly Color CommanderButtonColorHopper = new Color32(255, 9, 15, 255);
    public static readonly Color CommanderButtonColorQuartermaster = new Color32(55, 192, 27, 255);
    public static readonly Color CommanderButtonColorArchimedes = new Color32(255, 200, 35, 255);

    public static readonly Color CommanderInGameColorBellafide = new Color(0, 0.5f, 1, 1);
    public static readonly Color CommanderInGameColorHopper = new Color32(255, 0, 0, 255);
    public static readonly Color CommanderInGameColorQuartermaster = new Color32(55, 163, 27, 255);
    public static readonly Color CommanderInGameColorArchimedes = new Color32(255, 200, 35, 255);

    public static readonly Color CommanderUIColorBellafide = new Color32(33, 115, 200, 255);
    public static readonly Color CommanderUIColorHopper = new Color32(220, 0, 0, 255);
    public static readonly Color CommanderUIColorQuartermaster = new Color32(55, 163, 27, 255);
    public static readonly Color CommanderUIColorArchimedes = new Color32(255, 200, 35, 255);

    public static readonly Color SummaryBgColorBellafide = new Color32(93, 198, 255, 255);
    public static readonly Color SummaryBgColorHopper = new Color32(255, 114, 102, 255);
    public static readonly Color SummaryBgColorQuartermaster = new Color32(84, 200, 104, 255);
    public static readonly Color SummaryBgColorArchimedes = new Color32(255, 209, 105, 255);

    /// Minimap Colors
    public static readonly Color MinimapColorCommanderBellafide = new Color32(39, 113, 236, 255);
    public static readonly Color MinimapColorCommanderHopper = new Color32(224, 31, 31, 255);
    public static readonly Color MinimapColorCommanderQuartermaster = new Color32(38, 179, 25, 255);
    public static readonly Color MinimapColorCommanderArchimedes = new Color32(255, 228, 0, 255);

    public static readonly Color MinimapColorCampBellafide = new Color32(40, 100, 200, 255);
    public static readonly Color MinimapColorCampHopper = new Color32(200, 40, 40, 255);
    public static readonly Color MinimapColorCampQuartermaster = new Color32(40, 150, 30, 255);
    public static readonly Color MinimapColorCampArchimedes = new Color32(255, 194, 0, 255);

    public static readonly Color MinimapColorCampBellafideA = new Color32(40, 100, 200, 148);
    public static readonly Color MinimapColorCampHopperA = new Color32(200, 40, 40, 148);
    public static readonly Color MinimapColorCampQuartermasterA = new Color32(40, 150, 30, 148);
    public static readonly Color MinimapColorCampArchimedesA = new Color32(255, 194, 0, 128);

    public static readonly Color MinimapColorNeutral = new Color32(196, 196, 196, 255);
    public static readonly Color MinimapColorNeutralA = new Color32(196, 196, 196, 128);


    public static int Calculate_TileDistance(Vector3 from, Vector3 to)
    {
        var tileFromPos = TilemapSystem.Instance.WorldToCellPos(from);
        var tileToPos = TilemapSystem.Instance.WorldToCellPos(to);

        return Mathf.RoundToInt(Vector3Int.Distance(tileFromPos, tileToPos));
    }

    public static int Calculate_TileDistance(Vector3Int from, Vector3Int to)
    {
        return Mathf.RoundToInt(Vector3Int.Distance(from, to));
    }

    public static bool IsAttackableBuilding(Camp camp, Character building)
    {
        Camp buildingCamp = building.Base.MyCamp;

        if (camp == buildingCamp)
            return false;

        if (buildingCamp == Camp.End &&
            (CommonType.Gristmill == building.Base.Type || CommonType.Farm == building.Base.Type))
        {
            return false;
        }

        return true;
    }
}
