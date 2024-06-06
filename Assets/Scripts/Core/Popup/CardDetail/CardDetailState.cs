using Framework.Base;

namespace Core.Popup.CardDetail {
public abstract class States {
    public static readonly Started STARTED = new();
}

public class Started : State<CardDetailPopup> { }
}