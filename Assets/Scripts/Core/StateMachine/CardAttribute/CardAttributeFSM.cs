using System;
using System.Globalization;
using Core.Sprites;
using Core.StateMachine.Cards;
using Core.Utils;
using Core.Utils.Constants;
using Framework.Base;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Color = UnityEngine.Color;

namespace Core.StateMachine.CardAttribute {

public class CardAttributeFSM : StateMachine<CardAttributeFSM, State<CardAttributeFSM>> {
    [SerializeField] public CardAttributeType attributeType;
    [SerializeField] public Components components;

    private CardFSM _cardFSM;
    protected override CardAttributeFSM FSM => this;
    protected override State<CardAttributeFSM> GetInitialState => States.Preload;

    public static GameObject Create(CardFSM cardFSM, CardAttributeType attributeType,
        Transform transform, Vector3 position) {
        var cardAttributePrefab = AssetLoader.AsGameObject(attributeType);
        var cardAttribute = Instantiate(cardAttributePrefab, transform);
        cardAttribute.GetComponent<CardAttributeFSM>()._cardFSM = cardFSM;
        cardAttribute.GetComponent<CardAttributeFSM>().SyncDataBase();
        cardAttribute.transform.localPosition = position;
        return cardAttribute;
    }

    protected override void SyncDataBase() {
        var cardAttributeComponent = _cardFSM.attributes.Find(a => a.attributeType == attributeType);

        // When rarity, only set next value
        if (attributeType == CardAttributeType.Rarity) {
            if (cardAttributeComponent.ValueOfLevel(_cardFSM.Level() + 1) == 0f) {
                components.descriptionText.gameObject.SetActive(false);
                components.currentText.gameObject.SetActive(false);
                // components.currentText.text = RarityUtils.From(_cardFSM.Rarity).Label;
            } else {
                var rarity = _cardFSM.Rarity + 1;
                components.descriptionText.gameObject.SetActive(true);
                components.currentText.gameObject.SetActive(true);
                components.currentText.color = Color.yellow;
                components.currentText.text = LocalizationUtils.From(rarity);
            }
        }
        else {
            components.currentText.text = cardAttributeComponent.ConcatValue(_cardFSM.Level())
                .ToString(CultureInfo.InvariantCulture);
            var nextValue = cardAttributeComponent.ValueOfLevel(_cardFSM.Level() + 1);
            components.nextValueText.text =
                nextValue != 0f ? "+" + nextValue.ToString(CultureInfo.InvariantCulture) : "";
        }
    }
}

[Serializable]
public class Components {
    [SerializeField] public TextMeshProUGUI currentText;
    [SerializeField] public TextMeshProUGUI nextValueText;
    [SerializeField] public TextMeshProUGUI descriptionText;
}

}