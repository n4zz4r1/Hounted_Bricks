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

    internal CardFSM CardFSM;
    protected override CardAttributeFSM FSM => this;
    protected override State<CardAttributeFSM> GetInitialState => States.Created;

    public static GameObject Create(CardFSM cardFSM, Cards.CardAttribute attribute,
        Transform transform, Vector3 position) {
        // Debug.Log("CardAttribute" + attribute.ToString());
        var cardAttributePrefab = Resources.Load("CardAttribute" + attribute) as GameObject;
        var cardAttribute = Instantiate(cardAttributePrefab, transform);
        cardAttribute.GetComponent<CardAttributeFSM>().CardFSM = cardFSM;
        cardAttribute.GetComponent<CardAttributeFSM>().SyncDataBase();
        cardAttribute.transform.localPosition = position;
        return cardAttribute;
    }

    protected override void SyncDataBase() {
        var cardAttributeComponent = CardFSM.attributes.Find(a => a.attribute == attribute);

        // When rarity, only set next value
        if (attribute == Cards.CardAttribute.RARITY) {
            if (cardAttributeComponent.ValueOfLevel(CardFSM.Level() + 1) == 0f) {
                components.currentText.text = RarityUtils.From(CardFSM.Rarity).Label;
            }
            else {
                components.currentText.color = Color.yellow;
                components.currentText.text = RarityUtils.From(CardFSM.Rarity + 1).Label;
            }
        }
        else {
            components.currentText.text = cardAttributeComponent.ConcatValue(CardFSM.Level())
                .ToString(CultureInfo.InvariantCulture);
            var nextValue = cardAttributeComponent.ValueOfLevel(CardFSM.Level() + 1);
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