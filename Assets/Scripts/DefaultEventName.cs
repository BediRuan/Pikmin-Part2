using System;
using UnityEngine;

public static class DefaultEventName
{
    // Start
    public const string GameStartEvent = nameof(GameStartEvent);

    // End
    public const string GameOverEvent = nameof(GameOverEvent);


    // to target
    public const string MovePosEvent = nameof(MovePosEvent);
}

public class GameOverEventArgs : EventArgs
{
    public bool playerWin;
}

public class MovePosEventArgs : EventArgs
{
    public Vector3 movePos;
}