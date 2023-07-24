using System;
using Shun_State_Machine;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityUtilities;

namespace _Scripts.Managers
{
    public enum GameState
    {
        StartRound,
        Over,
        Pause,
        InteractMap,
        ChooseCard,
        ChangeSide,

    }

    public enum PlayerRole
    {
        Detective,
        Imposter
    }

    public class GameManager : SingletonMonoBehaviour<GameManager>
    {
        public readonly BaseStateMachine<GameState> BaseStateMachine = new BaseStateMachine<GameState>();
        
        public GameStartRoundState GameStartRoundState = new (GameState.StartRound);
        public GameOverState GameOverState = new (GameState.Over);
        public GamePauseState GamePauseState = new (GameState.Pause);
        public GameInteractMapState GameInteractMapState = new (GameState.InteractMap);
        public GameChooseCardState GameChooseCardStateState = new (GameState.ChooseCard);
        public GameChangeSideState GameChangeSideStateState = new (GameState.ChangeSide);

        
        public CharacterInformation[] CharactersInformation;
        
        public PlayerRole CurrentRolePlaying;
        protected void Awake()
        {
            InitializeState();
        }

        void InitializeState()
        {
            BaseStateMachine.AddState(GameStartRoundState);
            BaseStateMachine.AddState(GameOverState);
            BaseStateMachine.AddState(GamePauseState);
            BaseStateMachine.AddState(GameInteractMapState);
            BaseStateMachine.AddState(GameChooseCardStateState);
            BaseStateMachine.AddState(GameChangeSideStateState);
            
            BaseStateMachine.SetToState(GameState.StartRound);

        }

        private void Update()
        {
            BaseStateMachine.CurrentBaseState.ExecuteState();
        }
        
        
    }
    
    
    
}