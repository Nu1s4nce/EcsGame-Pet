using System.Net;
using Leopotam.Ecs;
using UnityEngine;

public class StartServerOrClientSystem : IEcsInitSystem
{
    private RuntimeData _runtimeData;
    private SceneData _sceneData;
    private StaticData _staticData;
    
    public void Init()
    {
        _sceneData.hostBtn.onClick.AddListener(() =>
        {
            _runtimeData._server = new Server();
            _runtimeData._server.StartServer(_staticData.PORT);
            _runtimeData.IsServer = true;
        });
        _sceneData.joinBtn.onClick.AddListener(() =>
        {
            _runtimeData._client = new Client();
            EndPoint endPoint = new IPEndPoint(IPAddress.Parse(_staticData.ServerEndPoint), _staticData.PORT);
            _runtimeData._client.ConnectTo(endPoint);
            _runtimeData.IsServer = false;
        });
    }
}
