using Core.StateMachine.Cards;
using UnityEngine;

namespace Core.Utils.Constants {

public static class Colors {
    public static readonly Color PRIMARY = new(0.69f, 0.60f, 0.47f);
    public static readonly Color SECONDARY = new(0.55f, 0.48f, 0.38f);
    public static readonly Color COMMON_CARD = new(0.77f, 0.73f, 0.69f);
    public static readonly Color COMMON_CARD_DARK = new(0.61f, 0.58f, 0.54f);
    public static readonly Color UNCOMMON_CARD = new(0.64f, 0.64f, 0.46f);
    public static readonly Color UNCOMMON_CARD_DARK = new(0.51f, 0.51f, 0.37f);
    public static readonly Color RARE_CARD = new(0.46f, 0.57f, 0.70f);
    public static readonly Color RARE_CARD_DARK = new(0.37f, 0.46f, 0.56f);
    public static readonly Color LEGENDARY_CARD = new(0.50f, 0.00f, 0.50f);
    public static readonly Color LEGENDARY_CARD_DARK = new(0.40f, 0.00f, 0.40f);
    public static readonly Color GOLD_CARD = new(0.85f, 0.65f, 0.13f);
    public static readonly Color GOLD_CARD_DARK = new(0.68f, 0.52f, 0.10f);
    public static readonly Color LIGHT_FONT = new(0.98f, 0.96f, 0.95f);
    public static readonly Color DARK_FONT = new(0.23f, 0.20f, 0.16f);
    public static readonly Color DARK_WOOD = new(0.23f, 0.20f, 0.16f);
    public static readonly Color ACCENT_FONT = new(0.31f, 0.50f, 0.60f);
    public static readonly Color MENU_BACKGROUND = new(0.87f, 0.83f, 0.77f);
    public static readonly Color ACTION_BUTTON = new(0.62f, 0.52f, 0.45f);
    public static readonly Color EXIT_BUTTON = new(0.78f, 0.29f, 0.19f);
    public static readonly Color POPUP_BODY = new(0.96f, 0.94f, 0.90f);
    public static readonly Color POPUP_TITLE = new(0.52f, 0.36f, 0.21f);
    public static readonly Color ACTIVE_ACTION_BUTTON = new(0.62f, 0.52f, 0.45f);
    public static readonly Color INACTIVE_ACTION_BUTTON = new(0.69f, 0.64f, 0.58f);
    public static readonly Color ACTIVE_EXIT_BUTTON = new(0.78f, 0.29f, 0.19f);
    public static readonly Color INACTIVE_EXIT_BUTTON = new(0.84f, 0.62f, 0.57f);
    public static readonly Color DISABLED = new(0.67f, 0.67f, 0.67f, 1f);
    public static readonly Color DISABLED_ALPHA = new(0.67f, 0.67f, 0.67f, 0.2f);
    public static readonly Color ENABLED = COMMON_CARD;
    public static readonly Color ENABLED_2 = PRIMARY;
    public static readonly Color WHITE = new(0.98f, 0.96f, 0.95f);
    public static readonly Color BLACK = new(0f, 0f, 0f);
    public static readonly Color ORANGE_CHEST = new(0.83f, 0.45f, 0.09f);
    public static readonly Color DIAMOND = new(0.67f, 0.33f, 0.33f);

    public static readonly Color EASY = new(0.56f, 0.76f, 0.39f);
    public static readonly Color MEDIUM = new(0.85f, 0.72f, 0.19f);
    public static readonly Color HARD = new(0.90f, 0.45f, 0.17f);
    public static readonly Color VERY_HARD = new(0.75f, 0.22f, 0.17f);
    public static readonly Color BUTTON_VARIANT_1 = new(0.45f, 0.56f, 0.67f);
    public static readonly Color BUTTON_VARIANT_2 = new(0.70f, 0.58f, 0.50f);
    public static readonly Color BUTTON_VARIANT_3 = new(0.67f, 0.33f, 0.33f);
    public static readonly Color ACTIVE_MENU_BUTTON = new(0.62f, 0.52f, 0.45f);
    public static readonly Color INACTIVE_MENU_BUTTON = new(0.62f, 0.52f, 0.45f, 0.5f);
    public static readonly Color ROCK_ON_POISON = new(0.62f, 0.52f, 0.45f, 0.5f); // TODO change
    public static readonly Color FIRE = new(0.62f, 0.52f, 0.45f, 0.5f); // TODO change

