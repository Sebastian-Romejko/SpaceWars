using Assets.Scripts.Enums;
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
        public Dictionary<EnemyStrategy, decimal> enemyStrategyToPresence;

        public DifficultyLevelConfiguration(decimal percentOfEnemyPlanets, int secondsToProduceUnit, int unitsHP, Dictionary<EnemyStrategy, decimal> enemyStrategyToPresence)
        {
            this.percentOfEnemyPlanets = percentOfEnemyPlanets;
            this.secondsToProduceUnit = secondsToProduceUnit;
            this.unitsHP = unitsHP;
            this.enemyStrategyToPresence = enemyStrategyToPresence;
        }
    }
}
