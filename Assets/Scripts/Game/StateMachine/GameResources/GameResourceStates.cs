using Core.Handler;
using Core.Utils;
using Core.Utils.Constants;
using DG.Tweening;
using Framework.Base;
using UnityEngine;

namespace Game.StateMachine.GameResources {
public abstract class States {
    public static readonly Preload Preload = new();
    public static readonly Created Created = new();
}

public class Created : State<GameResourceFSM> {
    public override bool Decrease(GameResourceFSM fsm, int value = 1) {
        var totalLeft = fsm.Counter.Subtract(value);

        // remove icons
        if (!fsm.OneAssetOnly) {
            if (totalLeft + value <= 0 || fsm.AllEmpty) {
                fsm.AllEmpty = true;
                return true;
            }

            if (totalLeft < 0)
                totalLeft = 0;

            for (var i = totalLeft; i < totalLeft + value; i++)
                fsm.SpriteIcons[i].rectTransform.DOScale(0f, GameResourceFSM.EffectTime);
        }

        // Change Text
        fsm.ChangeText(totalLeft);
        return totalLeft == 0;
    }

    public override void Increase(GameResourceFSM fsm, int value = 1) {
        var totalLeft = fsm.Counter.Add(value); // Assuming Counter.Add exists and works similarly to Counter.Subtract

        // add icons
        if (!fsm.OneAssetOnly) {
            if (totalLeft > fsm.SpriteIcons.Count) {
                fsm.Counter.Value = fsm.SpriteIcons.Count;
                return;
            }

            for (var i = totalLeft - value; i < totalLeft; i++)
                fsm.SpriteIcons[i].rectTransform
                    .DOScale(1f, GameResourceFSM.EffectTime); // Assuming 1f is the original scale
        }

        // Change Text
        fsm.ChangeText(totalLeft);
    }
}


public class Preload : State<GameResourceFSM> {
    public override void Prepare(GameResourceFSM fsm) {
        fsm.ChangeText(fsm.Counter.Value);

        var iconPrefab = AssetLoader.AsComponent<OriginalSprite>(UI.ResourceIcon);

        var total = fsm.Counter.Value;
        // Create sprites
        if (fsm.OneAssetOnly) {
            var icon = Object.Instantiate(iconPrefab, fsm.transform);
            icon.image.sprite = AssetLoader.AsSprite(fsm.Type);
            icon.transform.localPosition = fsm.InitialPosition;
            icon.name = "icon0";
            fsm.SpriteIcons.Add(icon);
        }
        else {
            for (var i = 0; i < total; i++) {
                // bw icon
                var iconBw = Object.Instantiate(iconPrefab, fsm.transform);
                iconBw.image.sprite = AssetLoader.AsSprite(fsm.Type);
                iconBw.image.color = Color.black;
                iconBw.transform.localPosition = new Vector3(fsm.InitialPosition.x + i * 42f, fsm.InitialPosition.y,
                    fsm.InitialPosition.z);
                iconBw.name = $"icon{i}BW";

                // Then, icon
                var icon = Object.Instantiate(iconPrefab, fsm.transform);
                icon.image.sprite = AssetLoader.AsSprite(fsm.Type);
                icon.transform.localPosition = new Vector3(fsm.InitialPosition.x + i * 42f, fsm.InitialPosition.y,
                    fsm.InitialPosition.z);
                icon.name = $"icon{i}";
                fsm.SpriteIcons.Insert(i, icon);
            }
        }

        // text color
        fsm.PrintText();
        fsm.ChangeState(States.Created);
    }
}
}