using System;
using System.Threading.Tasks;
using Core.Data;
using Core.Utils;
using Framework.Base;
using TMPro;
using UnityEngine;
using Image = UnityEngine.UI.Image;

namespace Core.StateMachine.Resource {

/**
 * Refactor: TODO
 */
public class ResourceFSM : StateMachine<ResourceFSM, State<ResourceFSM>> {
    [SerializeField] public ResourceType resourceType;
    [SerializeField] public Components components;

    protected override ResourceFSM FSM => this;
    protected override State<ResourceFSM> GetInitialState => States.Created;

    protected override async Task BeforeAsync() {
        components.resourceIcon.sprite = await AssetLoader<Sprite, ResourceType>.Load(resourceType);
        components.quantityText.text = ResourcesV1.Instance.GetResourcesAmount(resourceType).ToString();
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