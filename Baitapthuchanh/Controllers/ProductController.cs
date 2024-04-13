using Baitapthuchanh.Models;
using Baitapthuchanh.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;
using System.Linq;
using System.Web;
using Baitapthuchanh.Repository;

namespace Baitapthuchanh.Controllers
{

    public class ProductController : Controller
    {

        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly DataContext _dataContext;

        public ProductController(IProductRepository productRepository, ICategoryRepository
        categoryRepository, DataContext context)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _dataContext = context;
        }
        public async Task<IActionResult> Index()
        {
            var products = await _productRepository.GetAllAsync();
            return View(products);
        }
        // Hiển thị form thêm sản phẩm mới
        
        // Xử lý thêm sản phẩm mới
        
        public async Task<IActionResult> Display(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }
        [HttpGet]
        public IActionResult CheckQuantity(int id)
        {
            var product = _dataContext.Products.FirstOrDefault(p => p.Id == id);
            if (product != null && product.Quantity > 0)
            {
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

    }
}