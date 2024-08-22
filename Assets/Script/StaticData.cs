using System.Net;
using UnityEngine;

[CreateAssetMenu]
public class StaticData : ScriptableObject
{
    public int defaultHealth;
    public int clickDamage;

    public int PORT;
    public string ServerEndPoint;
}
