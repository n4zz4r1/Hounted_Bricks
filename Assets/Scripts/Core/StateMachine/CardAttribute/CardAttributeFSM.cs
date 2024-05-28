using System;
using System.Globalization;
using Core.StateMachine.Cards;
using Core.Utils.Constants;
using Framework.Base;
using TMPro;
using UnityEngine;
using Color = UnityEngine.Color;

namespace Core.StateMachine.CardAttribute {

public class CardAttributeFSM : StateMachine<CardAttributeFSM, State<CardAttributeFSM>> {
    [SerializeField] public Cards.CardAttribute attribute;
    [SerializeField] public Components components;

    private CardFSM _cardFSM;
    protected override CardAttributeFSM FSM => this;
    protected override State<CardAttributeFSM> GetInitialState => States.Preload;

    public static GameObject Create(CardFSM cardFSM, Cards.CardAttribute attribute,
        Transform transform, Vector3 position) {
        var cardAttributePrefab = Resources.Load("CardAttribute" + attribute) as GameObject;
        var cardAttribute = Instantiate(cardAttributePrefab, transform);
        cardAttribute.GetComponent<CardAttributeFSM>()._cardFSM = cardFSM;
        cardAttribute.GetComponent<CardAttributeFSM>().SyncDataBase();
        cardAttribute.transform.localPosition = position;
        return cardAttribute;
    }

    protected override void SyncDataBase() {
        var cardAttributeComponent = _cardFSM.attributes.Find(a => a.attribute == attribute);

        // When rarity, only set next value
        if (attribute == Cards.CardAttribute.RARITY) {
            if (cardAttributeComponent.ValueOfLevel(_cardFSM.Level() + 1) == 0f) {
                components.currentText.text = RarityUtils.From(_cardFSM.Rarity).Label;
            }
            else {
                components.currentText.color = Color.yellow;
                components.currentText.text = RarityUtils.From(_cardFSM.Rarity + 1).Label;
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
}

}