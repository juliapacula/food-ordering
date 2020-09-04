using System;

namespace DatabaseStructure.Messages
{
    [Serializable]
    public class TestCommand : Message
    {
        public string testMessage;

        public override MessageType MessageType => MessageType.TestCommand;
    }
}