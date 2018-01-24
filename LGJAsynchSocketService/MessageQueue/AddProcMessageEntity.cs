using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace LGJAsynchSocketService.MessageQueue
{
    public class AddProcMessageEntity
    {

        private MessageEntityManager<LGJMessageEntity> mem;
        public delegate void AsynchNotifyHandler(string MessageStr);
        public AsynchNotifyHandler ResultAsynchNotifyHandler;
        //QueneMainForm MyQueneMainForm;

        public static void Start(MessageEntityManager<LGJMessageEntity> MeMessageEntityManager)
        {

            //			ProcessDocuments<T, U> pd;
            //			pd = new ProcessDocuments<T, U>(dm);
            //			Thread t1 = new Thread(new ThreadStart(pd.Run));
            //			t1.Start();

            // new Thread(new ThreadStart(new ProcessDocuments<T, U>(dm).Run)).Start();

            new AddProcMessageEntity(MeMessageEntityManager).Run();
        }

        protected AddProcMessageEntity(MessageEntityManager<LGJMessageEntity> MeMessageEntityManager)
        {
            mem = MeMessageEntityManager;
            //MyQueneMainForm = MeQueneMainForm;
            //ResultAsynchNotifyHandler = new AsynchNotifyHandler(MyQueneMainForm.AsynchCallBackHandler);
        }


        protected void Run()
        {
            int i = 0;
            while (false)
            {
                i++;
                //LGJMessageEntity AnyLGJMessageEntity = new LGJMessageEntity("title-Create-" + i.ToString(), "content-Socket");
                //mem.AddMessageEntity(AnyLGJMessageEntity);

                //if (ResultAsynchNotifyHandler != null) ResultAsynchNotifyHandler(AnyLGJMessageEntity.Title + ":" + AnyLGJMessageEntity.Content);
                Thread.Sleep(20);
            }
        }
    }
}
