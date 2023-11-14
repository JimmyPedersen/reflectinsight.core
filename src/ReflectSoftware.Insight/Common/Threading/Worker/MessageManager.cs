using System;
using System.Collections.Generic;
using System.ServiceModel.Channels;
using System.Threading;

namespace RI.Threading.Worker
{
    internal class MessageManager : BaseThread
    {
        private readonly WorkManager FWorkManager;

        private readonly List<MessageManagerInfo> FMessageQueue;

        public MessageState[] FMessageStates;

        public int MessageCount
        {
            get
            {
                lock (FMessageQueue)
                {
                    return FMessageQueue.Count;
                }
            }
        }

        public MessageManager(WorkManager manager)
            : base("RI.Threading.Worker.MessageManager")
        {
            FWorkManager = manager;
            FMessageQueue = new List<MessageManagerInfo>();
            FMessageStates = new MessageState[Enum.GetNames(typeof(MessageManagerID)).Length];
        }

        protected override void OnInitializeThread()
        {
            base.OnInitializeThread();
            SetAllMessagesToState(MessageState.Allow);
        }

        protected override int GetWorkSleepValue()
        {
            return 250;
        }

        protected override int GetWaitOnTerminateThreadValue()
        {
            return 5000;
        }

        protected override void OnDispose()
        {
            lock (FMessageQueue)
            {
                foreach (MessageManagerInfo item in FMessageQueue)
                {
                    item.Dispose();
                }

                FMessageQueue.Clear();
                FMessageQueue.Capacity = 0;
            }
        }

        protected override void OnWork()
        {
            MessageManagerInfo[] array = null;
            lock (FMessageQueue)
            {
                array = FMessageQueue.ToArray();
            }

            if (FMessageQueue.Count == 0)
            {
                return;
            }

            MessageManagerInfo[] array2 = array;
            foreach (MessageManagerInfo messageManagerInfo in array2)
            {
                if (FMessageStates[messageManagerInfo.FMessageID.GetHashCode()] == MessageState.Allow)
                {
                    FWorkManager.ReceivedMQMessage(messageManagerInfo);
                }

                lock (FMessageQueue)
                {
                    FMessageQueue.Remove(messageManagerInfo);
                }
            }
        }

        public void SendMessages(List<MessageManagerInfo> messages)
        {
            lock (FMessageQueue)
            {
                FMessageQueue.AddRange(messages);
            }
        }

        public void SendMessage(MessageManagerInfo mInfo)
        {
            lock (FMessageQueue)
            {
                FMessageQueue.Add(mInfo);
            }
        }

        public void SendMessage(MessageManagerID mid, object mData)
        {
            SendMessage(new MessageManagerInfo(mid, mData));
        }

        public void SetMessageState(MessageManagerID mid, MessageState state)
        {
            lock (FMessageQueue)
            {
                FMessageStates[mid.GetHashCode()] = state;
            }
        }

        public void SetAllMessagesToState(MessageState state)
        {
            lock (FMessageQueue)
            {
                for (int i = 0; i < FMessageStates.Length; i++)
                {
                    FMessageStates[i] = state;
                }
            }
        }

        public void WaitOnEmptyQueue()
        {
            while (MessageCount != 0)
            {
                Thread.Sleep(100);
            }
        }
    }
}