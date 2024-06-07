using Framework.Base;

namespace Game.StateMachine.BuffItems {
public abstract class States {
    public static readonly Preload Preload = new();
    public static readonly Created Created = new();
}

public class Created : State<BuffItemFSM> {
    public override void Increase(BuffItemFSM itemFSM, int value = 1) {
        var totalLeft = itemFSM.Counter.Add(value);

        // Change Text
        itemFSM.ChangeText(totalLeft);
    }
}

public class Preload : State<BuffItemFSM> {
    public override void Prepare(BuffItemFSM itemFSM) {
        itemFSM.Counter.Value = 1;
        itemFSM.ChangeText(itemFSM.Counter.Value);

        // text color
        itemFSM.PrintText();
        itemFSM.ChangeState(States.Created);
    }
}
}