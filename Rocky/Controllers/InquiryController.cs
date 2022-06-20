using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RockyDataAccess.Data.Repository.IRepository;
using RockyModels;
using RockyModels.Models;
using RockyModels.Models.ViewModels;
using RockyUtility;
using System.Collections.Generic;

namespace Rocky.Controllers
{
    [Authorize(Roles = WC.AdminRole)]
    public class InquiryController : Controller
    {
        private readonly IInquiryHeaderRepository _inqHRepo;
        private readonly IInquiryDetailRepository _inqDRepo;

        [BindProperty]
        public InquiryViewModel InquiryViewModel { get; set; }

        public InquiryController(IInquiryHeaderRepository inqHRepo, IInquiryDetailRepository inqDRepo)
        {
            _inqHRepo = inqHRepo;
            _inqDRepo = inqDRepo;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(int id)
        {
            InquiryViewModel = new InquiryViewModel()
            {
                InquiryHeader = _inqHRepo.FirstOrDefault(inq => inq.Id == id),
                InquiryDetail = _inqDRepo.GetAll(inq => inq.InquiryHeaderId == id, includeProperties: "Product")
            };
            return View(InquiryViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Details()
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            InquiryViewModel.InquiryDetail = _inqDRepo.GetAll(inq => inq.InquiryHeaderId == InquiryViewModel.InquiryHeader.Id);

            foreach (var detail in InquiryViewModel.InquiryDetail)
            {
                ShoppingCart shoppingCart = new ShoppingCart()
                {
                    ProductId = detail.ProductId,
                };
                shoppingCartList.Add(shoppingCart);
            }

            HttpContext.Session.Clear();
            HttpContext.Session.Set(WC.SessionCart, shoppingCartList);
            HttpContext.Session.Set(WC.SessionInquiryId, InquiryViewModel.InquiryHeader.Id);

            return RedirectToAction("Index", "Cart");
        }

        [HttpPost]
        public IActionResult Delete()
        {
            InquiryHeader inquiryHeader = _inqHRepo.FirstOrDefault(inq => inq.Id == InquiryViewModel.InquiryHeader.Id);
            IEnumerable<InquiryDetail> inquiryDetails = _inqDRepo.GetAll(inq => inq.InquiryHeaderId == InquiryViewModel.InquiryHeader.Id);

            _inqDRepo.RemoveRange(inquiryDetails);
            _inqHRepo.Remove(inquiryHeader);
            _inqHRepo.Save();

            TempData[WC.Success] = "Action completed successfully";

            return RedirectToAction(nameof(Index));
        }

        #region API CALLS

        [HttpGet]
        public IActionResult GetInquiryList()
        {
            return Json(new { data = _inqHRepo.GetAll() });
        }

        #endregion
    }
}
