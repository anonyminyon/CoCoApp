//using COCOApp.Hubs;
//using COCOApp.Services.Impl;
//using Microsoft.AspNetCore.SignalR;

//namespace COCOApp.Services;

//public class TransactionService { 
//    private readonly IHubContext<TransactionHub> _hubContext;

//    public TransactionService(IHubContext<TransactionHub> hubContext)
//    {
//        _hubContext = hubContext;
//    }

//    public async Task OnTransactionSuccess(int customerId)
//    {
//        await _hubContext.Clients.Group(customerId.ToString()).SendAsync("TransactionSuccess");
//    }
//}