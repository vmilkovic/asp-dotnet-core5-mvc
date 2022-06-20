using System.Collections.Generic;

namespace RockyModels.Models.ViewModels
{
    public class InquiryViewModel
    {
        public InquiryHeader InquiryHeader { get; set; }
        public IEnumerable<InquiryDetail> InquiryDetail { get; set; }
    }
}
