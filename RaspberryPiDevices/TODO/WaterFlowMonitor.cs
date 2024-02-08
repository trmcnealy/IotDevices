using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnitsNet;

namespace RaspberryPiDevices;

internal class Unsubscriber<T> : IDisposable
{
    private List<IObserver<T>> _observers;
    private IObserver<T> _observer;

    public Unsubscriber(List<IObserver<T>> observers, IObserver<T> observer)
    {
        _observers = observers;
        _observer = observer;
    }

    public void Dispose()
    {
        if (!(_observer == null))
        {
            _observers.Remove(_observer);
        }
    }
}

internal class WaterFlowMonitor : IObservable<VolumeFlow>
{

    private readonly List<IObserver<VolumeFlow>> _observers;

    public WaterFlowMonitor()
    {
        _observers = new List<IObserver<VolumeFlow>>();
    }


    public IDisposable Subscribe(IObserver<VolumeFlow> observer)
    {
        if (!_observers.Contains(observer))
        {
            _observers.Add(observer);
        }

        return new Unsubscriber<VolumeFlow>(_observers, observer);
    }
}
