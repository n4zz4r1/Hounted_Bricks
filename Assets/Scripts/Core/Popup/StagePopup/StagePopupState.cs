using Framework.Base;

namespace Core.Popup.StagePopup {

public abstract class States {
    public static readonly Started Started = new();
}

public class Started : State<StagePopup> { }

}