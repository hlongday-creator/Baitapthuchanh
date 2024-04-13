using Baitapthuchanh.Models;
using Baitapthuchanh.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Baitapthuchanh.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
    public class OrderController : Controller
    {
        private readonly DataContext _dataContext;

        public OrderController(DataContext context)
        {
            _dataContext = context;
        }


        public async Task<IActionResult> Index()
        {
            var orders = await _dataContext.Orders.OrderByDescending(p => p.Id).ToListAsync();
            int totalOrders = orders.Count();
            ViewBag.TotalOrders = totalOrders;
            return View(orders);
        }
    }
}
