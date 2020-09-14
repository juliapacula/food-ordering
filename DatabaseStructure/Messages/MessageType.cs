﻿namespace DatabaseStructure.Messages
{
    public enum MessageType
    {
        InitOrder,
        FinalizeOrder,
        RegisterOrder,
        CancelOrder,
        FinalizingError,
        Error,
        FinalizingSuccess,
    }
}
