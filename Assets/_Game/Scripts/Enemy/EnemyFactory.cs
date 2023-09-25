using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class EnemyFactory : FactoryBase
{
    [System.Serializable]
    public class EnemyConfig
    {
        [SerializeField]
        public EnemyType Type;

        [SerializeField]
        public Enemy EnemyPrefab;

        [FloatRangeSlider(0.5f, 2f)]
        public FloatRange Durability = new FloatRange(1f);
        [FloatRangeSlider(10f, 15f)]
        public FloatRange Health = new FloatRange(10f);
    }

    [SerializeField]
    private List<EnemyConfig> _enemyConfigs = new List<EnemyConfig>();

    public Enemy Get(EnemyType type)
    {
        EnemyConfig config = this._enemyConfigs.Find(enemy => enemy.Type == type);
        Enemy instance = new EnemyBuilder(CreateInstance(config.EnemyPrefab)).SetFactory(this);

        return instance;
    }

    public void Reclaim(Enemy enemy)
    {
        Destroy(enemy);
    }
}
