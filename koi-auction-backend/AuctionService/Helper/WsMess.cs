using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuctionService.Helper
{
    public static class WsMess
    {
        public static string ReceiveWinner = "ReceiveWinner";
        public static string ReceiveSuccessBid = "ReceiveSuccessBid";
        public static string ReceivePredictEndTime = "ReceivePredictEndTime";
        public static string ReceiveFetchBidLog = "ReceiveFetchBidLog";
        public static string ReceiveFetchAuctionLot = "ReceiveFetchAuctionLot";
        public static string ReceiveFetchWinnerPrice = "ReceiveFetchWinnerPrice";
        public static string ReceiveExceptionMessage = "ReceiveExceptionMessage";
        public static string ReceiveStartAuctionLot = "ReceiveStartAuctionLot";
        public static string ReceiveEndAuctionLot = "ReceiveEndAuctionLot";
        public static string ReceivePriceDesc = "ReceivePriceDesc";
        public static string ReceivePendingPayment = "ReceivePendingPayment";
        public static string ReceiveLoading = "ReceiveLoading";
    }
}