using Framework.Base;

namespace Core.StateMachine.CardAttribute {

public abstract class States {
    public static readonly Preload Preload = new();
}

public class Preload : State<CardAttributeFSM> {
    public override void Enter(CardAttributeFSM fsm) { }
}

}