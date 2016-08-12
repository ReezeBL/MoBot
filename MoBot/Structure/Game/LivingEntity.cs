namespace MoBot.Structure.Game
{
    public class LivingEntity : Entity
    {
        
        public float Yaw, Pitch;
        public float Health = 20;

        public override string ToString()
        {
            return $"Utyped Living Entity ({(int)X} | {(int)Y} | {(int)Z})";
        }           

        public LivingEntity(int entityId) : base(entityId)
        {
            
        }
    }
}
