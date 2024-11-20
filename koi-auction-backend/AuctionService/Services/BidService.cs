// using System.Collections.Concurrent;
// using BiddingService.Dto.AuctionLot;
// using BiddingService.Dto.BidLog;
// using BiddingService.HandleMethod;
// using BiddingService.IRepositories;
// using BiddingService.IServices;
// using BiddingService.Mappers;



// namespace BiddingService.Services
// {
//     public class BidService
//     {
//         // public readonly ConcurrentQueue<CreateBidLogDto> _bidQueue;
//         // public readonly ConcurrentDictionary<int, decimal> _userBalance;
//         // private readonly WalletService _walletService;
//         //private readonly IServiceScopeFactory _serviceScopeFactory;
//         // private readonly Dictionary<BidMethodType, Func<IBidStrategy>> _strategyFactory;
//         // private decimal _standardPrice;
//         // private decimal _stepPrice;
//         // protected AuctionLotBidDto? _auctionLotBidDto;

//         public readonly ConcurrentQueue<CreateBidLogDto> _bidQueue;
//         // private ConcurrentDictionary<int, decimal> _userBalance;
//         // private readonly WalletService _walletService;
//         // public WalletService? WalletService { get; set; }
//         // private readonly IServiceScopeFactory _serviceScopeFactory;
//         // private readonly Dictionary<BidMethodType, Func<IBidStrategy>> _strategyFactory;
//         // private decimal _standardPrice = 0;
//         // private decimal _stepPrice = 0;
//         // private AuctionLotBidDto? _auctionLotBidDto;
//         // public decimal StandardPrice { get; set; }
//         // public decimal StepPrice { get; set; }
//         // public ConcurrentDictionary<int, decimal> UserBalance { get => _userBalance; set { _userBalance = new(); } }

//         //private IBidStrategy _bidStrategy;
//         private IBidStrategy _currentStrategy;

//         // public AuctionLotBidDto? AuctionLotBidDto
//         // {
//         //     get => _auctionLotBidDto;
//         //     set
//         //     {
//         //         if (value == null)
//         //             throw new ArgumentNullException(nameof(value));

//         //         _auctionLotBidDto = value;
//         //         _standardPrice = _auctionLotBidDto.StartPrice;
//         //     }
//         // }


//         public BidService(IBidStrategy currentStrategy)
//         {
//             // _serviceScopeFactory = serviceScopeFactory;
//             // _userBalance = new ConcurrentDictionary<int, decimal>();
//             _bidQueue = new ConcurrentQueue<CreateBidLogDto>();
//             _currentStrategy = currentStrategy;
//         }

//         public void StrategySetUp()
//         {
//             // _bidStrategy = new ABidStrategyService(_auctionLotBidDto);


//         }

//         // Thêm phương thức này để lấy chiến lược dựa trên BidMethodType
//         // public void SetStrategy(BidMethodType bidMethodType)
//         // {
//         //     if (_strategies.TryGetValue(bidMethodType, out var strategy))
//         //     {
//         //         _currentStrategy = strategy;
//         //     }
//         //     else
//         //     {
//         //         throw new ArgumentException("Invalid bid method type");
//         //     }
//         // }

//         public async Task<bool> IsBidValid(CreateBidLogDto bid, AuctionLotBidDto auctionLotBidDto)
//         {
//             if (_currentStrategy == null)
//                 throw new InvalidOperationException("Strategy has not been set");

//             return await _currentStrategy.IsBidValid(bid, auctionLotBidDto);
//         }

//         public CreateBidLogDto? GetWinner()
//         {
//             if (_currentStrategy == null)
//                 throw new InvalidOperationException("Strategy has not been set");

//             return _currentStrategy.GetWinner();
//         }
//         public void AddBidLog(CreateBidLogDto bid)
//         {
//             // Cập nhật _highestBid nếu bid mới lớn hơn
//             // Cập nhật giá trị cao nhất
//             _bidQueue.Enqueue(bid);// Thêm bid hợp lệ vào hàng đợi
//             // await Task.Run(() => ProcessQueue());
//             //await ProcessQueue();
//         }

//         // public async Task<bool> IsBidValid(CreateBidLogDto bid)
//         // {
//         //     //kiểm tra AuctionLotStaus
//         //     if (_auctionLotBidDto != null && _auctionLotBidDto.AuctionLotId == bid.AuctionLotId
//         //             //&& _cacheService.GetBalance(bid.BidderId) <= bid.BidAmount
//         //             && bid.BidAmount >= _standardPrice)
//         //     {
//         //         if (!_userBalance.TryGetValue(bid.BidderId, out var balance))
//         //         {
//         //             var wallet = await _walletService.GetBalanceByIdAsync(bid.BidderId);
//         //             balance = wallet!.Balance;
//         //             // Thêm balance vào Dictionary _userBalance
//         //             _userBalance[bid.BidderId] = balance;
//         //         }
//         //         if (bid.BidAmount <= _userBalance[bid.BidderId])
//         //         {
//         //             _standardPrice = bid.BidAmount + _stepPrice;
//         //             return true;
//         //         }

//         //     }
//         //     return false;
//         // }
//         // private decimal CalculateStepPrice(decimal startPrice, int stepPercent)
//         // {
//         //     return startPrice * stepPercent / 100;
//         // }

//         // public async Task ProcessQueue()
//         // {
//         //     while (_bidQueue.TryDequeue(out var bid))
//         //     {
//         //         System.Console.WriteLine($"bid = {bid.BidAmount}");
//         //         using (var scope = _serviceScopeFactory.CreateScope())
//         //         {
//         //             var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

