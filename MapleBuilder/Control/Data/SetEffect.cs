using System;
using System.Collections.Generic;
using System.Linq;
using MapleAPI.DataType;
using MapleAPI.Enum;
using MapleBuilder.Control.Data.Item;
using MapleBuilder.Control.Data.Setitem;

namespace MapleBuilder.Control.Data;

public class SetEffect
{
    private MapleEquipType.EquipType? LuckyItemType { get; set; }

    private readonly Dictionary<Type, List<MapleEquipType.EquipType>> playerSetEffect;
    private readonly Dictionary<Type, ISetType> setObjects;
    private readonly Dictionary<Type, int[]> setIdMap = new()
    {
        {typeof(MapleSet120Boss), new[] {452, 453, 454, 455, 456}},
        {typeof(MapleSet130Boss), new[] {457, 458, 459, 460, 461}},
        {typeof(MapleSet140Boss), new[] { 25,  26,  27,  28,  29}},
        {typeof(MapleSet150Boss), new[] {247, 248, 249, 250, 251}},
        {typeof(MapleSet160Boss), new[] {504, 505, 506, 507, 508}},
        {typeof(MapleSet200Boss), new[] {617, 618, 619, 620, 621}},
        {typeof(MapleSet250Boss), new[] {886, 887, 888, 889, 890}},
        
        {typeof(MapleSet130Common), new[]{442, 443, 444, 445, 446}},
        {typeof(MapleSet140Common), new[]{447, 448, 449, 450, 451}},
        
        {typeof(MapleSetMaister), new[]{287}},
        {typeof(MapleSetBossAccessory), new[] {462}},
        {typeof(MapleSetMonsterPark), new[] {584}},
        {typeof(MapleSetDawnAccessory), new[] {869}},
        {typeof(MapleSetBlackAccessory), new[] {677}},
    };

    public string SetString { get; private set; }

    protected internal SetEffect()
    {
        setObjects = new Dictionary<Type, ISetType>();
        playerSetEffect = new Dictionary<Type, List<MapleEquipType.EquipType>>();
        LuckyItemType = null;

        SetString = "";
    }

    private Type? GetSetType(ItemBase item)
    {
        return item.EquipData == null ? null : setIdMap.Where(pair => pair.Value.Contains(item.EquipData.SetId)).Select(pair => pair.Key).FirstOrDefault();
    }
    
    public MapleStatContainer GetSetOptions()
    {
        SetString = "";
        MapleStatContainer result = new MapleStatContainer();
        foreach (var pair in playerSetEffect)
        {
            if (!setObjects.ContainsKey(pair.Key))
            {
                ISetType? instance = (ISetType?) Activator.CreateInstance(pair.Key);
                if (instance == null) throw new NullReferenceException($"{pair.Key}에 대한 CreateInstance가 NULL을 반환했습니다.");
                setObjects.Add(pair.Key, instance);
            }

            var stat = setObjects[pair.Key].GetSetEffect(pair.Value.ToArray(), LuckyItemType, out var setCount);
            result += stat;

            if (stat.IsEmpty()) continue;
            SetString += $"{setObjects[pair.Key].GetSignature()}:{setCount}|";
        }

        if (SetString.Length > 0) SetString = SetString[..^1];
        // Console.WriteLine($"New Set Options :: {SetString}");
        return result;
    }
    
    public void Add(ItemBase item)
    {
        if (item.EquipData is {IsLuckyItem: true})
            LuckyItemType = item.EquipType;

        Type? type = GetSetType(item);
        if (type == null) return;
        if(!playerSetEffect.ContainsKey(type)) playerSetEffect.Add(type, new List<MapleEquipType.EquipType>());
        playerSetEffect[type].Add(item.EquipType);
    }

    public void Sub(ItemBase item)
    {
        if (item.EquipData is {IsLuckyItem: true} && LuckyItemType != null)
            LuckyItemType = null;
        
        Type? type = GetSetType(item);
        if (type == null) return;
        playerSetEffect[type].Remove(item.EquipType);
    }
}