using System;
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

    public new async void Awake() {
        // update icon, label and text based on its type
        components.resourceIcon.sprite = await Sprites<Sprite, ResourceType>.LoadAssetAsync(resourceType);
        components.quantityText.text = ResourcesV1.Instance.GetResourcesAmount(resourceType).ToString();

        base.Awake();
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