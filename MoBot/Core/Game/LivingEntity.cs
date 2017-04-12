namespace MoBot.Core.Game
{
    public class LivingEntity : Entity
    {
        
        public float Yaw, Pitch;
        private float health = 20;

        public float Health
        {
            get => health;

            set
            {
                health = value;
                OnPropertyChanged(nameof(Health));
            }
        }

        public override string ToString()
        {
            return $"Utyped Living Entity ({(int)X} | {(int)Y} | {(int)Z})";
        }           

        public LivingEntity(int entityId) : base(entityId)
        {
            
        }
    }
}
