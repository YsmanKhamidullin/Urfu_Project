using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private EnemyType _type;

    public  IEnemyMove Motor{ get; private set; }
    public IEnemyHealth Health { get; private set; }

    public EnemyFactory OriginFactory { get; set; }
}
public enum EnemyType
{
    DefaultEnemy,
    Boss
}
