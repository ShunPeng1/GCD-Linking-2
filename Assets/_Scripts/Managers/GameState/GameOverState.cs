using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts.Managers;
using Shun_State_Machine;
using UnityEngine;

[Serializable]
public class GameOverState : BaseState<GameState>
{
    public GameOverState(GameState myStateEnum, Action<GameState, object[]> executeEvents = null, Action<GameState, object[]> exitEvents = null, Action<GameState, object[]> enterEvents = null) : base(myStateEnum, executeEvents, exitEvents, enterEvents)
    {
    }
}
