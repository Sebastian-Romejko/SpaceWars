using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Enums
{
    public enum Fraction
    {
        [Description("Player")]
        PLAYER,
        [Description("Enemy")]
        ENEMY,
        [Description("Neutral")]
        NEUTRAL
    }
}
