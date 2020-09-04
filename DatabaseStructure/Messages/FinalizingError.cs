namespace DatabaseStructure.Messages
{
    public class FinalizingError : Message
    {
        public string errorMessage;
        
        public override MessageType MessageType => MessageType.FinalizingError;
    }
}