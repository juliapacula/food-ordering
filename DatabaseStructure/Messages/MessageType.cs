namespace DatabaseStructure.Messages
{
    public enum MessageType
    {
        #region General

        Unknown,

        #endregion

        #region Commands

        InitOrder,
        FinalizeOrder,
        CancelOrder,
        GetAllDishes,

        #endregion

        #region Replies

        FinalizingError,
        FinalizingSuccess,

        #endregion
    }
}
