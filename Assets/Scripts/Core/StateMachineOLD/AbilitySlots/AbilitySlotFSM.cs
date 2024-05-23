// using System;
// using Core.Popup;
// using Core.StateMachine.Cards;
// using Core.Utils.Constants;
// using Framework.Base;
// using UnityEngine;
// using UnityEngine.Serialization;
// using UnityEngine.UI;
//
// namespace Core.StateMachine.AbilitySlots {
//
// public class AbilitySlotFSM : StateMachine<AbilitySlotFSM, State<AbilitySlotFSM>> {
//     [FormerlySerializedAs("CardFSM")] [SerializeField]
//     public CardFSM cardFSM;
//
//     [FormerlySerializedAs("Internal")] [SerializeField]
//     public Components components;
//
//     protected override AbilitySlotFSM FSM => this;
//     protected override State<AbilitySlotFSM> GetInitialState => States.Started;
//
//     private bool Selected { get; set; }
//     public AbilitiesPopup ParentPopup { get; private set; }
//
//     public int Tier { get; set; }
//
//     public void Select() {
//         if (State == States.InUse)
//             ParentPopup.components.useButton.gameObject.SetActive(false);
//         else if (State == States.Found)
//             ParentPopup.components.useButton.gameObject.SetActive(true);
//         else
//             ParentPopup.components.useButton.gameObject.SetActive(false);
//
//         components.iconSelected.color = Colors.PRIMARY;
//         components.iconSelected.enabled = true;
//         Selected = true;
//     }
//
//     public void Unselect() {
//         components.iconSelected.enabled = false;
//         Selected = false;
//         ParentPopup.components.useButton.gameObject.SetActive(false);
//     }
//
//     protected override void Before() {
//         FSM.components.buttonSelect.onClick.AddListener(() => { ParentPopup.Select(!Selected, FSM); });
//         ParentPopup = GetComponentInParent<AbilitiesPopup>();
//     }
// }
//
//
// [Serializable]
// public class Components {
//     [FormerlySerializedAs("IconSelected")] public Image iconSelected;
//     [FormerlySerializedAs("ButtonSelect")] public Button buttonSelect;
//     [FormerlySerializedAs("BoxFound")] public GameObject boxFound;
//     [FormerlySerializedAs("BoxNotFound")] public GameObject boxNotFound;
//     [FormerlySerializedAs("Icon")] public Image icon;
// }
//
// }