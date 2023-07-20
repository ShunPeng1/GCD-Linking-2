using System;
using Shun_State_Machine;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityUtilities;

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

    public class GameManager : SingletonMonoBehaviour<GameManager>
    {
        public readonly BaseStateMachine<GameState> BaseStateMachine = new BaseStateMachine<GameState>();
        
        public GameStartState GameStartState = new (GameState.Start);
        public GameOverState GameOverState = new (GameState.Over);
        public GamePauseState GamePauseState = new (GameState.Pause);
        public GameInteractMapState GameInteractMapState = new (GameState.InteractMap);
        public GameChooseCardState GameChooseCardStateState = new (GameState.ChooseCard);
        public GameChangeSideState GameChangeSideStateState = new (GameState.ChangeSide);
        
        protected void Awake()
        {
            InitializeState();
        }

        void InitializeState()
        {
            BaseStateMachine.AddState(GameStartState);
            BaseStateMachine.AddState(GameOverState);
            BaseStateMachine.AddState(GamePauseState);
            BaseStateMachine.AddState(GameInteractMapState);
            BaseStateMachine.AddState(GameChooseCardStateState);
            BaseStateMachine.AddState(GameChangeSideStateState);
            
            BaseStateMachine.SetToState(GameState.Start);

        }

        private void Update()
        {
            BaseStateMachine.CurrentBaseState.ExecuteState();
        }
    }
}