﻿using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Nodes;
using MapleAPI.Enum;

namespace MapleAPI.DataType;

public class MapleItem
{
    private string? hash;
    public string Hash
    {
        get
        {
            if (hash != null) return hash;
            byte[] bytes = Encoding.UTF8.GetBytes(data.ToString());
            byte[] hashBytes;
            using SHA256 sha256 = SHA256.Create();
            hashBytes = sha256.ComputeHash(bytes);
            hash = "";
            foreach (byte b in hashBytes)
                hash += b.ToString("x2");
            return hash;
        }
    }
    

    private readonly JsonObject? data;
    public MapleEquipType EquipType { get; private set; }
    public string Name { get; private set; }
    public uint StarForce { get; private set; }
    public uint SpecialRingLevel { get; private set; }
    public MapleOption? ExceptionalOption { get; private set; }
    public MapleOption? AddOption { get; private set; }
    public MapleOption? EtcOption { get; private set; }
    
    private MapleItem(JsonObject data)
    {
        this.data = data;
        Name = data["item_name"]!.ToString();
        StarForce = uint.TryParse(data["starforce"]!.ToString(), out uint val) ? val : 0;
        SpecialRingLevel = uint.TryParse(data["special_ring_level"]!.ToString(), out uint val2) ? val2 : 0;
        ExceptionalOption = new MapleOption(data["item_exceptional_option"]!.AsObject());
        AddOption = new MapleOption(data["item_add_option"]!.AsObject());
        EtcOption = new MapleOption(data["item_etc_option"]!.AsObject());
    }

    private MapleItem(string name, string desc)
    {
        Name = name;
        StarForce = 0;
        SpecialRingLevel = 0;
        AddOption = new MapleOption();
    }

    private static MapleEquipType GetEquipType(string slot)
    {
        return slot.TrimEnd('1', '2', '3', '4') switch
        {
            "모자" => MapleEquipType.HELMET,
            "얼굴장식" => MapleEquipType.FACE,
            "눈장식" => MapleEquipType.EYE,
            "귀고리" => MapleEquipType.EARRING,
            "상의" => MapleEquipType.TOP,
            "하의" => MapleEquipType.BOTTOM,
            "한벌옷" => MapleEquipType.TOP_BOTTOM,
            "신발" => MapleEquipType.BOOT,
            "장갑" => MapleEquipType.GLOVE,
            "망토" => MapleEquipType.CAPE,
            "보조무기" => MapleEquipType.SUB_WEAPON,
            "무기" => MapleEquipType.WEAPON,
            "반지" => MapleEquipType.RING,
            "펜던트" => MapleEquipType.PENDANT,
            "훈장" => MapleEquipType.MEDAL,
            "벨트" => MapleEquipType.BELT,
            "어깨장식" => MapleEquipType.SHOULDER,
            "포켓 아이템" => MapleEquipType.POCKET,
            "기계 심장" => MapleEquipType.HEART,
            "뱃지" => MapleEquipType.BADGE,
            "엠블렘" => MapleEquipType.EMBLEM,
            _ => throw new ArgumentOutOfRangeException(nameof(slot), slot, null)
        };
    }

    public static MapleItem Parse(JsonObject data)
    {
        if (data.ContainsKey("title_name"))
        {
            return new MapleItem(data["title_name"]!.ToString(), data["title_description"]!.ToString());
        }
        
        MapleItem item = new MapleItem(data);
        item.EquipType = GetEquipType(data["item_equipment_slot"]!.ToString());
        return item;
    }
}