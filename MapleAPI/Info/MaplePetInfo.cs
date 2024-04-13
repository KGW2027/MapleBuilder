using System.Text.Json.Nodes;
using MapleAPI.DataType;
using MapleAPI.DataType.Item;
using MapleAPI.Enum;

namespace MapleAPI.Info;

public class MaplePetInfo : MapleInfo
{
    public MaplePetInfo(string ocid, CharacterInfo parent) : base(ocid, parent)
    {
        PetItems = new List<MaplePetItem>();
    }

    private protected override APIRequestType GetRequestType()
    {
        return APIRequestType.PET;
    }

    public readonly List<MaplePetItem> PetItems;

    private protected override void ParseInfo()
    {
        for (int index = 1; index <= 3; index++)
        {
            if (!TryGet($"pet_{index}_equipment", out JsonObject? petEquip) || petEquip == null) continue;
            
            string? petTypeNode = Get<string>($"pet_{index}_pet_type");
            MaplePetType.PetType petType = petTypeNode == null
                ? MaplePetType.PetType.OTHER
                : MaplePetType.GetPetType(petTypeNode);
            
            PetItems.Add(new MaplePetItem(petEquip, petType));
        }
    }
}