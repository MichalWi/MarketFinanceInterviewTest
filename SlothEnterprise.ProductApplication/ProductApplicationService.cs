using SlothEnterprise.External.V1;
using SlothEnterprise.ProductApplication.Applications;
using SlothEnterprise.ProductApplication.Factories;
using SlothEnterprise.ProductApplication.Products;
using System;

namespace SlothEnterprise.ProductApplication
{
    public class ProductApplicationService : IProductApplicationService
    {
        private readonly ISelectInvoiceService _selectInvoiceService;
        private readonly IConfidentialInvoiceService _confidentialInvoiceWebService;
        private readonly IBusinessLoansService _businessLoansService;

        private const int FailedStatus = -1;

        public ProductApplicationService(
            ISelectInvoiceService selectInvoiceService,
            IConfidentialInvoiceService confidentialInvoiceWebService,
            IBusinessLoansService businessLoansService)
        {
            _selectInvoiceService = selectInvoiceService;
            _confidentialInvoiceWebService = confidentialInvoiceWebService;
            _businessLoansService = businessLoansService;
        }

        public int SubmitApplicationFor(ISellerApplication application)
        {
            if (application is null || application.CompanyData is null)
                throw new ArgumentNullException(nameof(application));

            if (application.Product is SelectiveInvoiceDiscount)
                return SubmitForSelectiveInvoiceDiscount(application);
            else if (application.Product is ConfidentialInvoiceDiscount)
                return SubmitForConfidentialInvoiceDiscount(application);
            else if (application.Product is BusinessLoans)
                return SubmitForBusinessLoans(application);
            else
                throw new InvalidOperationException();
        }

        private int SubmitForSelectiveInvoiceDiscount(ISellerApplication application)
        {
            var product = application.Product as SelectiveInvoiceDiscount;
            return _selectInvoiceService.SubmitApplicationFor(
                application.CompanyData.Number.ToString(),
                product.InvoiceAmount,
                product.AdvancePercentage);
        }

        private int SubmitForConfidentialInvoiceDiscount(ISellerApplication application)
        {
            var product = application.Product as ConfidentialInvoiceDiscount;
            var result = _confidentialInvoiceWebService.SubmitApplicationFor(
                CompanyDataRequestFactory.Create(application.CompanyData),
                product.TotalLedgerNetworth,
                product.AdvancePercentage,
                product.VatRate);

            return result.ApplicationId ?? FailedStatus;
        }

        private int SubmitForBusinessLoans(ISellerApplication application)
        {
            var result = _businessLoansService.SubmitApplicationFor(
                CompanyDataRequestFactory.Create(application.CompanyData),
                LoansRequestFactory.Create(application.Product as BusinessLoans));

            return result.ApplicationId ?? FailedStatus;
        }
    }
}