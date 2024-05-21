using Framework.Base;

namespace Core.Controller.AboutUs
{
    public abstract class States {
        public static readonly Started Started = new();
    }

    public class Started : State<AboutUsController> { }
}