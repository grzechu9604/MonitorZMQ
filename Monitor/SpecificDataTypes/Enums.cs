namespace Monitor.SpecificDataTypes
{
    public enum MessageTypes
    {
        IDRequest = 1,
        IDSet = 2,
        Reset = 3,
        MonitorAcquire = 4,
        MonitorRelease = 5,
        Wait = 6,
        Signal = 7,
        SignalAll = 8,
        Acknowledgement = 100,
        Negation = 101
    }
}
