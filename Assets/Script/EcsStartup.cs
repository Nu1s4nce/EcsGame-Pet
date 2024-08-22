using Leopotam.Ecs;
using UnityEngine;

public class EcsStartup : MonoBehaviour
{
    [SerializeField] StaticData configuration;
    [SerializeField] SceneData sceneData;
    
    private EcsWorld _ecsWorld;
    private EcsSystems _systems;

    private void Start()
    {
        _ecsWorld = new EcsWorld(); // создаем новый EcsWorld
        _systems = new EcsSystems(_ecsWorld); // и группу систем в этом мире
        
        #if UNITY_EDITOR
            Leopotam.Ecs.UnityIntegration.EcsWorldObserver.Create (_ecsWorld);
        #endif
        
        RuntimeData runtimeData = new RuntimeData();

        _systems
            .Add(new HealthInitSystem()) // добавляем первую систему
            .Add(new DamageSystem())
            .Add(new StartServerOrClientSystem())
            
            .Add(new HealthViewSystem())
            
            .Inject(configuration)
            .Inject(sceneData)
            .Inject(runtimeData)
            
            .Init(); // обязательно инициализируем группу систем
        
        #if UNITY_EDITOR
            Leopotam.Ecs.UnityIntegration.EcsSystemsObserver.Create (_systems);
        #endif
    }

    private void Update()
    {
        _systems?.Run(); // запускаем системы каждый кадр
    }

    private void OnDestroy()
    {
        _systems?.Destroy(); // уничтожаем группу систем при уничтожении стартапа
        _systems = null;
        _ecsWorld?.Destroy(); // и мир
        _ecsWorld = null;
    }
}