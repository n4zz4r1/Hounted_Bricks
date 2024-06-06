using System;
using System.Threading.Tasks;
using Core.Data;
using Core.Sprites;
using Core.Utils;
using Framework.Base;
using TMPro;
using UnityEngine;
using Image = UnityEngine.UI.Image;

namespace Core.StateMachine.Resource {
public class ResourceFSM : StateMachine<ResourceFSM, State<ResourceFSM>> {
    [SerializeField] public ResourceType resourceType;
    [SerializeField] public Components components;

    protected override ResourceFSM FSM => this;
    protected override State<ResourceFSM> GetInitialState => States.Preload;

    protected override async Task BeforeAsync() {
        var texture2d = await AssetLoader<ResourceType>.Load<Texture2D>(resourceType);
        components.resourceIcon.sprite = ConvertToSprite(texture2d);
        components.quantityText.text = ResourcesV1.Instance.GetResourcesAmount(resourceType).ToString();
    }

    private Sprite ConvertToSprite(Texture2D texture) {
        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }

    protected override void SyncDataBase() {
        components.quantityText.text = ResourcesV1.Instance.GetResourcesAmount(resourceType).ToString();
    }
}

[Serializable]
public class Components {
    [SerializeField] public TextMeshProUGUI quantityText;
    [SerializeField] public Image resourceIcon;
}
}