using OpenBveApi.Colors;
using OpenBveApi.Runtime;

namespace Plugin.Managers
{
    internal static class MessageManager
    {
        private static AddInterfaceMessageDelegate AddMessage;
        internal static void Initialize(AddInterfaceMessageDelegate addMessage)
        {
            AddMessage = addMessage;
        }
        internal static void PrintMessage(string Message, MessageColor Color, double Time)
        {
            AddMessage.Invoke(Message, Color, Time);
        }
    }
}
