using Braintree;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using RockyDataAccess.Data.Repository.IRepository;
using RockyModels;
using RockyModels.Models;
using RockyModels.ViewModels;
using RockyUtility;
using RockyUtility.Braintree;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Rocky.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IEmailSender _emailSender;
        private readonly IBraintreeGate _braintreeGate;
        private readonly IApplicationUserRepository _userRepo;
        private readonly IProductRepository _prodRepo;
        private readonly IInquiryHeaderRepository _inqHRepo;
        private readonly IInquiryDetailRepository _inqDRepo;
        private readonly IOrderHeaderRepository _orderHRepo;
        private readonly IOrderDetailRepository _orderDRepo;

        [BindProperty]
        private ProductUserViewModel ProductUserVM { get; set; }

        public CartController(IWebHostEnvironment webHostEnvironment, IEmailSender emailSender, IBraintreeGate braintreeGate, IApplicationUserRepository userRepo, IProductRepository prodRepo, IInquiryHeaderRepository inqHRepo, IInquiryDetailRepository inqDRepo, IOrderHeaderRepository orderHRepo, IOrderDetailRepository orderDRepo)
        {
            _webHostEnvironment = webHostEnvironment;
            _emailSender = emailSender;
            _braintreeGate = braintreeGate;
            _userRepo = userRepo;
            _prodRepo = prodRepo;
            _inqHRepo = inqHRepo;
            _inqDRepo = inqDRepo;
            _orderHRepo = orderHRepo;
            _orderDRepo = orderDRepo;
        }

        public IActionResult Index()
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart) != null && HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart).Count > 0)
            {
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart);
            }

            List<int> productsInCart = shoppingCartList.Select(i => i.ProductId).ToList();
            IEnumerable<Product> productTempList = _prodRepo.GetAll(p => productsInCart.Contains(p.Id));
            IList<Product> productList = new List<Product>();

            foreach (var cart in shoppingCartList)
            {
                Product tempProduct = productTempList.FirstOrDefault(p => p.Id == cart.ProductId);
                tempProduct.TempSqFt = cart.SqFt;
                productList.Add(tempProduct);
            }

            return View(productList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Index")]
        public IActionResult IndexPost(IEnumerable<Product> productList)
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            foreach (Product product in productList)
            {
                shoppingCartList.Add(new ShoppingCart { ProductId = product.Id, SqFt = product.TempSqFt });
            }

            HttpContext.Session.Set(WC.SessionCart, shoppingCartList);

            return RedirectToAction(nameof(Summary));
        }

        public IActionResult Summary()
        {
            ApplicationUser applicationUser;

            if (User.IsInRole(WC.AdminRole))
            {
                if (HttpContext.Session.Get<int>(WC.SessionInquiryId) != 0)
                {
                    InquiryHeader inquiryHeader = _inqHRepo.FirstOrDefault(inq => inq.Id == HttpContext.Session.Get<int>(WC.SessionInquiryId));
                    applicationUser = new ApplicationUser()
                    {
                        Email = inquiryHeader.Email,
                        FullName = inquiryHeader.FullName,
                        PhoneNumber = inquiryHeader.PhoneNumber
                    };
                }
                else
                {
                    applicationUser = new ApplicationUser();
                }

                var gateway = _braintreeGate.GetGateway();
                var clientToken = gateway.ClientToken.Generate();

                ViewBag.ClientToken = clientToken;
            }
            else
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                applicationUser = _userRepo.FirstOrDefault(u => u.Id == claim.Value);
            }

            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart) != null && HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart).Count > 0)
            {
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart);
            }

            List<int> productsInCart = shoppingCartList.Select(i => i.ProductId).ToList();
            IEnumerable<Product> productList = _prodRepo.GetAll(u => productsInCart.Contains(u.Id)).ToList();

            ProductUserVM = new ProductUserViewModel()
            {
                ApplicationUser = applicationUser
            };

            foreach (var cart in shoppingCartList)
            {
                Product tempProduct = _prodRepo.FirstOrDefault(p => p.Id == cart.ProductId);
                tempProduct.TempSqFt = cart.SqFt;
                ProductUserVM.ProductList.Add(tempProduct);
            }

            return View(ProductUserVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Summary")]
        public async Task<IActionResult> SummaryPost(IFormCollection collection, ProductUserViewModel ProductUserVM)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (User.IsInRole(WC.AdminRole))
            {
                OrderHeader orderHeader = new OrderHeader()
                {
                    CreatedByUserId = claim.Value,
                    FinalOrderTotal = ProductUserVM.ProductList.Sum(x => x.TempSqFt * x.Price),
                    City = ProductUserVM.ApplicationUser.City ?? "",
                    StreetAddress = ProductUserVM.ApplicationUser.StreetAddress ?? "",
                    State = ProductUserVM.ApplicationUser.State ?? "",
                    PostalCode = ProductUserVM.ApplicationUser.PostalCode ?? "",
                    FullName = ProductUserVM.ApplicationUser.FullName ?? "",
                    Email = ProductUserVM.ApplicationUser.Email ?? "",
                    PhoneNumber = ProductUserVM.ApplicationUser.PhoneNumber ?? "",
                    OrderDate = DateTime.Now,
                    OrderStatus = WC.StatusPending
                };

                _orderHRepo.Add(orderHeader);
                _orderHRepo.Save();

                foreach (var product in ProductUserVM.ProductList)
                {
                    OrderDetail orderDetail = new OrderDetail()
                    {
                        OrderHeaderId = orderHeader.Id,
                        PricePerSqFt = product.Price,
                        SqFt = product.TempSqFt,
                        ProductId = product.Id
                    };
                    _orderDRepo.Add(orderDetail);
                }
                _orderDRepo.Save();

                string nonceFromTheClient = collection["payment_method_nonce"];

                var request = new TransactionRequest
                {
                    Amount = Convert.ToDecimal(orderHeader.FinalOrderTotal),
                    PaymentMethodNonce = nonceFromTheClient,
                    OrderId = orderHeader.Id.ToString(),
                    Options = new TransactionOptionsRequest
                    {
                        SubmitForSettlement = true
                    }
                };

                var gateway = _braintreeGate.GetGateway();
                Result<Transaction> result = gateway.Transaction.Sale(request);

                if (result.Target.ProcessorResponseText == "Approved")
                {
                    orderHeader.TransactionId = result.Target.Id;
                    orderHeader.OrderStatus = WC.StatusApproved;
                }
                else
                {
                    orderHeader.OrderStatus = WC.StatusCancelled;
                }

                _orderHRepo.Save();

                return RedirectToAction(nameof(InquiryConfirmation), new { id = orderHeader.Id });
            }
            else
            {
                var PathToTemplate = _webHostEnvironment.WebRootPath + Path.DirectorySeparatorChar.ToString() + "templates" + Path.DirectorySeparatorChar.ToString() + "Inquiry.html";
                var subject = "New Inquiry";
                string HtmlBody = "";

                using (StreamReader sr = System.IO.File.OpenText(PathToTemplate))
                {
                    HtmlBody = sr.ReadToEnd();
                }

                StringBuilder productListSB = new StringBuilder();
                foreach (var product in ProductUserVM.ProductList)
                {
                    productListSB.Append($" - Name: {product.Name} <span style='font-size:14px;'> (ID: {product.Id})</span><br />");
                }

                string messageBody = string.Format(HtmlBody,
                    ProductUserVM.ApplicationUser.FullName,
                    ProductUserVM.ApplicationUser.Email,
                    ProductUserVM.ApplicationUser.PhoneNumber,
                    productListSB.ToString());

                await _emailSender.SendEmailAsync(WC.EmailAdmin, subject, messageBody);

                InquiryHeader inquiryHeader = new InquiryHeader()
                {
                    ApplicationUserId = claim.Value,
                    FullName = ProductUserVM.ApplicationUser.FullName,
                    Email = ProductUserVM.ApplicationUser.Email,
                    PhoneNumber = ProductUserVM.ApplicationUser.PhoneNumber,
                    InquiryDate = DateTime.Now
                };

                _inqHRepo.Add(inquiryHeader);
                _inqHRepo.Save();

                foreach (var product in ProductUserVM.ProductList)
                {
                    InquiryDetail inquiryDetail = new InquiryDetail()
                    {
                        InquiryHeaderId = inquiryHeader.Id,
                        ProductId = product.Id,
                    };
                    _inqDRepo.Add(inquiryDetail);
                }
                _inqDRepo.Save();
            }


            return RedirectToAction(nameof(InquiryConfirmation));
        }

        public IActionResult InquiryConfirmation(int id = 0)
        {
            OrderHeader orderHeader = _orderHRepo.FirstOrDefault(oh => oh.Id == id);
            HttpContext.Session.Clear();
            return View(orderHeader);
        }

        public IActionResult Remove(int id)
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart) != null && HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart).Count > 0)
            {
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart);
            }

            shoppingCartList.Remove(shoppingCartList.FirstOrDefault(u => u.ProductId == id));

            HttpContext.Session.Set(WC.SessionCart, shoppingCartList);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateCart(IEnumerable<Product> productList)
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            foreach (Product product in productList)
            {
                shoppingCartList.Add(new ShoppingCart { ProductId = product.Id, SqFt = product.TempSqFt });
            }

            HttpContext.Session.Set(WC.SessionCart, shoppingCartList);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Clear()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
