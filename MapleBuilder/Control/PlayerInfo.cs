using System;
using System.Collections.Generic;
using MapleAPI.DataType;
using MapleAPI.DataType.Item;
using MapleAPI.Enum;
using MapleBuilder.MapleData;

namespace MapleBuilder.Control;

public class PlayerInfo
{
    public PlayerInfo(uint level, MapleStatus.StatusType mainStat, 
        MapleStatus.StatusType subStat, MapleStatus.StatusType subStat2 = 0)
    {
        PlayerStat = new MapleStatContainer
        {
            MainStatType = mainStat,
            SubStatType = subStat,
            SubStat2Type = subStat2
        };
        PlayerStat[mainStat] += (mainStat == MapleStatus.StatusType.HP ? 90 * level + 545 : 5 * level + 18);
        PlayerStat[subStat] += 4;
        PlayerStat[subStat2] += 4;
        PlayerStat[MapleStatus.StatusType.CRITICAL_CHANCE] += 5;
        SetEffects = new SetEffect();
        LastSymbols = new Dictionary<MapleSymbol.SymbolType, int>();
        LastAbilities = new Dictionary< MapleStatus.StatusType, int>();
    }
    
    public MapleStatContainer PlayerStat { get; protected internal set; }
    public SetEffect SetEffects { get; private set; }
    public Dictionary<MapleSymbol.SymbolType, int> LastSymbols;
    public Dictionary< MapleStatus.StatusType, int> LastAbilities;
    
    #region 장비 아이템 효과 적용
    
    ///<summary>
    ///    잠재능력, 에디셔널 잠재능력을 현재 스텟에 적용합니다.
    ///</summary>
    private void ApplyPotential(KeyValuePair<MapleStatus.StatusType, int>[] potential, bool isAdd = true)
    {
        int sign = isAdd ? 1 : -1;

        foreach (var pair in potential)
            PlayerStat[pair.Key] += pair.Value * sign;
    }
    
    
    ///<summary>
    ///    아이템 착용과 해제를 현재 스텟에 시뮬레이션합니다.
    ///</summary>
    private void ApplyAddSub(MapleCommonItem item, bool isAdd)
    {
        // item.Status.Flush();
        if (isAdd)  PlayerStat += item.Status;
        else        PlayerStat -= item.Status;
        
        // 잠재능력 적용
        if (item.Potential != null)
        {
            ApplyPotential(item.Potential.Potentials, isAdd);
            ApplyPotential(item.Potential.Additionals, isAdd);
        }
        
        // 세트 효과 적용
        MapleStatContainer setEffectPrev = SetEffects.GetSetOptions();
        if(isAdd)
            SetEffects.Add(item);
        else
            SetEffects.Sub(item);
        PlayerStat += SetEffects.GetSetOptions() - setEffectPrev;
    }
    
    ///<summary>
    ///    아이템을 장착합니다.
    ///</summary>
    public void AddCommonItem(MapleCommonItem item)
    {
        ApplyAddSub(item, true);
    }
    
    ///<summary>
    ///    아이템을 해제합니다.
    ///</summary>
    public void SubCommonItem(MapleCommonItem item)
    {
        ApplyAddSub(item, false);
    }
    #endregion
    
    #region 심볼 효과 적용

    private int[] GetSymbolStats(Dictionary<MapleSymbol.SymbolType, int> symbolData)
    {
        int stat = 0, arcane = 0, authentic = 0;
        int baseValue = BuilderDataContainer.CharacterInfo!.Class switch
            {
              MapleClass.ClassType.XENON => 48,
              MapleClass.ClassType.DEMON_AVENGER => 2100,
              _ => 100
            };
        foreach (var pair in symbolData)
        {
            if (pair.Value == 0) continue;
            switch (pair.Key)
            {
                case MapleSymbol.SymbolType.YEORO:
                case MapleSymbol.SymbolType.CHUCHU:
                case MapleSymbol.SymbolType.LACHELEIN:
                case MapleSymbol.SymbolType.ARCANA:
                case MapleSymbol.SymbolType.MORAS:
                case MapleSymbol.SymbolType.ESFERA:
                    stat += baseValue * (pair.Value + 2);
                    arcane += (pair.Value + 2) * 10;
                    break;
                case MapleSymbol.SymbolType.CERNIUM:
                case MapleSymbol.SymbolType.ARCS:
                case MapleSymbol.SymbolType.ODIUM:
                case MapleSymbol.SymbolType.DOWONKYUNG:
                case MapleSymbol.SymbolType.ARTERIA:
                case MapleSymbol.SymbolType.CARCION:
                    stat += baseValue * (2 * pair.Value + 3);
                    authentic += pair.Value * 10;
                    break;
                case MapleSymbol.SymbolType.UNKNOWN:
                default:
                    break;
            }
        }
        return new[]{stat, arcane, authentic};
    }
    
    public void ApplySymbolData(Dictionary<MapleSymbol.SymbolType, int> symbolData)
    {
        int[] prev = GetSymbolStats(LastSymbols);
        int[] now = GetSymbolStats(symbolData);
        int[] delta = {now[0] - prev[0], now[1] - prev[1], now[2] - prev[2]};
        PlayerStat[PlayerStat.MainStatType + 0x20] += delta[0];
        if (BuilderDataContainer.CharacterInfo!.Class == MapleClass.ClassType.XENON)
        {
            PlayerStat[PlayerStat.SubStatType + 0x20] += delta[0];
            PlayerStat[PlayerStat.SubStat2Type + 0x20] += delta[0];
        }

        PlayerStat[MapleStatus.StatusType.ARCANE_FORCE] += delta[1];
        PlayerStat[MapleStatus.StatusType.AUTHENTIC_FORCE] += delta[2];

        LastSymbols = symbolData;
        BuilderDataContainer.RefreshAll();
    }
    
    #endregion
    
    #region 어빌리티 효과 적용

    public void ApplyAbility(Dictionary<MapleStatus.StatusType, int> abilities)
    {
        foreach (var pair in LastAbilities)
            PlayerStat[pair.Key] += pair.Value * -1;
        foreach (var pair in abilities)
            PlayerStat[pair.Key] += pair.Value;
        LastAbilities = abilities;
        
        BuilderDataContainer.RefreshAll();
    }
    
    #endregion
    
    #region 하이퍼 스탯 효과 적용
    
    public void ApplyHyperStat(MapleStatus.StatusType type, double delta)
    {
        PlayerStat[type] += delta;
        BuilderDataContainer.RefreshAll();
    }
    
    #endregion
    
    #region 유니온 공격대 효과 적용
    
    public void ApplyUnionRaider(MapleStatus.StatusType type, int delta)
    {
        PlayerStat[type] += delta;
        BuilderDataContainer.RefreshAll();
    }
    
    #endregion
    
    #region 펫 장비 효과 적용

    public void ApplyPetItem(List<MaplePetItem> petItems)
    {
        foreach (MaplePetItem petItem in petItems)
            PlayerStat += petItem.Status;
    }
    
    #endregion
}