//         //             await unitOfWork.BidLog.CreateAsync(bid.ToBidLogFromCreateBidLogDto());
//         //             if (!await unitOfWork.SaveChangesAsync())
//         //             {
//         //                 throw new Exception("An error occurred while saving the data");
//         //             }
//         //         }
//         //     }
//         // }
//     }
// }


using System.Collections.Concurrent;
using AuctionService.Dto.AuctionLot;
using AuctionService.Dto.BidLog;
using AuctionService.HandleMethod;
using AuctionService.IRepository;
using AuctionService.IServices;
using AuctionService.Mapper;
using AuctionService.Enums;
using Microsoft.AspNetCore.SignalR;
using AuctionService.Hubs;
using AuctionService.Dto.Wallet;

namespace AuctionService.Services
{

    public class BidService
    {
        private readonly ConcurrentQueue<CreateBidLogDto> _bidQueue; //add bid log
        private IBidStrategy? _currentStrategy; // chonj phuong thuc
        public IBidStrategy? CurrentStrategy
        {
            get => _currentStrategy;
        }
        private AuctionLotBidDto? _auctionLotBidDto; // dto
        public AuctionLotBidDto? AuctionLotBidDto
        {
            get => _auctionLotBidDto;
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));

                _auctionLotBidDto = value;
            }
        }
        private readonly WalletService? _walletService;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IHubContext<BidHub> _bidHub;

        public BidService(IServiceScopeFactory serviceScopeFactory, IHubContext<BidHub> bidHub)
        {
            _bidQueue = new ConcurrentQueue<CreateBidLogDto>();
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var httpClient = new HttpClient();
            _walletService = new WalletService(httpClient, configuration);
            _serviceScopeFactory = serviceScopeFactory;
            _bidHub = bidHub;
        }
        public void SetStrategy(AuctionLotBidDto auctionLotBidDto)
        {
            System.Console.WriteLine($"BidService: set strategy for {auctionLotBidDto.AuctionLotId} <-> {auctionLotBidDto.AuctionMethodId}");
            // Lấy chiến lược từ scope để đảm bảo mọi dịch vụ phụ thuộc được khởi tạo đúng
            switch (auctionLotBidDto.AuctionMethodId)
            {
                case (int)BidMethodType.FixedPrice:
                    _currentStrategy = new FixedPriceBidStrategy();
                    break;
                case (int)BidMethodType.SealedBid:
                    _currentStrategy = new SealedBidStrategy();
                    break;
                case (int)BidMethodType.AscendingBid:
                    try
                    {
                        _currentStrategy = new AscendingBidStrategy(_bidHub);
                        if (_currentStrategy is AscendingBidStrategy ascendingBidStrategy)
                        {
                            ascendingBidStrategy.SetUp(_auctionLotBidDto!);
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Console.Error.WriteLine(ex.Message);
                    }
                    break;
                case (int)BidMethodType.DescendingBid:
                    try
                    {
                        _currentStrategy = new DescendingBidStrategy(_bidHub);
                        if (_currentStrategy is DescendingBidStrategy descendingBidStrategy)
                        {
                            descendingBidStrategy.SetUp(_auctionLotBidDto!);
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Console.Error.WriteLine(ex.Message);
                    }
                    break;
                // Thêm các case khác ở đây nếu có
                default:
                    throw new ArgumentException("Invalid auctionLotMethodId");
            }
        }
        public DateTime? GetPredictEndTime()
        {
            return _auctionLotBidDto!.PredictEndTime;
        }

        public HighestBidLog? GetWinner()
        {
            System.Console.WriteLine("BidService: GetWinner");
            if (_currentStrategy == null)
                throw new InvalidOperationException("Strategy has not been set");

            return _currentStrategy.GetWinner();
        }

        public decimal GetPriceDesc()
        {
            if (_currentStrategy == null)
                throw new InvalidOperationException("Strategy has not been set");

            if (_currentStrategy is DescendingBidStrategy descendingBidStrategy)
            {
                return descendingBidStrategy.CurrentPrice!.Value;
            }
            return 0;
        }

        public bool IsBidValid(CreateBidLogDto bid)
        {
            System.Console.WriteLine("BidService: IsBidValid");
            if (_currentStrategy == null)
                throw new InvalidOperationException("Strategy has not been set");


            if (!_currentStrategy.IsBidValid(bid, _auctionLotBidDto))
            {
                System.Console.WriteLine("BidServie: Bid not valid");
                return false;
            }
            System.Console.WriteLine($"BidServie: Bid valid, Amount: {bid.BidAmount}");
            return true;
        }

        public async Task AddBidLog(CreateBidLogDto bid)
        {
            _bidQueue.Enqueue(bid);
            await Task.Run(() => ProcessQueue());
        }

        // Phương thức xử lý hàng đợi bid, lưu vào cơ sở dữ liệu hoặc xử lý logic khác
        public async Task ProcessQueue()
        {
            while (_bidQueue.TryDequeue(out var bid))
            {
                Console.WriteLine($"Processing bid with amount: {bid.BidAmount}");

                // Tạo scope để quản lý các dịch vụ scoped
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                    // Lưu bid vào cơ sở dữ liệu
                    await unitOfWork.BidLog.CreateAsync(bid.ToBidLogFromCreateBidLogDto());
                    if (!await unitOfWork.SaveChangesAsync())
                    {
                        throw new Exception("An error occurred while saving the bid log.");
                    }
                }

            }
        }

        // payment
        public async Task PaymentAsync(PaymentDto paymentDto)
        {
            await _walletService!.PaymentAsync(paymentDto);
        }
    }
}