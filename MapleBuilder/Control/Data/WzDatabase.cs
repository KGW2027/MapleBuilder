﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json.Nodes;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using MapleAPI.DataType;
using MapleAPI.Enum;
using MapleBuilder.Control.Data.Item;
using MapleBuilder.Control.Data.Serailize;
using MapleBuilder.View.SubFrames;

namespace MapleBuilder.Control.Data;

#pragma warning disable CS0168

public class WzDatabase
{
    private static WzDatabase? _self;
    public static WzDatabase Instance
    {
        get { return _self ??= new WzDatabase(); }
    }
    
    public delegate void WzDataLoaded(WzDatabase database);
    public static WzDataLoaded? OnWzDataLoadCompleted;

    private WzDatabase()
    {
        EquipmentDataList = new Dictionary<string, EquipmentData>();
        LoadEquipments();
    }

    public readonly Dictionary<string, EquipmentData> EquipmentDataList;
    private void LoadEquipments()
    {
        const string cachedEquipmentPath = "./equipments.dat";
        if (!File.Exists(cachedEquipmentPath))
        {
            InitEquipments();
            WzSerializer serializer = new WzSerializer();
            WzSerializer innerSerializer = new WzSerializer();
            serializer.InsertSignature(EquipmentData.GetSignature());
            int idx = 0;
            Summarize.StartProgressBar();
            foreach (IWzSerializable data in EquipmentDataList.Values)
            {
                if(idx++ % 1000 == 0)
                    Summarize.UpdateProgressBar(idx, EquipmentDataList.Count);
                innerSerializer.Clear();
                data.Serialize(innerSerializer);
                serializer += innerSerializer;
            }
            Summarize.FinishProgressBar();
            Console.WriteLine($"{cachedEquipmentPath} save Start...");
            serializer.Save(cachedEquipmentPath);
            Console.WriteLine("complete");
        }
        else
        {
            WzDeserializer deserializer = new WzDeserializer(cachedEquipmentPath);
            deserializer.CheckSignature(EquipmentData.GetSignature());
            while (true)
            {
                try
                {
                    EquipmentData data = EquipmentData.Deserialize(deserializer);
                    EquipmentDataList.TryAdd(data.Name, data);
                }
                catch (ArgumentNullException ignore)
                {
                    //ignore
                }
                catch (Exception ex)
                {
                    break;
                }
            }
        }
        OnWzDataLoadCompleted!.Invoke(this);
    }
    private void InitEquipments()
    {
        using (StreamReader reader = File.OpenText("./CharacterExtractorResult.json"))
        {
            string jsonContent = reader.ReadToEnd();
            JsonObject json = JsonNode.Parse(jsonContent)!.AsObject();

            int idx = 0;
            Summarize.StartProgressBar();
            foreach (var pair in json)
            {
                if (idx++ % 1000 == 0)
                    Summarize.UpdateProgressBar(idx, json.Count);
                if (pair.Value is not JsonObject itemData || !itemData.TryGetPropertyValue("info", out _)) continue;
                EquipmentDataList.TryAdd(pair.Key, new EquipmentData(itemData));
            }
            Summarize.FinishProgressBar();
        }

        using (StreamReader reader = File.OpenText("./ItemExtractorResult.json"))
        {
            string jsonContent = reader.ReadToEnd();
            JsonObject json = JsonNode.Parse(jsonContent)!.AsObject();
            
            int idx = 0;
            Summarize.StartProgressBar();
            foreach (var pair in json)
            {
                if (idx++ % 1000 == 0)
                    Summarize.UpdateProgressBar(idx, json.Count);
                if (pair.Value is not JsonObject itemData 
                    || !itemData.TryGetPropertyValue("info", out _)
                    || !itemData.TryGetPropertyValue("name", out _)) continue;
                EquipmentDataList.TryAdd(pair.Key, new EquipmentData(itemData));
            }
            Summarize.FinishProgressBar();
        }
    }

