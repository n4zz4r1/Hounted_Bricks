using Core.Handler;
using Core.Sprites;
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
        if (!fsm.oneAssetOnly) {
            
            if (totalLeft + value <= 0 || fsm.AllEmpty) {
                fsm.AllEmpty = true;
                return true;
            }
        
            if (totalLeft < 0)
                totalLeft = 0;
            
            for (var i = totalLeft; i < totalLeft + value; i++) {
                fsm.SpriteIcons[i].rectTransform.DOScale(0f, GameResourceFSM.EffectTime);
            }
        }

        // Change Text
        fsm.ChangeText(totalLeft);
        return totalLeft == 0;
    }

    public override void Increase(GameResourceFSM fsm, int value = 1) {
        var totalLeft = fsm.Counter.Add(value); // Assuming Counter.Add exists and works similarly to Counter.Subtract


        // add icons
        if (!fsm.oneAssetOnly) {
            if (totalLeft > fsm.SpriteIcons.Count) {
                fsm.Counter.Value = fsm.SpriteIcons.Count;
                return;
            }
            
            for (var i = totalLeft - value; i < totalLeft; i++) {
                fsm.SpriteIcons[i].rectTransform.DOScale(1f, GameResourceFSM.EffectTime); // Assuming 1f is the original scale
            }
        }

        // Change Text
        fsm.ChangeText(totalLeft);
    }
}


public class Preload : State<GameResourceFSM> {
    public override void Prepare(GameResourceFSM fsm) {

        fsm.Counter.Value = fsm.type switch {
            // Get initial quantity if Drop or Hearts
            GameResource.Drop => fsm.GameController.PlayerCardInGame.Attribute(CardAttributeType.Consumable),
            _ => fsm.GameController.PlayerCardInGame.Attribute(CardAttributeType.Health)
        };
        fsm.ChangeText(fsm.Counter.Value);

        var iconPrefab = AssetLoader.AsComponent<OriginalSprite>((GameResourceType)fsm.type);
        var total = fsm.Counter.Value;
        // Create sprites
        if (fsm.oneAssetOnly) {
            var icon = Object.Instantiate(iconPrefab, fsm.transform);
            icon.transform.localPosition = fsm.InitialPosition;
            icon.name = "icon0";
            fsm.SpriteIcons.Add(icon);
        } else {
            for (var i = 0; i < total; i++) {
                var icon = Object.Instantiate(iconPrefab, fsm.transform);
                icon.transform.localPosition = new Vector3(fsm.InitialPosition.x + i * 42f, fsm.InitialPosition.y, fsm.InitialPosition.z);
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