using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LGJAsynchSocketService.MessageQueue
{
    public interface IMessageEntityManager<T>
    {
        void AddMessageEntity(T MessageEntity);
        T GetMessageEntity();
        bool IsMessageEntityAvailable
        {
            get;
        }
    }
}
