using System;
using System.Collections.Generic;
using System.Linq;
using Core.Services;
using Core.Utils.Constants;
using Framework.Base;
using UnityEngine;

namespace Core.Data {

[Serializable]
public class PlayerDataV1 : Data<PlayerDataV1> {
    public List<List<Card>> SavedAbilities { get; private set; } = new() {
        new List<Card>(Enumerable.Repeat(Card.NONE, 4).ToArray()),
        new List<Card>(Enumerable.Repeat(Card.NONE, 4).ToArray()),
        new List<Card>(Enumerable.Repeat(Card.NONE, 4).ToArray())
    };

    public List<int> CurrentLife { get; private set; } = new() {
        3, 2, 1
    };

    public List<DateTime> LastMatchTime { get; private set; } = new() {
        new DateTime(), new DateTime(), new DateTime()
    };

    public Card[] GetSavedRocks() {
        var cards = new List<Card>();
        cards.AddRange(saveRockSlot.Where(card => card != Card.NONE));
        return cards.ToArray();
    }

    public void SetLastPosition(Vector3 vector) {
        lastMapPosition[0] = vector.x;
        lastMapPosition[1] = vector.y;
        Save();
    }

    public Vector3 GetLastPosition() {
        return new Vector3(lastMapPosition[0], lastMapPosition[1], -10);
    }

    public void SetLastMatchTime(Card player, DateTime time) {
        LastMatchTime[GetIndexByCharacter(player)] = time;
        Save();
    }

    public DateTime GetLastMatchTime(Card player) {
        return LastMatchTime[GetIndexByCharacter(player)];
    }


    public void AddLife(Card character) {
        CurrentLife[GetIndexByCharacter(character)] = CurrentLife[GetIndexByCharacter(character)] + 1;
        Save();
    }

    public int GetLife(Card character) {
        return CurrentLife[GetIndexByCharacter(character)];
    }

    public void RemoveLife(Card character) {
        if (CurrentLife[GetIndexByCharacter(character)] <= 0)
            return;

        CurrentLife[GetIndexByCharacter(character)] = CurrentLife[GetIndexByCharacter(character)] - 1;

        // Only Reset Live if character just lost its first life (two instead of three)
        if (CurrentLife[GetIndexByCharacter(character)] == 2)
            SetLastMatchTime(selectedCharacter, TimeServices.Now());
        Save();
    }

    public void RemoveLifeFromCurrentPlayer() {
        RemoveLife(selectedCharacter);
    }

    public Vector3 GetCurrentLocation() {
        return new Vector3(currentLocation[0], currentLocation[1], 0);
    }

    public int GetIndexByCharacter(Card card) {
        return card switch {
            Card.Card_005_Char_Lucas => 0,
            Card.Card_006_Char_Lisa => 1,
            Card.Card_007_Char_Bill => 2,
            _ => 0
        };
    }

    public bool HasMasterKey() {
        return CardsDataV1.Instance.HasCard(Card.Card_008_Special_Char_Willy);
    }

    public void ChangeLocation(Vector3 position) {
        ChangeLocation(position.x, position.y);
    }

    public void ChangeLocation(float x, float y) {
        currentLocation[0] = x;
        currentLocation[1] = y;
        Save();
    }

    public bool CurrentPlayerHasLife() {
        return CurrentLife[GetIndexByCharacter(selectedCharacter)] > 0;
    }

    public int CurrentPlayerLife() {
        return CurrentLife[GetIndexByCharacter(selectedCharacter)];
    }

    public bool HasChosenAbility(Card characterCard, int tier, Card card) {
        return SavedAbilities[GetIndexByCharacter(characterCard)][tier] == card;
    }

    public List<Card> GetAbilities(Card card) {
        return SavedAbilities[GetIndexByCharacter(card)] ?? new List<Card>();
    }

    public void ChooseAbility(int tierIndex, Card newCard, Card character) {
        SavedAbilities[GetIndexByCharacter(character)][tierIndex] = newCard;
        Save();
    }

    public void ClearSaveRockSlots() {
        saveRockSlot = Enumerable.Repeat(Card.NONE, 30).ToArray();
        saveRockSlot[0] = Card.Card_001_Crooked_Rock;
        Save();
    }

    public void SetSelectedCharacter(Card card) {
        selectedCharacter = card;
        Save();
    }

    public int RockCardCounter(Card card) {
        return saveRockSlot.Count(x => x == card);
    }

    public void ChangeRockSlot(int index, Card card) {
        saveRockSlot[index] = card;
        Save();
    }

    #region Properties

    [SerializeField] private float[] lastMapPosition = { 0f, 0f };
    [SerializeField] public float[] currentLocation = { -11.5f, -20f };

    [SerializeField] public Card[] saveRockSlot =
        Enumerable.Repeat(Card.NONE, 29).Prepend(Card.Card_001_Crooked_Rock).ToArray();

    [SerializeField] public Card selectedCharacter = Card.Card_005_Char_Lucas;

    #endregion
}

}