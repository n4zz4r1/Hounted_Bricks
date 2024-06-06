using System;
using System.Collections.Generic;
using Core.Data;
using Core.Utils.Constants;
using Framework.Base;
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

namespace Core.StateMachine.CharacterSelectBox {
public class CharacterSelectBoxFSM : StateMachine<CharacterSelectBoxFSM, State<CharacterSelectBoxFSM>> {
    [SerializeField] public Components components;
    [SerializeField] public Card character = Card.Card_005_Char_Lucas;

    protected override CharacterSelectBoxFSM FSM => this;
    protected override State<CharacterSelectBoxFSM> GetInitialState => States.Preload;

    private int MaxHeart { get; } = 3;
    // public DateTime LastDateTime { get; set; } = new ();

    protected override void SyncDataBase() {
        if (!CardsDataV1.Instance.HasCard(character)) {
            ChangeState(States.NotFound);
            return;
        }

        var hearts = PlayerDataV1.Instance.GetLife(character);
        RefreshHearts(hearts);
        // LastDateTime = PlayerDataV1.Instance.GetLastMatchTime(character);

        if (hearts > 0)
            if (PlayerDataV1.Instance.selectedCharacter == character)
                ChangeState(States.Selected);
            else
                ChangeState(States.NotSelected);
        else
            ChangeState(States.Dead);
    }

    protected override void Before() {
        components.enabledButton.onClick.AddListener(() => { State.Choose(FSM); });
    }

    // public void UpdateTime()
    // {
    //     var timeLeft = TimeServices.timeForNewLife - (TimeServices.now() - FSM.LastDateTime);
    //     if (timeLeft.TotalMilliseconds < 0)
    //     {
    //         State.AddLife(FSM);
    //         PlayerDataV1.Instance.SetLastMatchTime(character, FSM.LastDateTime + TimeServices.timeForNewLife);
    //         LastDateTime = PlayerDataV1.Instance.GetLastMatchTime(character);
    //     }
    // }

    internal void RefreshHearts(int hearts) {
        for (var i = 0; i < hearts; i++)
            FSM.components.hearts[i].color = Colors.WHITE;
        for (var i = hearts; i < FSM.MaxHeart; i++)
            FSM.components.hearts[i].color = Colors.BLACK;
    }


    // public void TakeHit()
    // {
    //     State.TakeHit(FSM);
    // }
    // public void Cure()
    // {
    //     State.AddLife(FSM);
    // }
}

[Serializable]
public class Components {
    [SerializeField] public List<Image> hearts;
    [SerializeField] public GameObject enabledBox;
    [SerializeField] public GameObject disabledBox;
    [SerializeField] public GameObject notFoundBox;
    [SerializeField] public Button enabledButton;
}
}