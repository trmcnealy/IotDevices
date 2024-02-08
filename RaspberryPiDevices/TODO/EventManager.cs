using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaspberryPiDevices;

public static class EventManager
{
    private static readonly object _oLockEvents = new object();

    private static readonly Dictionary<AutoResetEvent, List<AutoResetEvent>> _dEvents = new Dictionary<AutoResetEvent, List<AutoResetEvent>>();

    public static AutoResetEvent GenerateAutoResetEvent()
    {
        AutoResetEvent autoResetEvent = new AutoResetEvent(false);

        lock (_oLockEvents)
        {
            _dEvents.Add(autoResetEvent, new List<AutoResetEvent>());
        }

        return autoResetEvent;
    }
    
    public static AutoResetEvent GenerateChildEvent(AutoResetEvent autoResetEvent)
    {
        AutoResetEvent autoResetEventChild = new AutoResetEvent(false);

        lock (_oLockEvents)
        {
            _dEvents[autoResetEvent].Add(autoResetEventChild);
        }

        return autoResetEventChild;
    }

    public static void ReleaseAutoResetEvent(AutoResetEvent autoResetEvent)
    {
        lock (_oLockEvents)
        {
            _dEvents.Remove(autoResetEvent);
        }
    }


    public static void ReleaseChildEvent(AutoResetEvent autoResetEventChild)
    {
        lock (_oLockEvents)
        {
            foreach (List<AutoResetEvent> lAutoResetEvents in _dEvents.Values)
            {
                if (lAutoResetEvents.Contains(autoResetEventChild))
                {
                    lAutoResetEvents.Remove(autoResetEventChild);
                    break;
                }
            }
        }
    }

    public static void SetAll(AutoResetEvent autoResetEvent)
    {
        lock (_oLockEvents)
        {
            foreach (AutoResetEvent autoResetEventChild in _dEvents[autoResetEvent])
            {
                autoResetEventChild.Set();
            }
        }
    }
}