using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    public class DifficultyLevelConfiguration
    {
        public decimal percentOfEnemyPlanets;
        public int secondsToProduceUnit;

        public DifficultyLevelConfiguration(decimal percentOfEnemyPlanets, int secondsToProduceUnit)
        {
            this.percentOfEnemyPlanets = percentOfEnemyPlanets;
            this.secondsToProduceUnit = secondsToProduceUnit;
        }
    }
}
