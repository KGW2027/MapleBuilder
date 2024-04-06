using System.Text.Json.Nodes;
using MapleAPI.Enum;

namespace MapleAPI.DataType;

public class MapleOption
{
    public int Str { get; set; }
    public int Dex { get; set; }
    public int Int { get; set; }
    public int Luk { get; set; }
    public int MaxHp { get; set; }
    public int MaxMp { get; set; }
    public int AttackPower { get; set; }
    public int MagicPower { get; set; }
    public int BossDamage { get; set; }
    public int Damage { get; set; }
    public double IgnoreArmor { get; set; }
    public int AllStatRate { get; set; }
    public int MaxHpRate { get; set; }
    public int MaxMpRate { get; set; }
    public int CommonDamage { get; set; }
    public int CriticalDamage { get; set; }
    
    public int AllStatFlatInc
    {
        set
        {
            Str += value;
            Dex += value;
            Int += value;
            Luk += value;
        }
    }

    private int GetValue(JsonObject data, string key)
    {
        return data.ContainsKey(key) && int.TryParse(data[key]!.ToString(), out var val) ? val : 0;
    }

    public MapleOption()
    {
        Str = 0;
        Dex = 0;
        Int = 0;
        Luk = 0;
        MaxHp = 0;
        MaxMp = 0;
        AttackPower = 0;
        MagicPower = 0;
        BossDamage = 0;
        Damage = 0;
        IgnoreArmor = 0;
        AllStatRate = 0;
        MaxHpRate = 0;
        MaxMpRate = 0;
        CommonDamage = 0;
        CriticalDamage = 0;
    }
    
    public MapleOption(JsonObject data)
    {
        Str = GetValue(data, "str");
        Dex = GetValue(data, "dex");
        Int = GetValue(data, "int");
        Luk = GetValue(data, "luk");
        MaxHp = GetValue(data, "max_hp");
        MaxMp = GetValue(data, "max_mp");
        AttackPower = GetValue(data, "attack_power");
        MagicPower = GetValue(data, "magic_power");
        BossDamage = GetValue(data, "boss_damage");
        Damage = GetValue(data, "damage");
        IgnoreArmor = GetValue(data, "ignore_monster_armor");
        AllStatRate = GetValue(data, "all_stat");
        MaxHpRate = GetValue(data, "max_hp_rate");
        MaxMpRate = GetValue(data, "max_mp_rate");
        CommonDamage = 0;
        CriticalDamage = 0;
    }

    public void SetTitleOption(int bossDamage = 0, int ignoreArmor = 0, int atkMagic = 0, int allStat = 0, int hpmp = 0)
    {
        BossDamage = bossDamage;
        IgnoreArmor = ignoreArmor;
        AttackPower = atkMagic;
        MagicPower = atkMagic;
        AllStatRate = allStat;
        MaxHp = hpmp;
        MaxMp = hpmp;
    }

    public void ApplyIgnoreArmor(double newValue)
    {
        bool isAdd = newValue > 0;
        double cvtBase = (100 - IgnoreArmor) / 100.0;
        double cvtAdd = (100 - Math.Abs(newValue)) / 100.0;
        IgnoreArmor = isAdd ? (1 - cvtBase * cvtAdd) * 100 : (1 - cvtBase / cvtAdd) * 100;
    }

    public static MapleOption operator +(MapleOption lhs, MapleOption rhs)
    {
        lhs.Str += rhs.Str;
        lhs.Dex += rhs.Dex;
        lhs.Int += rhs.Int;
        lhs.Luk += rhs.Luk;
        lhs.MaxHp += rhs.MaxHp;
        lhs.MaxMp += rhs.MaxMp;
        lhs.MaxHpRate += rhs.MaxHpRate;
        lhs.MaxMpRate += rhs.MaxMpRate;
        lhs.AttackPower += rhs.AttackPower;
        lhs.MagicPower += rhs.MagicPower;
        lhs.BossDamage += rhs.BossDamage;
        lhs.Damage += rhs.Damage;
        lhs.ApplyIgnoreArmor(rhs.IgnoreArmor);
        lhs.AllStatRate += rhs.AllStatRate;
        lhs.CommonDamage += rhs.CommonDamage;
        lhs.CriticalDamage += rhs.CriticalDamage;
        
        return lhs;
    }
    
    public static MapleOption operator -(MapleOption lhs, MapleOption rhs)
    {
        lhs.Str -= rhs.Str;
        lhs.Dex -= rhs.Dex;
        lhs.Int -= rhs.Int;
        lhs.Luk -= rhs.Luk;
        lhs.MaxHp -= rhs.MaxHp;
        lhs.MaxMp -= rhs.MaxMp;
        lhs.MaxHpRate -= rhs.MaxHpRate;
        lhs.MaxMpRate -= rhs.MaxMpRate;
        lhs.AttackPower -= rhs.AttackPower;
        lhs.MagicPower -= rhs.MagicPower;
        lhs.BossDamage -= rhs.BossDamage;
        lhs.Damage -= rhs.Damage;
        lhs.ApplyIgnoreArmor(-rhs.IgnoreArmor);
        lhs.AllStatRate -= rhs.AllStatRate;
        lhs.CommonDamage -= rhs.CommonDamage;
        lhs.CriticalDamage -= rhs.CriticalDamage;
        
        return lhs;
    }

    public override string ToString()
    {
        return $"STR : {Str}, DEX : {Dex}, INT : {Int}, LUK : {Luk}, ALL STAT : {AllStatRate}\n"
               // + $"MaxHp : {MaxHp}, {MaxHpRate}%, MaxMp : {MaxMp}, {MaxMpRate}%\n"
               + $"Attack : {AttackPower}, Magic : {MagicPower}\n"
               // + $"Damage : {Damage}, BossDamage : {BossDamage}, CommonDamage : {CommonDamage}, CriticalDamage : {CriticalDamage}\n"
               // + $"IgnoreAmor : {IgnoreArmor:F2}%"
               ;
    }
}