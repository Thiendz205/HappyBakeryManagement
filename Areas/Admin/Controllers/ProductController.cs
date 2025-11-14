using HappyBakeryManagement.DTO;
using HappyBakeryManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;
using WebApplication3.Repository;

namespace HappyBakeryManagement.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        private readonly IProductServices _productService;
        private readonly IWebHostEnvironment _env;
        private readonly ICategoriesServices _categoryService;
        private readonly ILogger<ProductDTO> _logger;

        public ProductController(IProductServices productService, IWebHostEnvironment env, ICategoriesServices categoryService,ILogger<ProductDTO>logger)
        {
            _productService = productService;
            _env = env;
            _categoryService = categoryService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            var _products = _productService.GetAllProducts();
            return View(_products);
        }
        public IActionResult Add()
        {
           _logger.LogInformation("Accessed Add Product page at {Time}", DateTime.UtcNow);
            var categories = _categoryService.GetNameAndIDCategory();
            ViewBag.CategoryList = new SelectList(categories, "Id", "Name");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(ProductDTO productDTO)
        {
            var categories = _categoryService.GetNameAndIDCategory();
            ViewBag.CategoryList = new SelectList(categories, "Id", "Name");

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Vui lòng kiểm tra lại thông tin sản phẩm.");
                return View(productDTO);
            }

            try
            {
                if (productDTO.ImageFile != null)
                {
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                    var extension = Path.GetExtension(productDTO.ImageFile.FileName).ToLowerInvariant();

                    if (!allowedExtensions.Contains(extension))
                    {
                        ModelState.AddModelError("ImageFile", "❌ Chỉ cho phép định dạng .jpg, .jpeg hoặc .png!");
                        return View(productDTO);
                    }

                    if (productDTO.ImageFile.ContentType != "image/jpeg" &&
                        productDTO.ImageFile.ContentType != "image/png")
                    {
                        ModelState.AddModelError("ImageFile", "❌ Tệp tải lên không phải là ảnh hợp lệ!");
                        return View(productDTO);
                    }

                    string uploadsFolder = Path.Combine(_env.WebRootPath, "images/products");
                    Directory.CreateDirectory(uploadsFolder); 

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + productDTO.ImageFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        productDTO.ImageFile.CopyTo(stream);
                    }

                    productDTO.Image =  uniqueFileName;
                }
                else
                {
                    ModelState.AddModelError("ImageFile", "Vui lòng chọn ảnh sản phẩm!");
                    return View(productDTO);
                }

               await _productService.AddAsync(productDTO);

                TempData["success"] = "🎉 Thêm sản phẩm thành công!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Đã xảy ra lỗi khi thêm sản phẩm.");
                return View(productDTO);
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                // Lấy sản phẩm từ DB
                var product = _productService.GetProductById(id);

                if (product == null)
                {
                    TempData["error"] = $"Không tìm thấy sản phẩm có ID = {id}.";
                    return RedirectToAction("Index");
                }

                // Kiểm tra đường dẫn ảnh hợp lệ
                if (!string.IsNullOrEmpty(product.Image))
                {
                    string uploadsDir = Path.Combine(_env.WebRootPath, "images/products");
                    string oldFilePath = Path.Combine(uploadsDir, Path.GetFileName(product.Image));

                    // Ghi log để kiểm tra đường dẫn thực tế
                    _logger.LogInformation("Đang xóa ảnh: {FilePath}", oldFilePath);

                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                        _logger.LogInformation("✅ Đã xóa ảnh: {FilePath}", oldFilePath);
                    }
                    else
                    {
                        _logger.LogWarning("⚠️ Không tìm thấy ảnh tại: {FilePath}", oldFilePath);
                    }
                }

                // Xóa sản phẩm trong DB
                _productService.DeleteProduct(id);
                _logger.LogInformation("✅ Sản phẩm có ID {Id} đã được xóa thành công.", id);

                TempData["success"] = "✅ Sản phẩm đã được xóa thành công!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Lỗi khi xóa sản phẩm ID {Id}", id);
                TempData["error"] = $"❌ Lỗi khi xóa sản phẩm: {ex.Message}";
            }

            return RedirectToAction("Index");
        }


        [HttpGet]
        [Route("Edit")]
        public IActionResult Edit(int id)
        {
            var product = _productService.GetProductById(id);
            if (product == null) return NotFound();

            var categories = _categoryService.GetNameAndIDCategory();
            ViewBag.CategoryList = new SelectList(categories, "Id", "Name", product.CategoryID);

            return View(product);
        }

        [HttpPost]
        [Route("Edit")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ProductDTO productDTO)
        {
            var categories = _categoryService.GetNameAndIDCategory();
            ViewBag.CategoryList = new SelectList(categories, "Id", "Name", productDTO.CategoryID);

            if (!ModelState.IsValid)
            {
                TempData["error"] = "Vui lòng kiểm tra lại thông tin sản phẩm.";
                return View(productDTO);
            }

            try
            {
                var existingProduct = _productService.GetProductById(productDTO.Id);
                if (existingProduct == null)
                {
                    TempData["error"] = "Không tìm thấy sản phẩm cần chỉnh sửa.";
                    return RedirectToAction("Index");
                }

                // ✅ Xử lý upload ảnh mới
                if (productDTO.ImageFile != null)
                {
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                    var extension = Path.GetExtension(productDTO.ImageFile.FileName).ToLowerInvariant();

                    if (!allowedExtensions.Contains(extension))
                    {
                        ModelState.AddModelError("ImageFile", "❌ Chỉ cho phép định dạng .jpg, .jpeg hoặc .png!");
                        return View(productDTO);
                    }

                    // Xóa ảnh cũ nếu có
                    if (!string.IsNullOrEmpty(existingProduct.Image))
                    {
                        string oldPath = Path.Combine(_env.WebRootPath, "images/products", existingProduct.Image);
                        if (System.IO.File.Exists(oldPath))
                            System.IO.File.Delete(oldPath);
                    }

                    // Lưu ảnh mới
                    string uploadsFolder = Path.Combine(_env.WebRootPath, "images/products");
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + productDTO.ImageFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        productDTO.ImageFile.CopyTo(stream);
                    }

                    existingProduct.Image = uniqueFileName;
                }

                // ✅ Cập nhật thông tin khác
                existingProduct.Name = productDTO.Name;
                existingProduct.Detail = productDTO.Detail;
                existingProduct.Price = productDTO.Price;
                existingProduct.Quantity = productDTO.Quantity;
                existingProduct.CategoryID = productDTO.CategoryID;

                _productService.UpdateProduct(existingProduct);
                TempData["success"] = "🎉 Cập nhật sản phẩm thành công!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Lỗi Edit(): " + ex.ToString());
                TempData["error"] = "Đã xảy ra lỗi khi cập nhật sản phẩm.";
                return View(productDTO);
            }
        }


    }
}