    public EquipmentData? GetPowerWeapon(CommonItem commonItem)
    {
        if (commonItem.EquipData == null) return null;
        
        foreach (var itemData in EquipmentDataList.Values)
        {
            if (itemData.Level != commonItem.ItemLevel) continue;
            if (!itemData.Name.Split(" ")[0].Equals(commonItem.EquipData.Name.Split(" ")[0])) continue;
            if (string.IsNullOrEmpty(itemData.AfterImage) || !itemData.AfterImage.Equals("dualBow")) continue;

            return itemData;
        }

        return null;
    }
}

public class EquipmentData : IWzSerializable
{
    private EquipmentData(string name, string afterImage, string islot,
        int id, int setId, int level, int maxUpgrade, 
        bool isBlockGoldHammer, bool isSuperior, bool isLucky,
        string? iconPath, Dictionary<MapleStatus.StatusType, int> table)
    {
        Name = name;
        AfterImage = afterImage;
        ISlot = islot;
        Id = id;
        SetId = setId;
        Level = level;
        this.maxUpgrade = maxUpgrade;
        IsBlockGoldHammer = isBlockGoldHammer;
        IsSuperior = isSuperior;
        IsLuckyItem = isLucky;
        IconPath = iconPath;
        incTable = table;
    }
    
    public EquipmentData(JsonObject data)
    {
        Name = data["name"]!.ToString();
        Id = int.Parse(data["itemId"]!.ToString());

        SetId = -1;
        maxUpgrade = 0;
        IsBlockGoldHammer = false;
        IsSuperior = false;
        IsLuckyItem = false;
        incTable = new Dictionary<MapleStatus.StatusType, int>();
        AfterImage = "";
        ISlot = "";
        
        JsonObject info = data["info"]!.AsObject();
        IconPath = info.TryGetPropertyValue("icon", out var pathNode) && pathNode != null ? pathNode.ToString() : null; 
        
        foreach (var pair in info)
        {
            int.TryParse(pair.Value!.ToString(), out int val);
            switch (pair.Key)
            {
                case "reqLevel":
                    Level = val;
                    break;
                case "incSTR":
                    this[MapleStatus.StatusType.STR] = val;
                    break;
                case "incDEX":
                    this[MapleStatus.StatusType.DEX] = val;
                    break;
                case "incINT":
                    this[MapleStatus.StatusType.INT] = val;
                    break;
                case "incLUK":
                    this[MapleStatus.StatusType.LUK] = val;
                    break;
                case "incPAD":
                    this[MapleStatus.StatusType.ATTACK_POWER] = val;
                    break;
                case "incMAD":
                    this[MapleStatus.StatusType.MAGIC_POWER] = val;
                    break;
                case "incMHP":
                    this[MapleStatus.StatusType.HP] = val;
                    break;
                case "incMHPr":
                    this[MapleStatus.StatusType.HP_RATE] = val;
                    break;
                case "incMMP":
                    this[MapleStatus.StatusType.MP] = val;
                    break;
                case "incMMPr":
                    this[MapleStatus.StatusType.MP_RATE] = val;
                    break;
                case "imdR":
                    this[MapleStatus.StatusType.IGNORE_DEF] = val;
                    break;
                case "bdR":
                    this[MapleStatus.StatusType.BOSS_DAMAGE] = val;
                    break;
                case "tuc":
                    maxUpgrade = val;
                    break;
                case "blockGoldHammer":
                    IsBlockGoldHammer = true;
                    break;
                case "islot":
                    ISlot = pair.Value!.ToString();
                    break;
                case "setItemID":
                    SetId = val;
                    break;
                case "jokerToSetItem":
                    IsLuckyItem = true;
                    break;
                case "afterImage":
                    AfterImage = pair.Value.ToString();
                    break;
            }
        }
    }

    private readonly Dictionary<MapleStatus.StatusType, int> incTable;
    private BitmapImage? thumbnail;
    private readonly int maxUpgrade;
    private string? hash;
    
