﻿﻿using Deep.DungeonDefinition.Base;

 namespace Deep.DungeonDefinition
{
    public class PalaceOfTheDead : DeepDungeonDecorator
    {
        //public override string DisplayName => Name;
        
        public PalaceOfTheDead(Base.DeepDungeon deepDungeon) : base(deepDungeon)
        {
            
        }
        
        public override string GetDDType()
        {
            return "PotD";
        }
    }
}