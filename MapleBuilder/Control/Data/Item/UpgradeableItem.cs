// using System.Linq;
// using MapleAPI.Enum;
//
// namespace MapleBuilder.Control.Data.Item;
//
// public class UpgradeableItem : ItemBase
// {
//     public UpgradeableItem() : base(equipType, name)
//     {
//         
//     }
//
//     public bool CanStarforce
//     {
//         get
//         {
//             switch (EquipType)
//             {
//                 case MapleEquipType.EquipType.POCKET or MapleEquipType.EquipType.MEDAL
//                     or MapleEquipType.EquipType.BADGE:
//                 case MapleEquipType.EquipType.RING when IsEventRing() || IsSpecialRing():
//                     return false;
//                 default:
//                     return true;
//             }
//         }
//     }
//     
//     public bool CanPotential
//     {
//         get
//         {
//             switch (EquipType)
//             {
//                 case MapleEquipType.EquipType.POCKET or MapleEquipType.EquipType.MEDAL
//                     or MapleEquipType.EquipType.BADGE:
//                 case MapleEquipType.EquipType.RING when IsSpecialRing():
//                     return false;
//                 default:
//                     return true;
//             }
//         }
//     }
//
//     public bool CanUpgrade
//     {
//         get
//         {
//             switch (EquipType)
//             {
//                 case MapleEquipType.EquipType.POCKET or MapleEquipType.EquipType.MEDAL
//                     or MapleEquipType.EquipType.BADGE or MapleEquipType.EquipType.EMBLEM:
//                 case MapleEquipType.EquipType.SUB_WEAPON when !UniqueName.EndsWith("블레이드") && !UniqueName.EndsWith("실드"): // 보강 필요
//                 case MapleEquipType.EquipType.RING when IsSpecialRing() || IsEventRing():
//                     return false;
//                 default:
//                     return true;
//             }
//         }
//     }
//
//     public bool CanSoul => EquipType == MapleEquipType.EquipType.WEAPON;
//
//     public bool CanAddOption
//     {
//         get
//         {
//             switch (EquipType)
//             {
//                 case MapleEquipType.EquipType.MEDAL or MapleEquipType.EquipType.BADGE or MapleEquipType.EquipType.EMBLEM:
//                 case MapleEquipType.EquipType.SUB_WEAPON when !UniqueName.EndsWith("블레이드") && !UniqueName.EndsWith("실드"): // 보강 필요
//                 case MapleEquipType.EquipType.RING:
//                     return false;
//                 default:
//                     return true;
//             }
//         }   
//     }
//
//     private readonly string[] eventRings = {
//         "테네브리스 원정대 반지", "글로리온 링 : 슈프림", "어웨이크 링",
//         "카오스링", "어드벤처 딥다크 크리티컬 링", "어비스 헌터스 링",
//         "SS급 마스터 쥬얼링", "결속의 반지", "오닉스 링 \"완성\"", "벤젼스 링", "코스모스 링"
//     };
//
//     private readonly string[] specialRings = {
//         "리스트레인트 링", "리스크테이커 링", "크라이시스 - HM링", "크라이시스 - H링", "크라이시스 - M링",
//         "웨폰퍼프 - S링", "웨폰퍼프 - D링", "웨폰퍼프 - I링", "웨폰퍼프 - L링",
//         "레벨퍼프 - S링", "레벨퍼프 - D링", "레벨퍼프 - I링", "레벨퍼프 - L링",
//         "링 오브 썸", "듀라빌리티 링", "얼티메이덤 링", "크리데미지 링", "스위프트 링", "컨티뉴어스 링",
//         "헬스컷 링", "마나컷 링", "리밋 링", "실드스와프 링", "크리쉬프트 링", "스탠스쉬프트 링", "타워인헨스 링",
//         "오버패스 링", "리플렉티브 링", "버든리프트 링", "리커버디펜스 링", "리커버스탠스 링"
//     };
//     
//     private bool IsEventRing()
//     {
//         return eventRings.Contains(UniqueName);
//     }
//
//     private bool IsSpecialRing()
//     {
//         return specialRings.Contains(UniqueName);
//     }
//
// }