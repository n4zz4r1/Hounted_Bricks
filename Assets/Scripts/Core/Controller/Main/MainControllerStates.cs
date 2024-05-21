using Framework.Base;
using UnityEngine;
using Application = UnityEngine.Device.Application;

namespace Core.Controller.Main {

public abstract class States {
    public static readonly Created Created = new();
}

public class Created : State<MainController> {
    public override void Before(MainController fsm) {
        // Set default FPS to 70
        Application.targetFrameRate = 70;
        Debug.Log("Target Frame Rate: " + Application.targetFrameRate);
    }
}

}