using RababaTest.Characters;

namespace RababaTest.EventBus
{
    public interface IEvent {}

    public struct PlayerSpawnEvent : IEvent
    {
        public Player Player;
        public PlayerConfiguration PlayerConfiguration;
    }

    public struct BossSpawnEvent : IEvent
    {
        public Enemy Enemy;
    }
    
    public struct PlayerDiesEvent : IEvent
    {
        public Player Player;
    }

    public struct GameReadyEvent : IEvent
    {
        
    }

    public struct AllPlayersJoinedEvent : IEvent
    {
        
    }

    public struct PlayerTakeDamageEvent : IEvent
    {
        public int PlayerIndex;
        public float CurrentHealth;
    }
    
    public struct BossTakeDamageEvent : IEvent
    {
        public float CurrentHealth;
    }

    public struct BossDefeatedEvent : IEvent
    {
    }

    public struct GameOverEvent : IEvent
    {
    }
}