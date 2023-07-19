using Shun_State_Machine;

namespace _Scripts.Managers
{
    public enum GameState
    {
        Start,
        End,
        Pause,
        Card
    }

    public enum WhoseSide
    {
        Detective,
        Imposter
    }

    public class GameManager : BaseStateMachine<GameState>
    {
        protected override void Awake()
        {
            base.Awake();
            
            
        }
        
    }
}