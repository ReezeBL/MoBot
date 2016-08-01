﻿namespace MoBot.Structure.Game
{
    public class Mob : LivingEntity
    {
        public byte Type;
        public override string ToString()
        {
            return $"Mob : {Type} ({(int)X} | {(int)Y} | {(int)Z})";
        }      

        public Mob(int id): base(id)
        {
        }
    }
}