    public static Color WithAlpha(Color color, float alpha) {
        var newColorWithAlpha = color;
        newColorWithAlpha.a = alpha;
        return newColorWithAlpha;
    }
}


public class DifficultyUtil {
    public Color color { get; private set; }
    public StageDifficultyType difficultyType { get; private set; }
    public string label { get; }

    public static DifficultyUtil From(StageDifficultyType type) {
        DifficultyUtil difficultyUtil = new();
        difficultyUtil.difficultyType = type;
        switch (type) {
            case StageDifficultyType.EASY:
                difficultyUtil.color = Colors.EASY;
                // difficultyUtil.label = LocalizationUtils.GetLocalizedText("Label.StageDifficultyType.EASY");
                break;
            case StageDifficultyType.MEDIUM:
                difficultyUtil.color = Colors.MEDIUM;
                // difficultyUtil.label = LocalizationUtils.GetLocalizedText("Label.StageDifficultyType.MEDIUM");
                break;
            case StageDifficultyType.HARD:
                difficultyUtil.color = Colors.HARD;
                // difficultyUtil.label = LocalizationUtils.GetLocalizedText("Label.StageDifficultyType.HARD");
                break;
            case StageDifficultyType.IMPOSSIBLE:
                difficultyUtil.color = Colors.HARD;
                // difficultyUtil.label = LocalizationUtils.GetLocalizedText("Label.StageDifficultyType.HARD");
                break;
            default:
                difficultyUtil.color = Colors.EASY;
                // difficultyUtil.label = LocalizationUtils.GetLocalizedText("Label.StageDifficultyType.EASY");
                break;
        }

        return difficultyUtil;
    }
}

public class ResourceUtils {
    public Color BackgroundColor { get; private set; }

    public static ResourceUtils From(ResourceType resourceType) {
        return new ResourceUtils {
            BackgroundColor = resourceType switch {
                ResourceType.COIN => Colors.GOLD_CARD,
                ResourceType.DIAMOND => Colors.DIAMOND,
                ResourceType.CHEST => Colors.ORANGE_CHEST,
                _ => Colors.WHITE
            }
        };
    }
}


public class RarityUtils {
    public Color NormalColor { get; private set; }
    public Color LightColor { get; private set; }

    public string Label { get; }


    public static RarityUtils From(CardRarity cardRarity) {
        RarityUtils rarityUtils = new();

        switch (cardRarity) {
            case CardRarity.COMMON:
                rarityUtils.NormalColor = new Color(0.77f, 0.67f, 0.52f);
                rarityUtils.LightColor = rarityUtils.NormalColor;
                // rarityUtils.Label = LocalizationUtils.GetLocalizedText("Quality.Common");
                break;
            case CardRarity.UNCOMMON:
                rarityUtils.NormalColor = new Color(0.33f, 0.77f, 0.54f);
                rarityUtils.LightColor = rarityUtils.NormalColor;
                // rarityUtils.Label = LocalizationUtils.GetLocalizedText("Quality.Uncommon");
                break;
            case CardRarity.RARE:
                rarityUtils.NormalColor = new Color(0.28f, 0.54f, 0.85f);
                rarityUtils.LightColor = rarityUtils.NormalColor;
                // rarityUtils.Label = LocalizationUtils.GetLocalizedText("Quality.Rare");
                break;
            case CardRarity.LEGENDARY:
                rarityUtils.NormalColor = new Color(0.60f, 0.28f, 1f);
                rarityUtils.LightColor = rarityUtils.NormalColor;
                // rarityUtils.Label = LocalizationUtils.GetLocalizedText("Quality.Legendary");
                break;
            case CardRarity.GOLD:
                rarityUtils.NormalColor = new Color(0.83f, 0.76f, 0.45f);
                rarityUtils.LightColor = rarityUtils.NormalColor;
                // rarityUtils.Label = LocalizationUtils.GetLocalizedText("Quality.Gold");
                break;
            default:
                rarityUtils.NormalColor = Colors.PRIMARY;
                break;
        }

        return rarityUtils;
    }
}

}