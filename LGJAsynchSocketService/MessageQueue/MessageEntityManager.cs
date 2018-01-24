using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LGJAsynchSocketService.MessageQueue
{
    public class MessageEntityManager<T> : IMessageEntityManager<T>
    {
        private Queue<T> MessageEntityQueue = new Queue<T>();

        public void AddMessageEntity(T MessageEntity)
        {
            lock (this)
            {
                MessageEntityQueue.Enqueue(MessageEntity);
            }
        }

        public T GetMessageEntity()
        {
            T MessageEntity;
            lock (this)
            {
                MessageEntity = MessageEntityQueue.Dequeue();
            }
            return MessageEntity;
        }

        public bool IsMessageEntityAvailable
        {
            get
            {
                return (MessageEntityQueue.Count > 0) ? true : false;
            }
        }
    }
}
