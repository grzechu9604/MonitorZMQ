using ZeroMQ;

namespace Monitor.Communication.Technic
{
    class ZContextProvider
    {

        public static ZContext GlobalContext { get; } = new ZContext();

        private ZContextProvider()
        {

        }
    }
}
