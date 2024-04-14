using System;
using MapleAPI.DataType;
using MapleBuilder.Control.Data;
using MapleBuilder.Control.Data.Item;

namespace MapleBuilder.Control;

public class GlobalDataController
{
    private static GlobalDataController? _self;
    public static GlobalDataController Instance
    {
        get { return _self ??= new GlobalDataController(); }
    }

    public delegate void DataUpdated(PlayerData pData);
    public static DataUpdated? OnDataUpdated; 

    public void LoadPlayerData(string ocid)
    {
        CharacterInfo? cInfo = CharacterInfo.FromOcid(ocid).Result;
        if (cInfo == null) throw new Exception($"플레이어의 데이터를 불러오려고 했지만 실패했습니다. {{OCID={ocid}}}");

        playerData = new PlayerData(cInfo);

        foreach (var equipItem in cInfo.Items)
        {
            if (!ItemDatabase.Instance.RegisterItem(equipItem, out ItemBase? item, cInfo.PlayerName) ||
                item == null) continue;
            if (playerData[item.EquipType] != null) continue;
            playerData[item.EquipType] = item;
        }

        //MapleStatus.StatusType[] statTypes = MapleClass.GetClassStatType(charInfo.Class);
        // PlayerStatus = new PlayerInfo(charInfo.Level, statTypes[0], statTypes[1], statTypes[2]);
        // PlayerStatus.ApplySymbolData(charInfo.SymbolLevels);
        // PlayerStatus.ApplyPetItem(charInfo.PetEquips);
        // // 임시 성향 로드 코드
        // foreach (var pair in charInfo.PropensityLevels)
        // {
        //     MapleStatus.StatusType[] propStatTypes = MaplePropensity.GetStatusByPropensity(pair.Key);
        //     double[] args = MaplePropensity.GetStatusValueByPropensity(pair.Key);
        //
        //     for (int idx = 0; idx < propStatTypes.Length; idx++)
        //     {
        //         double delta = args[idx + 1] * Math.Floor(pair.Value / args[0]);
        //         PlayerStatus.PlayerStat[propStatTypes[idx]] += delta;
        //     }
        // }
        // // 임시 헥사 스텟 로드 코드
        // MapleHexaStatus.HexaStatus hexaStatus = charInfo.HexaStatLevels;
        // if (hexaStatus.MainStat.Key != MapleStatus.StatusType.OTHER)
        // {
        //     PlayerStatus.PlayerStat[hexaStatus.MainStat.Key] += MapleHexaStatus.GetStatusValue(hexaStatus.MainStat.Key,
        //         hexaStatus.MainStat.Value, true, charInfo.Class);
        //     PlayerStatus.PlayerStat[hexaStatus.SubStat1.Key] += MapleHexaStatus.GetStatusValue(hexaStatus.SubStat1.Key,
        //         hexaStatus.SubStat1.Value, false, charInfo.Class);
        //     PlayerStatus.PlayerStat[hexaStatus.SubStat2.Key] += MapleHexaStatus.GetStatusValue(hexaStatus.SubStat2.Key,
        //         hexaStatus.SubStat2.Value, false, charInfo.Class);
        // }
        //
        // StatSymbol.InitAbility(charInfo.AbilityValues);
        // StatSymbol.InitHyperStat(charInfo.HyperStatLevels);
        // RenderOverview.Update(charInfo);
        // foreach (var unionInfo in charInfo.UnionInfo)
        //     UnionFrame.UpdateUnionRank(unionInfo.classType, unionInfo.raiderRank);
        // UnionRaiderMap.UpdateUnionRaider(charInfo.UnionInfo, charInfo.UnionInner);
        // UnionArtifactPanel.UpdateArtifactPanel(charInfo.ArtifactPanels);
        //
        // foreach(var pair in charInfo.SkillData)
        //     Console.WriteLine($"{pair.Key} : {pair.Value}");
    }
    
    private PlayerData? playerData;

    public PlayerData? PlayerInstance => playerData;







}