using SlothEnterprise.External;
using SlothEnterprise.ProductApplication.Applications;

namespace SlothEnterprise.ProductApplication.Factories
{
    public static class CompanyDataRequestFactory
    {
        public static CompanyDataRequest Create(ISellerCompanyData data) =>
            new CompanyDataRequest
            {
                CompanyFounded = data.Founded,
                CompanyName = data.Name,
                CompanyNumber = data.Number,
                DirectorName = data.DirectorName,
            };
    }
}
