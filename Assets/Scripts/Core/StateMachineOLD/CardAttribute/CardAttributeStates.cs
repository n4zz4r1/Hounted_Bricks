using Framework.Base;

namespace Core.StateMachine.CardAttribute {

public abstract class States {
    public static readonly Created Created = new();
}

public class Created : State<CardAttributeFSM> {
    public override void Enter(CardAttributeFSM fsm) { }
}

}