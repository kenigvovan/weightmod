using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace weightmod.src
{

    //
    //https://github.com/DArkHekRoMaNT
    //
    public class Config
    {
        public static Config Current { get; set; } = new Config();
        public class Part<Config>
        {
            public readonly string Comment;
            public readonly Config Default;
            private Config val;
            public Config Val
            {
                get => (val != null ? val : val = Default);
                set => val = (value != null ? value : Default);
            }
            public Part(Config Default, string Comment = null)
            {
                this.Default = Default;
                this.Val = Default;
                this.Comment = Comment;
            }
            public Part(Config Default, string prefix, string[] allowed, string postfix = null)
            {
                this.Default = Default;
                this.Val = Default;
                this.Comment = prefix;

                this.Comment += "[" + allowed[0];
                for (int i = 1; i < allowed.Length; i++)
                {
                    this.Comment += ", " + allowed[i];
                }
                this.Comment += "]" + postfix;
            }
        }

        public Part<float> MAX_PLAYER_WEIGHT = new Part<float>(20000);

        public Part<float> WEIGH_PLAYER_THRESHOLD = new Part<float>(0.7f, "On this border of current weight/max weight player will get slower movespeed");

        public Part<float> RATIO_MIN_MAX_WEIGHT_PLAYER_HEALTH = new Part<float>(0.6f, "If player doesn't have full health his max weight won't be lower" +
            "that (MAX_PLAYER_WEIGHT * (this number))");

        public Part<float> ACCUM_TIME_WEIGHT_CHECK = new Part<float>(2.0f, "How often server will calculate weight of players inventories.");
        
    }
}
