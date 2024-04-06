using System.Text.Json.Nodes;

namespace MapleAPI.DataType;

public class MapleOption
{
    public int Str { get; private set; }
    public int Dex { get; private set; }
    public int Int { get; private set; }
    public int Luk { get; private set; }
    public int MaxHp { get; private set; }
    public int MaxMp { get; private set; }
    public int AttackPower { get; private set; }
    public int MagicPower { get; private set; }
    public int BossDamage { get; private set; }
    public int Damage { get; private set; }
    public double IgnoreArmor { get; private set; }
    public int AllStat { get; private set; }
    public int MaxHpRate { get; private set; }
    public int MaxMpRate { get; private set; }

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
        AllStat = 0;
        MaxHpRate = 0;
        MaxMpRate = 0;
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
        AllStat = GetValue(data, "all_stat");
        MaxHpRate = GetValue(data, "max_hp_rate");
        MaxMpRate = GetValue(data, "max_mp_rate");
    }

    public void SetTitleOption(int bossDamage = 0, int ignoreArmor = 0, int atkMagic = 0, int allStat = 0, int hpmp = 0)
    {
        BossDamage = bossDamage;
        IgnoreArmor = ignoreArmor;
        AttackPower = atkMagic;
        MagicPower = atkMagic;
        AllStat = allStat;
        MaxHp = hpmp;
        MaxMp = hpmp;
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
        lhs.IgnoreArmor = (1 - ((100 - lhs.IgnoreArmor) / 100.0) * ((100 - rhs.IgnoreArmor) / 100.0) ) * 100;
        lhs.AllStat += rhs.AllStat;
        
        return lhs;
    }
    
    
}