using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SignalRFeatureOverview.Hub
{
    public interface IClientInterface
    {
        Task ClientHook(Data data);
    }
}
