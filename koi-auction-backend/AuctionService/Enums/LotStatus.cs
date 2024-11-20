namespace AuctionService.Enums
{
    public enum LotStatus
    {
        Pending = 1,
        Approved = 2,
        Rejected = 3,
        InAuction = 4,
        UnSold = 5,
        ToPay = 6,
        ToShip = 7,
        ToReceive = 8,
        Completed = 9,
        Canceled = 10,
        PaymentOverdue = 11
    }
}