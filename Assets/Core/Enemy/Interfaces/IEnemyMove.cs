using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyMove 
{
    void Move(Vector3 position);

    public void SetDurability(float durability);
}
