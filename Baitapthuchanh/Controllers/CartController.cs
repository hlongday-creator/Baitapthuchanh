using Baitapthuchanh.Models.ViewModels;
using Baitapthuchanh.Models;
using Baitapthuchanh.Repository;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Scripting;
using System.Net;

namespace Baitapthuchanh.Controllers
{
    public class CartController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public CartController(DataContext _context, UserManager<ApplicationUser> userManage)
        {
            _dataContext = _context;
            _userManager = userManage;

        }
  
        public IActionResult Index()
        {
            List<CartItemModel> cartitems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();

            // Tính tổng số tiền trước giảm giá
            decimal totalBeforeDiscount = cartitems.Sum(x => x.Quantity * x.Price);

            // Tính tổng số tiền đã giảm giá
            decimal totalDiscount = cartitems.Sum(x => x.Discount);

            // Tính tổng số tiền sau khi đã giảm giá
            decimal grandTotal = totalBeforeDiscount - totalDiscount;

            // Tính tổng giá cũ sau khi đã giảm giá

            UserIndexViewModel cartVN = new()
            {
                CartItems = cartitems,
                GrandTotal = grandTotal,
                Total = totalBeforeDiscount,
                Discound = totalDiscount,

            };

            ViewBag.CartItemCount = GetCartItemCount(); // Truyền số lượng sản phẩm qua ViewBag
            return View(cartVN);
        }

        // Phương thức để lấy số lượng sản phẩm trong giỏ hàng
        private int GetCartItemCount()
        {
            List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            return cart.Sum(item => item.Quantity);
        }

        [HttpPost]
        public async Task<IActionResult> Add(int Id)
        {
            Product product = await _dataContext.Products.FindAsync(Id);
            List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            CartItemModel cartItem = cart.FirstOrDefault(c => c.ProductId == Id);

            if (cartItem != null)
            {
                cartItem.Quantity += 1;
            }
            else
            {
                cart.Add(new CartItemModel(product));
            }

            HttpContext.Session.SetJson("Cart", cart);

            return Json(new { success = true, message = "Đã Thêm Sản Phẩm Vào Giỏ Hàng Của Bạn Thành Công Rồi Nè" });
        }

        public async Task<IActionResult> Decrease(int Id)
        {
            List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart");
            CartItemModel cartItem = cart.FirstOrDefault(c => c.ProductId == Id);

            if (cartItem != null && cartItem.Quantity > 1)
            {
                cartItem.Quantity--;
            }
            else
            {
                cart.RemoveAll(p => p.ProductId == Id);
            }

            UpdateSessionCart(cart);
            TempData["success"] = "Bớt Sản Phẩm Trong Giỏ Hàng Thành Công";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Increase(int Id)
        {
            List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart");
            CartItemModel cartItem = cart.FirstOrDefault(c => c.ProductId == Id);

            if (cartItem != null)
            {
                cartItem.Quantity++;
            }

            UpdateSessionCart(cart);
            TempData["success"] = "Thêm Sản Phẩm Vào Giỏ Hàng Thành Công";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Remove(int Id)
        {
            List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart");
            cart.RemoveAll(p => p.ProductId == Id);
            UpdateSessionCart(cart);
            TempData["success"] = "Xóa Sản Phẩm Trong Giỏ Hàng Thành Công";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Clear()
        {
            HttpContext.Session.Remove("Cart");
            TempData["success"] = "Xóa Tất Cả Sản Phẩm Trong Giỏ Hàng Thành Công";
            return RedirectToAction("Index");
        }

        private void UpdateSessionCart(List<CartItemModel> cart)
        {
            if (cart.Count == 0)
            {
                HttpContext.Session.Remove("Cart");
            }
            else
            {
                HttpContext.Session.SetJson("Cart", cart);
            }
        }


        public IActionResult Error()
        {
            // Có thể thêm logic xử lý lỗi ở đây nếu cần thiết
            return View();
        }


        public IActionResult Details(int id)
        {
            var product = _dataContext.Products.Include(p => p.Category).FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }
        public async Task<IActionResult> ConfirmPaymentClient()
        {
            ApplicationUser user = await _userManager.GetUserAsync(User);
            var orderCode = Guid.NewGuid().ToString();
            var orderItem = new OrderModel
            {
                Ordercode = orderCode,
                UserName = user.UserName,

                Status = 1, // Giả sử status 1 là đơn hàng mới
                CreatedDate = DateTime.Now
            };

            // Thêm đơn hàng vào database
            _dataContext.Add(orderItem);
            await _dataContext.SaveChangesAsync();
            // Lấy thông tin giỏ hàng từ Session
            List<CartItemModel> cartItems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();

            // Tính tổng số lượng và tổng giá tiền của tất cả các mục OrderDetails
            int totalQuantity = cartItems.Sum(od => od.Quantity);
            decimal totalPrice = cartItems.Sum(od => od.Price * od.Quantity);




            // Tạo một đối tượng MoMoPayment để lưu vào cơ sở dữ liệu
          
            HttpContext.Session.Remove("Cart");
            await _dataContext.SaveChangesAsync();

            return View();
        }


    }

}

