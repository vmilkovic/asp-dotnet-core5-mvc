namespace RockyModels.ViewModels
{
    public class DetailsViewModel
    {
        public DetailsViewModel()
        {
            Product = new Product();
        }

        public Product Product { get; set; }

        public bool ExistsInCart { get; set; }
    }
}
