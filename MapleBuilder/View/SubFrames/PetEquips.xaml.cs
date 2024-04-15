using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using MapleAPI.Enum;
using MapleBuilder.Control;
using MapleBuilder.View.SubObjects;

namespace MapleBuilder.View.SubFrames;

public partial class PetEquips : UserControl
{
    private PetEquipSlot[] petEquipSlots;
    private int beforePetEffect;
    
    public PetEquips()
    {
        InitializeComponent();

        petEquipSlots = new[] {ctPetEquip1, ctPetEquip2, ctPetEquip3};
        beforePetEffect = 0;
    }
    
    #region 펫 슬롯/타입 변경 시 동기화

    private void UpdatePetSetEffect()
    {
        if (GlobalDataController.Instance.PlayerInstance == null) return;
        int atkmag = 0;

        for (int setNum = 1; setNum <= 3; setNum++)
        {
            MaplePetType.PetType type = MaplePetType.PetType.OTHER;
            int count = 0;
            foreach (PetEquipSlot petSlot in petEquipSlots)
            {
                if (petSlot.PetSetNumber != setNum) continue;
                if (type == MaplePetType.PetType.OTHER) type = petSlot.PetType;
                count++;
            }

            if (count == 0) continue;
            atkmag += type switch
            {
                MaplePetType.PetType.LUNA_PETIT =>   count == 1 ? 8 : count == 2 ? 18 : 30,
                MaplePetType.PetType.LUNA_DREAM =>   count == 1 ? 7 : count == 2 ? 16 : 27,
                MaplePetType.PetType.LUNA_SWEET =>   count == 1 ? 6 : count == 2 ? 14 : 24,
                MaplePetType.PetType.WONDER_BLACK => count == 1 ? 5 : count == 2 ? 12 : 21,
                MaplePetType.PetType.OTHER =>        count == 1 ? 3 : count == 2 ?  8 : 15,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
        
        int delta = atkmag - beforePetEffect;
        // BuilderDataContainer.PlayerStatus.PlayerStat[MapleStatus.StatusType.ATTACK_POWER] += delta;
        // BuilderDataContainer.PlayerStatus.PlayerStat[MapleStatus.StatusType.MAGIC_POWER] += delta;
        // BuilderDataContainer.RefreshAll();
        ctPetSetEffectLabel.Content = $"세트효과 - 공/마 +{atkmag}";
        beforePetEffect = atkmag;
    }
    
    private void OnPetEquipSetChanged(object sender, RoutedEventArgs e)
    {
        if (!IsInitialized) return;
        PetEquipSlot slot = (PetEquipSlot) sender;

        foreach (PetEquipSlot pSlot in petEquipSlots)
        {
            if (pSlot == slot) continue;
            if (slot.PetSetNumber == pSlot.PetSetNumber) slot.PetType = pSlot.PetType;
        }

        UpdatePetSetEffect();
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
        
        UpdatePetSetEffect();
    }
    
    #endregion
}