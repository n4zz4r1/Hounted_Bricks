using Framework.Base;

namespace Core.Controller.Home {
public abstract class States {
    public static readonly Created Created = new();
}

public class Created : State<HomeController> { }
}