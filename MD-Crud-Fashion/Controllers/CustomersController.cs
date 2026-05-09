#nullable enable
using MD_Crud_Fashion.Data;
using MD_Crud_Fashion.Models;
using MD_Crud_Fashion.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace MD_Crud_Fashion.Controllers
{
    
    public class CustomersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _he;

        public CustomersController(ApplicationDbContext context, IWebHostEnvironment he)
        {
            _context = context;
            _he = he;
        }
 
        public async Task<IActionResult> Index()
        {
            var customers = await _context.Customers
                .Include(x => x.OrderEntries)
                .ThenInclude(o => o.Product)
                .OrderByDescending(x => x.CustomersId)
                .ToListAsync();

            return View(customers);
        }
        public IActionResult AddNewProduct(int? id)
        {
            ViewBag.products = new SelectList(_context.Products, "ProductsId", "ProductsName", id?.ToString() ?? "");
            return PartialView("_addNewProduct");
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }
        public IActionResult Aggregation()
        {
            var customers = _context.Customers.Include(x => x.OrderEntries).ToList();

            ViewBag.count = customers.Count;
            ViewBag.max = customers.Any() ? customers.Max(x => x.OrderEntries.Count) : 0;
            ViewBag.min = customers.Any() ? customers.Min(x => x.OrderEntries.Count) : 0;
            ViewBag.sum = customers.Sum(x => x.OrderEntries.Count);
            ViewBag.avg = customers.Any() ? customers.Average(x => x.OrderEntries.Count) : 0;

            return View(customers);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CustomerVM customerVM, int[] productId)
        {
            if (ModelState.IsValid)
            {
                var customer = new Customer
                {
                    CustomersName = customerVM.CustomersName,
                    PaymentDate = customerVM.PaymentDate,
                    CustomerSize = customerVM.CustomerSize,
                    UrgentDelivery = customerVM.UrgentDelivery
                };

                // Image upload
                if (customerVM.PictureFile != null)
                {
                    string webroot = _he.WebRootPath;
                    string folder = "Images";
                    string ext = Path.GetExtension(customerVM.PictureFile.FileName);
                    string imgFileName = Path.GetRandomFileName() + ext;
                    string fileSave = Path.Combine(webroot, folder, imgFileName);

                    using (var stream = new FileStream(fileSave, FileMode.Create))
                    {
                        await customerVM.PictureFile.CopyToAsync(stream);
                        customer.Picture = "/" + folder + "/" + imgFileName;
                    }
                }

                _context.Customers.Add(customer);
                if (productId?.Length > 0)
                {
                    foreach (var item in productId)
                    {
                        var orderEntry = new OrderEntry
                        {
                            Customer = customer,
                            ProductsId = item
                        };
                        _context.OrderEntries.Add(orderEntry);
                    }
                }

                await _context.SaveChangesAsync();
                return PartialView("_success");
            }
            return PartialView("_error");
        }
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var customer = await _context.Customers.FirstOrDefaultAsync(x => x.CustomersId == id);
            if (customer == null) return NotFound();

            var customerProducts = await _context.OrderEntries.Where(x => x.CustomersId == id).ToListAsync();

            var customerVM = new CustomerVM
            {
                CustomersId = customer.CustomersId,
                CustomersName = customer.CustomersName,
                PaymentDate = customer.PaymentDate,
                CustomerSize = customer.CustomerSize,
                Picture = customer.Picture,
                UrgentDelivery = customer.UrgentDelivery,
                ProductList = customerProducts.Select(x => x.ProductsId).ToList()
            };

            return View(customerVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CustomerVM customerVM, int[] productId)
        {
            if (ModelState.IsValid)
            {
                var customer = new Customer
                {
                    CustomersId = customerVM.CustomersId,
                    CustomersName = customerVM.CustomersName,
                    PaymentDate = customerVM.PaymentDate,
                    CustomerSize = customerVM.CustomerSize,
                    UrgentDelivery = customerVM.UrgentDelivery
                };

                // Image update
                if (customerVM.PictureFile != null)
                {
                    string webroot = _he.WebRootPath;
                    string folder = "Images";
                    string ext = Path.GetExtension(customerVM.PictureFile.FileName);
                    string imgFileName = Path.GetRandomFileName() + ext;
                    string fileSave = Path.Combine(webroot, folder, imgFileName);

                    using (var stream = new FileStream(fileSave, FileMode.Create))
                    {
                        await customerVM.PictureFile.CopyToAsync(stream);
                        customer.Picture = "/" + folder + "/" + imgFileName;
                    }
                }
                else
                {
                    customer.Picture = customerVM.Picture;
                }

                _context.Update(customer);
                var existingEntries = _context.OrderEntries.Where(x => x.CustomersId == customer.CustomersId).ToList();
                _context.OrderEntries.RemoveRange(existingEntries);
                if (productId?.Length > 0)
                {
                    foreach (var item in productId)
                    {
                        var orderEntry = new OrderEntry
                        {
                            CustomersId = customer.CustomersId,
                            ProductsId = item
                        };
                        _context.OrderEntries.Add(orderEntry);
                    }
                }

                await _context.SaveChangesAsync();
                return PartialView("_success");
            }
            return PartialView("_error");
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var customer = await _context.Customers
                .Include(x => x.OrderEntries)
                .FirstOrDefaultAsync(m => m.CustomersId == id);

            if (customer == null) return NotFound();

             
            var customerVM = new CustomerVM
            {
                CustomersId = customer.CustomersId,
                CustomersName = customer.CustomersName,
                PaymentDate = customer.PaymentDate,
                CustomerSize = customer.CustomerSize,
                Picture = customer.Picture,
                UrgentDelivery = customer.UrgentDelivery,
               

                ProductList = customer.OrderEntries.Select(x => x.ProductsId).ToList()
            };

            return View(customerVM);
        }
        [Authorize(Roles = "Admin,Manager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int CustomersId)
        {
            var project = await _context.Customers.FindAsync(CustomersId);

            if (project != null)
            {
                var existEquipment = _context.OrderEntries.Where(x => x.CustomersId == CustomersId).ToList();
                _context.OrderEntries.RemoveRange(existEquipment);
                _context.Customers.Remove(project);

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
