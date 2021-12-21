using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//  유닛
//  Command_Move, Command_Attack 두개를 통해서 조종된다.

public class Unit : Character
{
    public virtual void Command_Move(Vector3 position) { }

    public virtual void Command_Move(Vector3Int cellPos) { }

    public virtual void Command_Attack(GameObject target) { }

    public virtual void Command_Attack(Character target) { }
}
