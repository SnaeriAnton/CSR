using System;

namespace Game
{
    public static class NetworkMessageTypeId
    {
        public static string Get<TMessage>() => typeof(TMessage).FullName;
    }
}
