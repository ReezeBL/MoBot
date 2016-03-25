using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoBot.Structure
{
    class Model : IObservable<SysAction>
    {
        public IDisposable Subscribe(IObserver<SysAction> observer)
        {
            throw new NotImplementedException();
        }
    }
}
