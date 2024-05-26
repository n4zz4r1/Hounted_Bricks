using System;
using System.Collections.Generic;
using Core.Data;
using Core.Services;
using Core.Utils.Constants;
using Framework.Base;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.StateMachine.CharacterLifeBar {

public class CharacterLifeBarFSM : StateMachine<CharacterLifeBarFSM, CharacterLifeBarState> {
    [SerializeField] public Components components;
    [SerializeField] public Card character = Card.Card_005_Char_Lucas;

    protected override CharacterLifeBarFSM FSM => this;
    protected override CharacterLifeBarState GetInitialState => States.Created;

    public int MaxHeart { get; set; } = 3;
    public DateTime LastDateTime { get; set; }

    public void UpdateTime() {
        var timeLeft = TimeServices.TimeForNewLife - (TimeServices.Now() - FSM.LastDateTime);
        components.elapsedTime.text = TimeServices.FormatTimeSpan(timeLeft);
        if (!(timeLeft.TotalMilliseconds < 0)) return;

        State.AddLife(FSM);
        PlayerDataV1.Instance.SetLastMatchTime(character, FSM.LastDateTime + TimeServices.TimeForNewLife);
        LastDateTime = PlayerDataV1.Instance.GetLastMatchTime(character);
    }

    public void TakeHit() {
        State.TakeHit(FSM);
    }

    public void Cure() {
        State.AddLife(FSM);
    }
}

[Serializable]
public class Components {
    [SerializeField] public List<Image> hearts;
    [SerializeField] public TextMeshProUGUI elapsedTime;
    [SerializeField] public Image characterEnabledIcon;
    [SerializeField] public Image characterDisabledIcon;
}

}