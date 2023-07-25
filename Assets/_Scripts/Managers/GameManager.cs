using System;
using System.Collections.Generic;
using _Scripts.DataWrapper;
using _Scripts.Lights;
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

        
        [SerializeField] private CharacterInformation[] _charactersInformation;
        public readonly Dictionary<CharacterInformation, CharacterSet> CharacterSets = new();

        public PlayerRole CurrentRolePlaying;
        protected void Awake()
        {
            InitializeState();
        }

        protected void Start()
        {
            InitializeCharacters();
            CardManager.Instance.Initialize();

            StartRound();
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
        
        private void InitializeCharacters()
        {
            
            foreach (var characterInformation in _charactersInformation)
            {
                var characterMap = MapManager.Instance.CreateCharacterMapGameObject(
                    characterInformation.CharacterMapDynamicGameObjectPrefab);
                var characterCard = CardManager.Instance.CreateCharacterMapGameObject(
                    characterInformation.BaseCharacterCardGameObjectPrefab);
                var characterPortrait = UiManager.Instance.CreatePortraitButton(
                    characterInformation.PortraitButtonRectPrefab);
                var characterRecognition =
                    new ObservableData<CharacterRecognitionState>(CharacterRecognitionState.InDark);
                
                CharacterSet set = new CharacterSet(characterInformation, characterMap, characterCard, characterPortrait, characterRecognition);
                CharacterSets[characterInformation] = set;

                characterMap.InitializeCharacter(set);
                characterCard.InitializeCharacter(set);
                characterPortrait.InitializeCharacter(set);
            }
        }

        public void StartRound()
        {
            
            CardManager.Instance.ShuffleBackToDeck();
            CardManager.Instance.AddFromDeckToHand();
        }

        public void EndRound()
        {
            MapManager.Instance.UpdateAllCharacterRecognition();
            
            StartRound();
        }

        private void SwapPlayingRole()
        {
            CurrentRolePlaying = CurrentRolePlaying switch
            {
                PlayerRole.Detective => PlayerRole.Imposter,
                PlayerRole.Imposter => PlayerRole.Detective,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
        
    }
    
    
    
}