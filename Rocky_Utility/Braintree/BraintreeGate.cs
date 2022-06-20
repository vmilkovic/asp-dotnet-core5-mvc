using Braintree;
using Microsoft.Extensions.Options;

namespace RockyUtility.Braintree
{
    public class BraintreeGate : IBraintreeGate
    {
        public BraintreeSettings _options { get; set; }
        private IBraintreeGateway braintreeGateway { get; set; }

        public BraintreeGate(IOptions<BraintreeSettings> options)
        {
            _options = options.Value;
        }

        public IBraintreeGateway CreateGateway()
        {
            return new BraintreeGateway(_options.Environment, _options.MerchantId, _options.PublicKey, _options.PrivateKey);
        }

        public IBraintreeGateway GetGateway()
        {
            if (braintreeGateway == null)
            {
                braintreeGateway = CreateGateway();
            }
            return braintreeGateway;
        }
    }
}
