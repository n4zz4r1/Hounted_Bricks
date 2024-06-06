using System;
using System.Collections.Generic;
using Core.Utils.Constants;
using Framework.Base;
using UnityEngine;
using UnityEngine.UI;

namespace Core.StateMachine.CharacterCards {
public class CharacterCardFSM : StateMachine<CharacterCardFSM, State<CharacterCardFSM>> {
    [SerializeField] public Card card;
    [SerializeField] public Components components;

    protected override CharacterCardFSM FSM => this;
    protected override State<CharacterCardFSM> GetInitialState => States.Preload;

    protected override void Before() {
        components.buttonFoundBox.onClick.AddListener(() => State.Select(FSM));
        components.buttonAbility.onClick.AddListener(OpenPopup);
    }
}

[Serializable]
public class Components {
    [SerializeField] public GameObject boxNotFound;
    [SerializeField] public GameObject boxFound;
    [SerializeField] public Shadow boxFoundShadow;
    [SerializeField] public Image boxFoundImage;
    [SerializeField] public CanvasGroup boxFoundCanvas;
    [SerializeField] public Image icon;
    [SerializeField] public Sprite iconSprite;
    [SerializeField] public Sprite iconSpriteBW;
    [SerializeField] public Button buttonFoundBox;
    [SerializeField] public Button buttonAbility;
    [SerializeField] public List<Image> abilitiesSlotIcons;
    [SerializeField] public List<Image> starsStrength;
    [SerializeField] public List<Image> starsMagic;
    [SerializeField] public List<Image> starsStamina;
    [SerializeField] public List<Image> starsLife;
}
}