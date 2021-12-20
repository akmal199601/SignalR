using System.Threading.Tasks;

namespace SignalR.Interface
{
    public interface IHubClient
    {
        Task BroadcastMessage();
    }
}