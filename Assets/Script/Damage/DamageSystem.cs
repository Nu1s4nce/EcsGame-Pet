using Leopotam.Ecs;

public class DamageSystem : IEcsInitSystem
{
    private EcsFilter<Health> filter;
    
    private StaticData _staticData;
    private SceneData _sceneData;
    private RuntimeData _runtimeData;
    
    public void Init()
    {
        _sceneData.mainBtn.onClick.AddListener((() =>
        {
            foreach (var i in filter)
            {
                ref var health = ref filter.Get1(i);
            
                health.healthCount -= _staticData.clickDamage;
                if (_runtimeData.IsServer)
                {
                    _runtimeData._server.SendMessageToAllSockets(new DataStruct
                    {
                        header = "simple",
                        message = "clickFromServer"
                    });
                }
                else
                {
                    _runtimeData._client.SendMessageToSocket(new DataStruct
                    {
                        header = "simple",
                        message = "click"
                    });
                }
            }
        }));
        
    }
}