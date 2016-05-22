namespace MoBot.Structure.Game
{
    internal class LivingEntity : Entity
    {
        
        public float Yaw, Pitch;
        public float Health;

        public override string ToString()
        {
            return $"Utyped Living Entity ({(int)X} | {(int)Y} | {(int)Z})";
        }           

        public LivingEntity(int entityId) : base(entityId)
        {
            
        }
    }
}
