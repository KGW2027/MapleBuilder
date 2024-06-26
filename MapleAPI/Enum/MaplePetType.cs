﻿namespace MapleAPI.Enum;

public class MaplePetType
{
    public enum PetType
    {
        WONDER_BLACK,
        LUNA_SWEET,
        LUNA_DREAM,
        LUNA_PETIT,
        OTHER
    }

    public static PetType GetPetType(string name)
    {
        return name switch
        {
            "루나 쁘띠" => PetType.LUNA_PETIT,
            "루나 드림" => PetType.LUNA_DREAM,
            "루나 스윗" => PetType.LUNA_SWEET,
            "원더 블랙" => PetType.WONDER_BLACK,
            _ => PetType.OTHER
        };
    }

    public static string GetPetTypeString(PetType petType)
    {
        return petType switch
        {
            PetType.WONDER_BLACK => "원더 블랙",
            PetType.LUNA_SWEET => "루나 스윗",
            PetType.LUNA_DREAM => "루나 드림",
            PetType.LUNA_PETIT => "루나 쁘띠",
            PetType.OTHER => "멀티펫",
            _ => throw new ArgumentOutOfRangeException(nameof(petType), petType, null)
        };
    }
}