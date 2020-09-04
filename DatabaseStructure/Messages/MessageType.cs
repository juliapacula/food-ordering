namespace DatabaseStructure.Messages
{
    public enum MessageType
    {
        #region General

        Unknown,

        #endregion

        #region Commands

        TestCommand,
        InitOrder,
        FinalizeOrder,
        CancelOrder,
        GetAllDishes,

        #endregion

        #region Replies

        S_OK,
        FinalizingError,
        FinalizingSuccess,
        AllDishes

        #endregion
    }
}
