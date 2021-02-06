using SlothEnterprise.External;
using SlothEnterprise.ProductApplication.Products;

namespace SlothEnterprise.ProductApplication.Factories
{
    public static class LoansRequestFactory
    {
        public static LoansRequest Create(BusinessLoans loans) =>
            new LoansRequest
            {
                InterestRatePerAnnum = loans.InterestRatePerAnnum,
                LoanAmount = loans.LoanAmount,
            };
    }
}
