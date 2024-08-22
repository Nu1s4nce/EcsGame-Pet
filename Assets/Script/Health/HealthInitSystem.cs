using Leopotam.Ecs;
using UnityEngine;

public class HealthInitSystem : IEcsInitSystem
{
    private EcsWorld _ecsWorld;
    private StaticData _staticData;
    private SceneData _sceneData;
    
    public void Init()
    {
        EcsEntity healthEntity = _ecsWorld.NewEntity();

        ref var health = ref healthEntity.Get<Health>();

        health.healthCount = _staticData.defaultHealth;
        
        _sceneData.healthSlider.value = health.healthCount;
        _sceneData.healthText.text = health.healthCount.ToString();
        
        Debug.Log(health.healthCount);
    }
}
