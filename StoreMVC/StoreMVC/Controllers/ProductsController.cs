using Microsoft.AspNetCore.Mvc;
using StoreMVC.Models;
using StoreMVC.Services;

namespace StoreMVC.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDBContext context;
        private readonly IWebHostEnvironment environment;

        public ProductsController(ApplicationDBContext context, IWebHostEnvironment environment)
        {
            this.context = context;
            this.environment = environment;

        }
        public IActionResult Index()
        {
            var products = context.Products.OrderByDescending(p=>p.Id).ToList();
            return View(products);
        }
        public IActionResult Create()
        { return View(); }

            [HttpPost]
        public IActionResult Create(ProductDto productDto)
        {
            if (productDto.ImageFile == null)
            {
                ModelState.AddModelError("ImageFile", "The image file is required");
            }
            if (!ModelState.IsValid)
            {
                return View(productDto);
            }

            string newFileName = DateTime.Now.ToString("M");
            newFileName += Path.GetExtension(productDto.ImageFile!.FileName);

            string imageFullPath=environment.WebRootPath+ "/products/"+newFileName;
            using(var stream = System.IO.File.Create(imageFullPath))
            {
                productDto.ImageFile.CopyTo(stream);
            }

            Product product = new Product()
            {
                Name = productDto.Name,
                Brand = productDto.Brand,
                Category = productDto.Category,
                Price = productDto.Price,
                Description = productDto.Description,
                ImageFileName = newFileName,
            };

            context.Products.Add(product);  
            context.SaveChanges();

            return RedirectToAction("Index","Products");
        }
        public IActionResult Edit(int id)
        {
            var product = context.Products.Find(id);
            if (product == null)
            {
                return RedirectToAction("Index", "Products");
            }
            var productDto = new ProductDto()
            {
                Name = product.Name,
                Brand = product.Brand,
                Category = product.Category,
                Price = product.Price,
                Description = product.Description,
            };
            ViewData["ProductId"] = product.Id;
            ViewData["ImageFileName"] = product.ImageFileName;

            return View(productDto);
        }

        //[HttpPost]
        //public IActionResult Edit(int id,ProductDto productDto )
        //{
        //    var product = context.Products.Find(id);
        //    if (product==null)
        //    {
        //        return RedirectToAction("Index", "Products");
                
        //    }

        //    if (!ModelState.IsValid)
        //    {

        //        ViewData["ProductId"] = product.Id;
        //        ViewData["ImageFileName"] = product.ImageFileName;


        //        return View(productDto);
        //    }

        //    string newFileName = product.ImageFileName;
        //    if (productDto.ImageFile != null)
        //    {
        //       // newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
        //        newFileName += Path.GetExtension(productDto.ImageFile!.FileName);

        //        string imageFullPath = environment.WebRootPath + "/products/" + newFileName;
        //        using (var stream = System.IO.File.Create(imageFullPath))
        //        {
        //            productDto.ImageFile.CopyTo(stream);
        //        }

        //        string oldImageFullPath = environment.WebRootPath + "/products" + product.ImageFileName;
        //        System.IO.File.Delete(oldImageFullPath);


        //    }


        //}

        //update



        //delete
        public IActionResult Delete(int id)
        {
            var product=context.Products.Find(id);
            if (product == null)
            {
                return RedirectToAction("Index", "Products");
            }
            string imageFullPath = environment.WebRootPath + "/products" + product.ImageFileName;
            System.IO.File.Delete(imageFullPath);
            context.Products.Remove(product);
            context.SaveChanges(true);
            return RedirectToAction("Index", "Products");
        }

        

    }
}
