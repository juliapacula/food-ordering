using System;

namespace DatabaseStructure.Messages
{
    [Serializable]
    public class Success : Message
    {
        public override MessageType MessageType => MessageType.S_OK;
    }
}
