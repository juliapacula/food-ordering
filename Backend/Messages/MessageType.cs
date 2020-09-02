namespace Backend.Messages
{
    public enum MessageType
    {
        #region General

        Unknown,
        S_OK,
        Error,

        #endregion

        #region Commands

        AddToCart,
        RemoveFromCard,
        GetAvailableDishes,
        GetChosenDishes,
        FinalizeOrder,

        #endregion

        #region Queries

        AvailableDishes,
        ChosenDishes,

        #endregion
    }
}
