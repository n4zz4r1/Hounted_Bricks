using System;
using System.Collections.Generic;
using Core.Data;
using Core.Popup;
using Core.StateMachine.Cards;
using Core.Utils.Constants;
using Framework.Base;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Core.StateMachine.CharacterCards {

public class CharacterCardFSM : StateMachine<CharacterCardFSM, State<CharacterCardFSM>> {
    [SerializeField] public Card card;

    [FormerlySerializedAs("AbilitiesPopup")] [SerializeField]
    public AbilitiesPopup abilitiesPopup;

    [FormerlySerializedAs("Internal")] [SerializeField]
    public Components components;

    protected override CharacterCardFSM FSM => this;
    protected override State<CharacterCardFSM> GetInitialState => States.Created;

    protected override void Before() {
        components.buttonFoundBox.onClick.AddListener(() => State.Select(FSM));
        components.buttonAbility.onClick.AddListener(OpenPopup);
        // originalSprite = 
        FillAbilityIcons();
    }

    protected override void SyncDataBase() {
        FillAbilityIcons();
    }

    private void FillAbilityIcons() {
        for (var i = 0; i < components.abilitiesSlotIcons.Count; i++) {
            var characterIndex = PlayerDataV1.Instance.GetIndexByCharacter(card);
            var abilityCard = PlayerDataV1.Instance.SavedAbilities[characterIndex][i];

            if (abilityCard is not Card.NONE) {
                components.abilitiesSlotIcons[i].enabled = true;
                components.abilitiesSlotIcons[i].sprite = CardFSM.GetCardIcon(abilityCard);
            }
            else {
                components.abilitiesSlotIcons[i].enabled = false;
            }
        }
    }

    private new void OpenPopup() {
        abilitiesPopup.gameObject.SetActive(true);
    }
}

[Serializable]
public class Components {
    [FormerlySerializedAs("BoxNotFound")] [SerializeField]
    public GameObject boxNotFound;

    [FormerlySerializedAs("BoxFound")] [SerializeField]
    public GameObject boxFound;

    [SerializeField] public Shadow boxFoundShadow;
    [SerializeField] public Image boxFoundImage;
    [SerializeField] public CanvasGroup boxFoundCanvas;

    [SerializeField] public Image icon;
    [SerializeField] public Sprite iconSprite;
    [SerializeField] public Sprite iconSpriteBW;

    [FormerlySerializedAs("ButtonFoundBox")] [SerializeField]
    public Button buttonFoundBox;

    [FormerlySerializedAs("ButtonAbility")] [SerializeField]
    public Button buttonAbility;

    [FormerlySerializedAs("AbilitiesSlotIcons")] [SerializeField]
    public List<Image> abilitiesSlotIcons;

    [FormerlySerializedAs("StarsStrenght")] [SerializeField]
    public List<Image> starsStrength;

    [FormerlySerializedAs("StarsMagic")] [SerializeField]
    public List<Image> starsMagic;

    [FormerlySerializedAs("StarsStamina")] [SerializeField]
    public List<Image> starsStamina;

    [FormerlySerializedAs("StarsLife")] [SerializeField]
    public List<Image> starsLife;
}

}