using System;
using Shun_State_Machine;
using UnityEngine;

namespace _Scripts.Managers
{
    public enum GameState
    {
        Start,
        Over,
        Pause,
        InteractMap,
        ChooseCard,
        ChangeSide,

    }

    public enum WhoseSide
    {
        Detective,
        Imposter
    }

    public class GameManager : BaseStateMachine<GameState>
    {
        [SerializeField] private GameStartState _gameStartState = new (GameState.Start);
        [SerializeField] private GameOverState _gameOverState = new (GameState.Over);
        [SerializeField] private GamePauseState _gamePauseState = new (GameState.Pause);
        [SerializeField] private GameInteractMapState _gameInteractMapState = new (GameState.InteractMap);
        [SerializeField] private GameChooseCardState _gameChooseCardStateState = new (GameState.ChooseCard);
        [SerializeField] private GameChangeSideState _gameChangeSideStateState = new (GameState.ChangeSide);
        
        protected override void Awake()
        {
            InitializeState();
        }

        void InitializeState()
        {
            AddState(_gameStartState);
            AddState(_gameOverState);
            AddState(_gamePauseState);
            AddState(_gameInteractMapState);
            AddState(_gameChooseCardStateState);
            AddState(_gameChangeSideStateState);
            
            SetToState(GameState.Start);

        }
    }
}