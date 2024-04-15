using System;
using MapleAPI.DataType;
using MapleAPI.Enum;
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

        PlayerInstance = new PlayerData(cInfo);

        int ringCount = 0, pendCount = 0;
        foreach (var equipItem in cInfo.Items)
        {
            if (!ItemDatabase.Instance.RegisterItem(equipItem, out ItemBase? item, cInfo.PlayerName) ||
                item == null) continue;

            if (item.EquipType == MapleEquipType.EquipType.RING && ringCount <= 3)
                PlayerInstance.Equipment[item.EquipType, ringCount++] = item;
            else if (item.EquipType == MapleEquipType.EquipType.PENDANT && pendCount <= 1)
                PlayerInstance.Equipment[item.EquipType, pendCount++] = item;
            else if (PlayerInstance.Equipment[item.EquipType, 0] == null)
                PlayerInstance.Equipment[item.EquipType, 0] = item;
        }
        
        OnDataUpdated!.Invoke(PlayerInstance);
    }
    
    public PlayerData? PlayerInstance { get; private set; }







}