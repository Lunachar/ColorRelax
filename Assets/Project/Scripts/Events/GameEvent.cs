using UnityEngine.Events;

[System.Serializable]
public class GameEvent : UnityEvent { }

[System.Serializable]
public class IntGameEvent : UnityEvent<int> { }