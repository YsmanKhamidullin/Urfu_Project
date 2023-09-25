public class EnemyBuilder
{
    private Enemy _enemy;

    public EnemyBuilder SetDurability(float durability)
    {
        this._enemy.Motor.SetDurability(durability);
        return this;
    }

    public EnemyBuilder SetHealth(int health) 
    {
        this._enemy.Health.AddHealth(health);
        return this;
    }

    public EnemyBuilder SetFactory(EnemyFactory factory)
    {
        this._enemy.OriginFactory = factory;
        return this;    
    }

    public EnemyBuilder(Enemy enemy)
    {
        this._enemy = enemy;
    }

    public static implicit operator Enemy(EnemyBuilder builder)
    {
        return builder._enemy;
    }
}
