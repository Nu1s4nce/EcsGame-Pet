using Leopotam.Ecs;

public class HealthViewSystem : IEcsRunSystem
{
    private EcsFilter<Health> filter;
    private SceneData _sceneData;

    public void Run()
    {
        foreach (var i in filter)
        {
            ref var health = ref filter.Get1(i);

            _sceneData.healthSlider.value = health.healthCount;
            _sceneData.healthText.text = health.healthCount.ToString();
        }
    }
}
