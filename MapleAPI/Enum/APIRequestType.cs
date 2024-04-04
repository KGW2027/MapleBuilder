namespace MapleAPI;

public class APIRequestType
{
    private string prefix;

    private APIRequestType(string prefix)
    {
        this.prefix = prefix;
    }

    public override string ToString()
    {
        return prefix;
    }
    
    #region 캐릭터
    public static APIRequestType OCID           = new("v1/id");
    public static APIRequestType BASIC          = new("v1/character/basic");
    public static APIRequestType POPULARITY     = new("v1/character/popularity");
    public static APIRequestType STAT           = new("v1/character/stat");
    public static APIRequestType HYPER_STAT     = new("v1/character/hyper-stat");
    public static APIRequestType PROPENSITY     = new("v1/character/propensity");
    public static APIRequestType ABILITY        = new("v1/character/ability");
    public static APIRequestType ITEM           = new("v1/character/item-equipment");
    public static APIRequestType CASH_ITEM      = new("v1/character/cashitem-equipment");
    public static APIRequestType SYMBOL         = new("v1/character/symbol-equipment");
    public static APIRequestType SET_EFFECT     = new("v1/character/set-effect");
    public static APIRequestType BEAUTY         = new("v1/character/beauty-equipment");
    public static APIRequestType ANDROID        = new("v1/character/android-equipment");
    public static APIRequestType PET            = new("v1/character/pet-equipment");
    public static APIRequestType SKILL          = new("v1/character/skill");
    public static APIRequestType LINK_SKILL     = new("v1/character/link-skill");
    public static APIRequestType V_MATRIX       = new("v1/character/vmatrix");
    public static APIRequestType HEXA_MATRIX    = new("v1/character/hexamatrix");
    public static APIRequestType HEXA_STAT      = new("v1/character/hexamatrix-stat");
    public static APIRequestType DOJANG         = new("v1/character/dojang");
    #endregion
    
    #region 유니온
    public static APIRequestType UNION          = new("v1/user/union");
    public static APIRequestType UNION_RADIER   = new("v1/user/union-raider");
    public static APIRequestType UNION_ARTIFACT = new("v1/user/union-artifact");
    #endregion
    
    #region 길드
    public static APIRequestType GUILD          = new("v1/guild/id");
    public static APIRequestType GUILD_BASIC    = new("v1/guild/basic");
    #endregion
    
    #region 계정
    public static APIRequestType LEGACY_ACCOUNT = new("legacy/ouid");
    public static APIRequestType ACCOUNT        = new("v1/ouid");
    #endregion
    
    #region 스타포스
    public static APIRequestType STARFORCE      = new("v1/history/starforce");
    #endregion
    
    #region 잠재능력
    public static APIRequestType POTENTIAL      = new("v1/history/potential");
    public static APIRequestType CUBE           = new("v1/history/cube");
    #endregion
    
    #region 랭킹
    public static APIRequestType RANK_OVERALL   = new("v1/ranking/overall");
    public static APIRequestType RANK_UNION     = new("v1/ranking/union");
    public static APIRequestType RANK_GUILD     = new("v1/ranking/guild");
    public static APIRequestType RANK_DOJANG    = new("v1/ranking/dojang");
    public static APIRequestType RANK_SEED      = new("v1/ranking/theseed");
    public static APIRequestType RANK_ACHIVE    = new("v1/ranking/achievement");
    #endregion
    
    
}