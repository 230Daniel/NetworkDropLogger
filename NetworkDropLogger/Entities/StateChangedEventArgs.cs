using System;

namespace NetworkDropLogger.Entities
{
    public class StateChangedEventArgs : EventArgs
    {
        public ConnectionState Old;
        public ConnectionState New;
    }
}
