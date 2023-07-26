using System;
using System.Collections.Generic;
using _Scripts.DataWrapper;
using _Scripts.Lights;
using Shun_State_Machine;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Serialization;
using UnityUtilities;
using Random = UnityEngine.Random;

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
        
        [SerializeField] private CharacterInformation[] _charactersInformation;
        public CharacterSet ImposterSet;
        public readonly Dictionary<CharacterInformation, CharacterSet> CharacterSets = new();

        public PlayerRole CurrentRolePlaying;
        public CharacterRecognitionState ImposterLastRoundRecognition;
        
        

        protected void Start()
        {
            InitializeCharacters();
            CardManager.Instance.InitializeBag();

            CreateImposter();
            StartRound();
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
            
            
            MapManager.Instance.UpdateAllCharacterRecognition();
        }

        private void CreateImposter()
        {
            var imposterCharacterInformation = _charactersInformation[ Random.Range(0, _charactersInformation.Length) ];
            ImposterSet = CharacterSets[imposterCharacterInformation];
        }

        private void StartTurn()
        {
            SwapPlayingRole();
        }

        public void EndTurn()
        {
            StartTurn();
        }

        private void StartRound()
        {
            CardManager.Instance.ShuffleBackToDeck();
            CardManager.Instance.AddFromDeckToHand();
            
        }

        public void EndRound()
        {
            MapManager.Instance.UpdateAllCharacterRecognition();
            UpdateImposterRecognitionWithOther();
            
            UiManager.Instance.UpdateImposterRecognition();
            
            StartRound();
        }

        private void UpdateImposterRecognitionWithOther()
        {
            ImposterLastRoundRecognition = ImposterSet.CharacterRecognition.Value;
            foreach (var (characterInformation, characterSet) in CharacterSets)
            {
                if (characterSet == ImposterSet) continue;
                
                if (ImposterLastRoundRecognition != characterSet.CharacterRecognition.Value && characterSet.CharacterRecognition.Value != CharacterRecognitionState.Innocent)
                {
                    characterSet.CharacterRecognition.Value = CharacterRecognitionState.Innocent;
                }
            }
            
        }
        
        private void SwapPlayingRole()
        {
            CurrentRolePlaying = CurrentRolePlaying switch
            {
                PlayerRole.Detective => PlayerRole.Imposter,
                PlayerRole.Imposter => PlayerRole.Detective,
                _ => throw new ArgumentOutOfRangeException()
            };
            
            UiManager.Instance.UpdateRolePlaying();
        }

        public void RequestEndGame(BaseCharacterMapDynamicGameObject requestingCharacter)
        {
            switch (CurrentRolePlaying)
            {
                case PlayerRole.Detective:

                    var collideCharacter = requestingCharacter.GetCell().Item
                        .GetFirstInCellGameObject<BaseCharacterMapDynamicGameObject>();
                    if (collideCharacter == ImposterSet.CharacterMapGameObject)
                    {
                        Debug.Log("DETECTIVE WIN ");
                    }
                    else
                    {
                        Debug.Log("IMPOSTER WIN");
                    }
                    break;
                case PlayerRole.Imposter:
                    var exit = requestingCharacter.GetCell().Item
                        .GetFirstInCellGameObject<ExitMapGameObject>();
                    if (exit!= null && exit.IsOpen)
                    {
                        Debug.Log("IMPOSTER WIN");
                    }
                    
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public bool CheckImposterCanExit(BaseCharacterMapDynamicGameObject checkingCharacter)
        {
            return (checkingCharacter != ImposterSet.CharacterMapGameObject
                    && ImposterLastRoundRecognition == CharacterRecognitionState.InDark);
        }
        
    }
    
    
    
}