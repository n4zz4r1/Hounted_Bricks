// using System;
// using System.Collections.Generic;
// using Core.StateMachine.AbilitySlots;
// using Core.StateMachine.Cards;
// using TMPro;
// using UnityEngine;
// using UnityEngine.Serialization;
// using UnityEngine.UI;
// using States = Core.StateMachine.AbilitySlots.States;
//
// namespace Core.Popup {
//
// public class AbilitiesPopup : BasePopup {
//     [SerializeField] public List<AbilitySlotFSM> cardsTier1;
//     [SerializeField] public List<AbilitySlotFSM> cardsTier2;
//     [SerializeField] public List<AbilitySlotFSM> cardsTier3;
//     [SerializeField] public List<AbilitySlotFSM> cardsTier4;
//
//     [SerializeField] public AbilitiesPopupComponents components;
//
//     [FormerlySerializedAs("CharacterCardFSM")] [SerializeField]
//     public CardFSM characterCardFSM;
//
//     private GameObject _currentSelected;
//     private AbilitySlotFSM SelectedAbilitySlot { get; set; }
//
//     private void Awake() {
//         cardsTier1.ForEach(item => item.Tier = 0);
//         cardsTier2.ForEach(item => item.Tier = 1);
//         cardsTier3.ForEach(item => item.Tier = 2);
//         cardsTier4.ForEach(item => item.Tier = 3);
//         components.useButton.onClick.AddListener(() => { SelectedAbilitySlot.State.Choose(SelectedAbilitySlot); });
//
//         title.text = characterCardFSM.GetCardTitle();
//     }
//
//     public void Select(bool selected, AbilitySlotFSM FSM) {
//         // CleanUp();
//
//         if (selected)
//             PrepareSelect(FSM);
//         else
//             Unselect(FSM);
//     }
//
//     private void PrepareSelect(AbilitySlotFSM FSM) {
//         if (SelectedAbilitySlot != null) SelectedAbilitySlot.State.Unselect(SelectedAbilitySlot);
//
//         SelectedAbilitySlot = FSM;
//         if (FSM.State == States.NotFound) {
//             components.textDescription.text = "Label.NotFound";
//
//             components.boxSelected.enabled = false;
//             components.boxSelectedIcon.enabled = false;
//             components.boxSelectedIcon2.enabled = false;
//
//             components.textType.gameObject.SetActive(false);
//             components.textTypeDetail.gameObject.SetActive(false);
//             components.textTypeDetail2.gameObject.SetActive(false);
//         }
//         else if (FSM.State == States.Found || FSM.State == States.InUse) {
//             PrepareCardDetail(FSM);
//         }
//
//
//         Destroy(_currentSelected);
//         _currentSelected = Instantiate(FSM.cardFSM.gameObject, components.boxSelectedCard.transform);
//         _currentSelected.transform.localPosition = new Vector3(0, 8, 0);
//         FSM.State.Select(FSM);
//     }
//
//     private void PrepareCardDetail(AbilitySlotFSM FSM) {
//         var abilityType = FSM.cardFSM.cardAbility.abilityType;
//         components.textType.text = "Label." + abilityType;
//         components.textDescription.text = FSM.cardFSM.GetCardFullDetail();
//         components.textType.gameObject.SetActive(true);
//
//         if (abilityType == AbilityType.IMPROVEMENT) {
//             components.textTypeDetail.text = "Label." + abilityType + ".Detail";
//
//             components.textTypeDetail.gameObject.SetActive(true);
//             components.textTypeDetail2.gameObject.SetActive(false);
//         }
//         else {
//             components.textTypeValue.text = FSM.cardFSM.cardAbility.value.ToString();
//             components.textTypeDetail2.text = "Label." + abilityType + ".Detail";
//
//             components.textTypeDetail.gameObject.SetActive(false);
//             components.textTypeDetail2.gameObject.SetActive(true);
//         }
//     }
//
//     private void Unselect(AbilitySlotFSM FSM) {
//         components.textDescription.text = "Label.SelectAbility";
//         Destroy(_currentSelected);
//         components.textType.gameObject.SetActive(false);
//         components.textTypeDetail.gameObject.SetActive(false);
//         components.textTypeDetail2.gameObject.SetActive(false);
//
//         components.boxSelected.enabled = true;
//         components.boxSelectedIcon.enabled = true;
//         components.boxSelectedIcon2.enabled = true;
//
//         FSM.State.Unselect(FSM);
//     }
// }
//
// [Serializable]
// public class AbilitiesPopupComponents {
//     [FormerlySerializedAs("BoxSelectedCard")]
//     public GameObject boxSelectedCard;
//
//     [FormerlySerializedAs("TextDescription")]
//     public TextMeshProUGUI textDescription;
//
//     [FormerlySerializedAs("TextType")] public TextMeshProUGUI textType;
//
//     [FormerlySerializedAs("TextTypeValue")]
//     public TextMeshProUGUI textTypeValue;
//
//     [FormerlySerializedAs("TextTypeDetail")]
//     public TextMeshProUGUI textTypeDetail;
//
//     [FormerlySerializedAs("TextTypeDetail2")]
//     public TextMeshProUGUI textTypeDetail2;
//
//     [FormerlySerializedAs("BoxSelected")] public Image boxSelected;
//
//     [FormerlySerializedAs("BoxSelectedIcon")]
//     public Image boxSelectedIcon;
//
//     [FormerlySerializedAs("BoxSelectedIcon2")]
//     public Image boxSelectedIcon2;
//
//     [FormerlySerializedAs("UseButton")] public Button useButton;
// }
//
// }