namespace DatabaseStructure.Messages
{
    public enum MessageType
    {
        #region General

        Unknown,
        S_OK,
        Error,

        #endregion

        #region Commands

        TestCommand,
        InitOrder,
        FinalizeOrder,
        CancelOrder,

        #endregion
    }
}
