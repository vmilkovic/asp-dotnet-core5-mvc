using Braintree;

namespace RockyUtility.Braintree
{
    public interface IBraintreeGate
    {
        IBraintreeGateway CreateGateway();

        IBraintreeGateway GetGateway();
    }
}
