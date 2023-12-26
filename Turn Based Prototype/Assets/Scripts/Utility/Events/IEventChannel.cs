using Utility.Events;

namespace Utility.Events
{
    public interface IEventChannel
    {
        void Publish(IBusEvent busEvent);
        void RemoveAllNonPermanentCallbacks();
    }
}