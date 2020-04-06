namespace LevelTweaks.Configuration
{
    public class TweakData
    {
        public virtual HashDifMode LevelInfo { get; set; } 

        public virtual bool Selected { get; set; }
        public virtual string Name { get; set; }
        public virtual float NJS { get; set; }
        public virtual float Offset { get; set; }

    
        public class HashDifMode
        {
            public virtual string Hash { get; set; }
            public virtual string Difficulty { get; set; }
            public virtual string Mode { get; set; }

            public bool Equals(IDifficultyBeatmap bm, string mode) => Equals(bm.level.levelID, bm.difficulty, mode);
            public bool Equals(HashDifMode hdm) => hdm.Hash == Hash && hdm.Difficulty == Difficulty && hdm.Mode == Mode;
            public bool Equals(string hash, BeatmapDifficulty diff, string mode) => hash == Hash && diff.ToString() == Difficulty && mode == Mode;
        }
    }
}
