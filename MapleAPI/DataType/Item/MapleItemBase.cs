using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Nodes;
using MapleAPI.Enum;

namespace MapleAPI.DataType.Item;

public class MapleItemBase
{
    #region Constructors
    
    private protected MapleItemBase(JsonObject jsonObject)
    {
        internalData = jsonObject;
        Name = "";
        DisplayName = "";
        Status = new MapleStatContainer();
        MaxUpgrade = 0;
        CanHasAdditional = true;
        CanHasPotential = true;
    }

    private string? hash;
    public string Hash
    {
        get
        {
            if (hash != null) return hash;
            byte[] bytes = Encoding.UTF8.GetBytes(internalData.ToString());
            byte[] hashBytes;
            using SHA256 sha256 = SHA256.Create();
            hashBytes = sha256.ComputeHash(bytes);
            hash = "";
            foreach (byte b in hashBytes)
                hash += b.ToString("x2");
            return hash;
        }
    }
    
    #endregion
    
    private protected JsonObject internalData;
    
    public string Name { get; private protected set; }
    public int MaxUpgrade { get; private protected set; }
    public MapleStatContainer Status { get; private protected set; }
    public MapleEquipType.EquipType EquipType { get; private protected set; }
    public bool CanHasPotential { get; private protected set; }
    public bool CanHasAdditional { get; private protected set; }
    public bool IsEmpty { get; private protected set; }
    
    public string DisplayName { get; set; }
}