using System.Windows;
using System.Windows.Controls;
using MapleBuilder.View.SubObjects;

namespace MapleBuilder.View.SubFrames;

public partial class PetEquips : UserControl
{
    private PetEquipSlot[] petEquipSlots;
    
    public PetEquips()
    {
        InitializeComponent();

        petEquipSlots = new[] {ctPetEquip1, ctPetEquip2, ctPetEquip3};
    }
    
    #region 펫 슬롯/타입 변경 시 동기화
    
    private void OnPetEquipSetChanged(object sender, RoutedEventArgs e)
    {
        if (!IsInitialized) return;
        PetEquipSlot slot = (PetEquipSlot) sender;

        foreach (PetEquipSlot pSlot in petEquipSlots)
        {
            if (pSlot == slot) continue;
            if (slot.PetSetNumber == pSlot.PetSetNumber) slot.PetType = pSlot.PetType;
        }

    }

    private void OnPetTypeChanged(object sender, RoutedEventArgs e)
    {
        if (!IsInitialized) return;
        PetEquipSlot slot = (PetEquipSlot) sender;
        
        foreach (PetEquipSlot pSlot in petEquipSlots)
        {
            if (pSlot == slot) continue;
            if (slot.PetSetNumber == pSlot.PetSetNumber) pSlot.PetType = slot.PetType;
        }
    }
    
    #endregion
}