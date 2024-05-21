using Framework.Base;
using Framework.Constants;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Controller.AboutUs
{
    public class AboutUsController : Controller<AboutUsController, State<AboutUsController>> {
        protected override AboutUsController FSM => this;
        protected override State<AboutUsController> GetInitialState => States.Started;

        [SerializeField] public Button closeButton;

        protected override void Before() {
            closeButton.onClick.AddListener(Close);
        }

        private void Close() {
            Debug.Log("closing");
            TransitionWithEffectTo(GameScenes.PreloadScene.ToString());
        }
    }
}
