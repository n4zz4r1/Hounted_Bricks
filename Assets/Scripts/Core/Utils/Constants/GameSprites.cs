using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Utils.Constants {

public static class GameSprites {
    public static Dictionary<ResourceType, Sprite> resourceTypeSprite = new();

    // Static constructor to load sprites
    static GameSprites() {
        // Preload all sprites
        Debug.Log("Preloading all sprites");
        foreach (var resourceType in Enum.GetValues(typeof(ResourceType)))
            resourceTypeSprite.Add((ResourceType)resourceType, LoadSprite("Resource_" + resourceType));
    }

    private static Sprite LoadSprite(string name) {
        return Resources.Load<Sprite>("" + name);
    }
}

}