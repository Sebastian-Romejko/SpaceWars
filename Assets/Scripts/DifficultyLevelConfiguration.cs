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
        public int unitsHP;

        public DifficultyLevelConfiguration(decimal percentOfEnemyPlanets, int secondsToProduceUnit, int unitsHP)
        {
            this.percentOfEnemyPlanets = percentOfEnemyPlanets;
            this.secondsToProduceUnit = secondsToProduceUnit;
            this.unitsHP = unitsHP;
        }
    }
}
