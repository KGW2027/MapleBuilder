namespace MapleAPI.Enum;

public class MapleSymbol
{
    public enum SymbolType
    {
        YEORO          = 0x01,
        CHUCHU         = 0x02,
        LACHELEIN      = 0x03,
        ARCANA         = 0x04,
        MORAS          = 0x05,
        ESFERA         = 0x06,
        CERNIUM        = 0x11,
        ARCS           = 0x12,
        ODIUM          = 0x13,
        DOWONKYUNG     = 0x14,
        ARTERIA        = 0x15,
        CARCION        = 0x16,
        UNKNOWN        = 0x00,
    }

    public static SymbolType GetSymbolType(string name)
    {
        if (name.Equals("아케인심볼 : 소멸의 여로")) return SymbolType.YEORO;
        if (name.Equals("아케인심볼 : 츄츄 아일랜드")) return SymbolType.CHUCHU;
        if (name.Equals("아케인심볼 : 레헬른")) return SymbolType.LACHELEIN;
        if (name.Equals("아케인심볼 : 아르카나")) return SymbolType.ARCANA;
        if (name.Equals("아케인심볼 : 모라스")) return SymbolType.MORAS;
        if (name.Equals("아케인심볼 : 에스페라")) return SymbolType.ESFERA;
        if (name.Equals("어센틱심볼 : 세르니움")) return SymbolType.CERNIUM;
        if (name.Equals("어센틱심볼 : 아르크스")) return SymbolType.ARCS;
        if (name.Equals("어센틱심볼 : 오디움")) return SymbolType.ODIUM;
        if (name.Equals("어센틱심볼 : 도원경")) return SymbolType.DOWONKYUNG;
        if (name.Equals("어센틱심볼 : 아르테리아")) return SymbolType.ARTERIA;
        if (name.Equals("어센틱심볼 : 카르시온")) return SymbolType.CARCION;
        return SymbolType.UNKNOWN;
    }
}