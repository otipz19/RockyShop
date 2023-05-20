using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RockyShop.DataAccess.Repository.Interfaces;
using RockyShop.Model.Enums;
using RockyShop.Model.Models;
using RockyShop.Model.ViewModels;
using RockyShop.Utility.Interfaces;
using RockyShop.Utility.Utilities;
using System.Linq.Expressions;

namespace RockyShop.Controllers
{
    [Authorize(Roles = Constants.AdminRole)]
    public class OrderController : Controller
    {
        private readonly IOrderHeaderRepository _orderHeaderRepo;
        private readonly IOrderDetailsRepository _orderDetailsRepo;
        private readonly IBraintreeService _braintreeService;

        public OrderController(IOrderHeaderRepository orderHeaderRepo,
            IOrderDetailsRepository orderDetailsRepo,
            IBraintreeService braintreeService)
        {
            _orderDetailsRepo = orderDetailsRepo;
            _orderHeaderRepo = orderHeaderRepo;
            _braintreeService = braintreeService;
        }

        [HttpGet]
        public IActionResult Index(int? searchStatus, string searchName = "", string searchEmail = "", string searchPhone = "")
        {
            Expression<Func<OrderHeader, bool>> filter =
                o => o.FullName.Contains(searchName) && o.Email.Contains(searchEmail) && o.PhoneNumber.Contains(searchPhone);
            if(searchStatus != null)
            {
                Expression<Func<OrderHeader, bool>> searchStatusFilter = o => o.OrderStatus == (OrderStatus)searchStatus;
                filter = CombineFilters(filter, searchStatusFilter);
            }
            var viewModel = new OrderIndexVM()
            {
                OrderHeaders = _orderHeaderRepo.GetAll(filter),
                StatusSelectList = Enum.GetValues<OrderStatus>()
                    .Select(os => new SelectListItem()
                    {
                        Value = ((int)os).ToString(),
                        Text = os.ToString(),
                    }),
            };
            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            OrderHeader orderHeader = _orderHeaderRepo.Find(id);
            if (orderHeader == null)
                return NotFound();
            var viewModel = new OrderDetailsVM()
            {
                OrderHeader = orderHeader,
                OrderDetailsList = _orderDetailsRepo.GetAllIncludeAll(d => d.OrderHeaderId == orderHeader.Id, isTracking: false),
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult StartProcessing(OrderDetailsVM viewModel)
        {
            OrderHeader orderHeader = _orderHeaderRepo.Find(viewModel.OrderHeader.Id);
            if (orderHeader == null)
                return NotFound();
            orderHeader.OrderStatus = OrderStatus.Processing;
            _orderHeaderRepo.SaveChanges();
            return RedirectToAction(nameof(Details), new {id = orderHeader.Id} );
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ShipOrder(OrderDetailsVM viewModel)
        {
            OrderHeader orderHeader = _orderHeaderRepo.Find(viewModel.OrderHeader.Id);
            if (orderHeader == null)
                return NotFound();
            orderHeader.OrderStatus = OrderStatus.Shipped;
            orderHeader.ShippingDate = DateTime.Now;
            _orderHeaderRepo.SaveChanges();
            return RedirectToAction(nameof(Details), new { id = orderHeader.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelOrder(OrderDetailsVM viewModel)
        {
            OrderHeader orderHeader = _orderHeaderRepo.Find(viewModel.OrderHeader.Id);
            if (orderHeader == null)
                return NotFound();
            if(orderHeader.OrderStatus != OrderStatus.Refunded || orderHeader.OrderStatus != OrderStatus.Cancelled)
                orderHeader.OrderStatus = await _braintreeService.RefundAsync(orderHeader);
            _orderHeaderRepo.SaveChanges();
            return RedirectToAction(nameof(Details), new { id = orderHeader.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateOrderDetails(OrderDetailsVM viewModel)
        {
            OrderHeader orderHeader = _orderHeaderRepo.FirstOrDefault(o => o.Id == viewModel.OrderHeader.Id, isTracking: false);
            if (orderHeader == null)
                return NotFound();
            if (ModelState.IsValid)
                _orderHeaderRepo.Update(viewModel.OrderHeader);
            _orderHeaderRepo.SaveChanges();
            return RedirectToAction(nameof(Details), new { id = orderHeader.Id });
        }

        private Expression<Func<OrderHeader, bool>> CombineFilters(
            Expression<Func<OrderHeader, bool>> first,
            Expression<Func<OrderHeader, bool>> second)
        {
            var toInvoke = Expression.Invoke(second, first.Parameters);
            return Expression.Lambda<Func<OrderHeader, bool>>(Expression.AndAlso(first.Body, toInvoke), first.Parameters);
        }
    }
}