    public readonly string Name;
    public string? IconPath;
    public readonly string AfterImage;
    public readonly string ISlot;
    public readonly bool IsLuckyItem;
    public readonly int Id;
    public readonly int SetId;
    public readonly int Level;
    public readonly bool IsBlockGoldHammer;
    public readonly bool IsSuperior;

    public string DataHash
    {
        get
        {
            if (hash != null) return hash;
            HashGen();
            return hash;
        }
    }

    public void HashGen()
    {
        hash = $"{DateTime.Now}::{GetHashCode():X}";
    }

    public int MaxUpgrade => maxUpgrade + (IsBlockGoldHammer ? 0 : 1);
    
    public BitmapImage Image
    {
        get
        {
            if (thumbnail != null) return thumbnail;
            thumbnail = new BitmapImage();
            if (IconPath == null) return thumbnail;
            string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, IconPath);
            if (!File.Exists(fullPath)) return thumbnail;
            thumbnail.BeginInit();
            thumbnail.UriSource = new Uri(fullPath);
            thumbnail.EndInit();
            return thumbnail;
        }
    }

    public int this[MapleStatus.StatusType statusType]
    {
        get => incTable.GetValueOrDefault(statusType, 0);
        private set
        {
            incTable.TryAdd(statusType, 0);
            incTable[statusType] = value;
        }
    }

    public MapleStatContainer GetStatus()
    {
        MapleStatContainer msc = new MapleStatContainer();
        foreach (var pair in incTable)
            msc[pair.Key] += pair.Value;
        return msc;
    }

    public static byte[] GetSignature()
    {
        return new byte[] {69, 81, 85, 73};
    }

    void IWzSerializable.Serialize(WzSerializer serializer)
    {
        serializer.SerializeString(Name);
        serializer.SerializeString(AfterImage);
        serializer.SerializeString(ISlot);
        serializer.SerializeInt(Id);
        serializer.SerializeInt(SetId);
        serializer.SerializeInt(Level);
        serializer.SerializeInt(maxUpgrade);
        serializer.SerializeByte((byte) (IsBlockGoldHammer ? 1 : 0));
        serializer.SerializeByte((byte) (IsSuperior ? 1 : 0));
        serializer.SerializeByte((byte) (IsLuckyItem ? 1 : 0));
        serializer.SerializeString(IconPath);
        foreach (var pair in incTable)
        {
            if (pair.Key == MapleStatus.StatusType.OTHER) continue;
            serializer.SerializeByte((byte) pair.Key);
            serializer.SerializeInt(pair.Value);
        }
        int length = serializer.Count;
        serializer.SerializeIntFirst(length);
    }

    public static EquipmentData Deserialize(WzDeserializer deserializer)
    {
        int length = deserializer.ReadInt();
        int endOffset = deserializer.GetOffset() + length;
        
        string name = deserializer.ReadString()!;
        string aftImage = deserializer.ReadString()!;
        string islot = deserializer.ReadString()!;
        int id = deserializer.ReadInt();
        int setId = deserializer.ReadInt();
        int level = deserializer.ReadInt();
        int upgrade = deserializer.ReadInt();
        bool goldhammer = deserializer.ReadBool();
        bool superior = deserializer.ReadBool();
        bool lucky = deserializer.ReadBool();
        string? icon = deserializer.ReadString();
        Dictionary<MapleStatus.StatusType, int> status = new Dictionary<MapleStatus.StatusType, int>();

        while (deserializer.GetOffset() < endOffset)
        {
            MapleStatus.StatusType key = (MapleStatus.StatusType) deserializer.ReadByte();
            int value = deserializer.ReadInt();
            status.TryAdd(key, value);
        }
        
        return new EquipmentData(name, aftImage, islot, id, setId, level, upgrade, goldhammer, superior, lucky, icon, status);
    }
}