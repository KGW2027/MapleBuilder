namespace MapleAPI.Enum;

public class MapleSymbol
{
    public enum SymbolType
    {
        YEORO,
        CHUCHU,
        LACHELEIN,
        ARCANA,
        MORAS,
        ESFERA,
        CERNIUM,
        ARCS,
        ODIUM,
        DOWONKYUNG,
        ARTERIA,
        CARCION,
        UNKNOWN
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