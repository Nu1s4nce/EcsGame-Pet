using System.Collections;
using System.Collections.Generic;
using Leopotam.Ecs;
using UnityEngine;

public class DamageSystem : IEcsRunSystem
{
    private EcsFilter<Health, Damage> filter;
    
    public void Run()
    {
        foreach (var i in filter)
        {
            ref var health = ref filter.Get1(i);
            ref var damage = ref filter.Get2(i);
            
            health.healthCount -= damage.clickDamage;
            
        }
    }
}

public struct Damage
{
    public int clickDamage;
}
