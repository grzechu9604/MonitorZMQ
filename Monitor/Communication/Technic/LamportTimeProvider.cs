using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Monitor.Communication.Technic
{
    class LamportTimeProvider
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Increment() => _lamportTime++;

        [MethodImpl(MethodImplOptions.Synchronized)]
        public ulong IncrementAndReturn() => _lamportTime++;

        [MethodImpl(MethodImplOptions.Synchronized)]
        public ulong IncrementAndReturnWithMin(ulong minValue)
        {
            _lamportTime = Math.Max(minValue, _lamportTime);
            return _lamportTime++;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void IncrementWithMin(ulong minValue)
        {
            _lamportTime = Math.Max(minValue, _lamportTime);
            _lamportTime++;
        }

        private ulong _lamportTime;

        #region Singleton
        public static LamportTimeProvider Instance { get; } = new LamportTimeProvider();
        private LamportTimeProvider()
        {
            _lamportTime = 0;
        }
        #endregion
    }
}
