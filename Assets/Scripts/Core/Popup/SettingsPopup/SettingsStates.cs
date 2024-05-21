using Framework;
using Framework.Base;

namespace Core.Popup.SettingsPopup
{
    
    public abstract class States {
        public static readonly Started Started = new();
    }

    public class Started : State<SettingsPopupFSM>
    {
        
    }
    
}