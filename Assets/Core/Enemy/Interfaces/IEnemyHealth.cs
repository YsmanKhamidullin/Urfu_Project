using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyHealth 
{
    void AddHealth(int healthCount);
    void TakeDamage(int damage);
}
