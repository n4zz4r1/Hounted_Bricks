using Framework.Base;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Controller.Preload {
/**
 * Refactor: ok
 */
public class PreloadController : Controller<PreloadController, State<PreloadController>> {
    [SerializeField] public Slider progressBar;

    protected override PreloadController FSM => this;
    protected override State<PreloadController> GetInitialState => States.Created;
}
}