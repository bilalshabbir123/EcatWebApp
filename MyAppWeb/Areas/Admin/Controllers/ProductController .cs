using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MyApp.DataAccessLayer.Infrastructure.IRepository;
using MyApp.Models;
using MyApp.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

namespace MyAppWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private IUnitOfWork _unitOfWork;
        private IWebHostEnvironment _hostEnvironment;

        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment;
        }
        #region APICALL
        public IActionResult AllProducts()
        {
            var products = _unitOfWork.Product.GetAll(includePropperties:"Category");
            return Json(new { data = products });
        }
        #endregion
        public IActionResult Index()
        {
            //ProductVM productVM = new ProductVM();
            //productVM.Products = _unitOfWork.Product.GetAll();
            return View();
        }
        [HttpGet]
        [Authorize]
        public IActionResult CreateUpdate(int? id)
        {
            ProductVM vm = new ProductVM()
            {
                Product=new(),
                Categories = _unitOfWork.Category.GetAll().Select(x =>
                new SelectListItem()
                {
                    Text=x.Name,
                    Value=x.Id.ToString()
                })
            };
            if (id == null || id == 0)
            {
                return View(vm);
            }
            else
            {
               vm.Product = _unitOfWork.Product.GetT(x => x.Id == id);
                if (vm.Product == null)
                {
                    return NotFound();
                }
                else
                {
                    return View(vm);
                }
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateUpdate(ProductVM vm,IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string fileName = String.Empty;
                if (file!=null)
                {
                    string uploadDir = Path.Combine(_hostEnvironment.WebRootPath, "ProductImage");
                    fileName = Guid.NewGuid().ToString() + "-" + file.FileName;
                    string filePath = Path.Combine(uploadDir, fileName);
                    if (vm.Product.ImageUrl!=null)
                    {
                        var oldImagePath = Path.Combine(_hostEnvironment.WebRootPath,vm.Product.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                         vm.Product.ImageUrl = @"\ProductImage\" + fileName;
                }
                if (vm.Product.Id==0)
                {
                    _unitOfWork.Product.Add(vm.Product);
                    TempData["Succcess"] = "Product Created Done!";
                }
                else
                {
                    _unitOfWork.Product.Update(vm.Product);
                    TempData["Succcess"] = "Product Created Done!";
                }
                _unitOfWork.Save();
               
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }
        #region DeleteAPICALL
        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var product = _unitOfWork.Product.GetT(x => x.Id == id);
            if (product == null)
            {
                return Json(new { success = false, message = "Error in Fetching Data" });
            }
            else
            {
                var oldImagePath = Path.Combine(_hostEnvironment.WebRootPath, product.ImageUrl.TrimStart('\\'));
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
                _unitOfWork.Product.Delete(product);
                _unitOfWork.Save();
                return Json(new { success=true,message="Product Deleted"});
            }
        }
        #endregion
    }
}
