﻿namespace Game.Utils {
public abstract class SingletonBase<T> where T : SingletonBase<T>, new() {
    public static T Instance { get; } = new();
}
}