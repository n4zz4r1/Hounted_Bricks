// using Com.UruStudios.State.MonsterHBState;

using Framework.Base;
using Game.StateMachine.Monster;
using UnityEngine;
using UnityEngine.UI;

namespace Game.StateMachine.MonsterHB {
public class MonsterHBFSM : StateMachine<MonsterHBFSM, State<MonsterHBFSM>> {
    [SerializeField] public MonsterFSM monsterFSM;
    [SerializeField] public RectTransform rect;
    [SerializeField] public RawImage rawImage;

    protected override MonsterHBFSM FSM => this;
    protected override State<MonsterHBFSM> GetInitialState => States.FULL;
}
}