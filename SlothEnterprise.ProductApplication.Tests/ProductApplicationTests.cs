using FluentAssertions;
using Moq;
using SlothEnterprise.External;
using SlothEnterprise.External.V1;
using SlothEnterprise.ProductApplication.Applications;
using SlothEnterprise.ProductApplication.Products;
using System;
using Xunit;

namespace SlothEnterprise.ProductApplication.Tests
{
    public class ProductApplicationTests
    {
        private readonly Mock<IConfidentialInvoiceService> _confidentialInvoiceServiceMock;
        private readonly Mock<IBusinessLoansService> _bussinesLoansService;
        private readonly Mock<ISelectInvoiceService> _selectInvoiceService;

        public ProductApplicationTests()
        {
            _confidentialInvoiceServiceMock = new Mock<IConfidentialInvoiceService>();
            _bussinesLoansService = new Mock<IBusinessLoansService>();
            _selectInvoiceService = new Mock<ISelectInvoiceService>();
        }

        [Fact]
        public void ProductApplicationService_SubmitApplicationFor_WhenCalledWithConfidentialInvoiceDiscount_ShouldReturnOne()
        {
            var expectedResult = 1;
            int? applicationId = 1;
            bool isSuccess = true;
            var (sut, application) = SetupServiceWithApplication(
                new ConfidentialInvoiceDiscount(),
                applicationId,
                isSuccess);

            var result = sut.SubmitApplicationFor(application);

            result.Should().Be(expectedResult);
        }
        
        [Fact]
        public void ProductApplicationService_SubmitApplicationFor_WhenCalledWithBusinessLoans_ShouldReturnMinusOne()
        {
            var expectedResult = -1;
            int? applicationId = null;
            bool isSuccess = false;
            var (sut, application) = SetupServiceWithApplication(
                new BusinessLoans(),
                applicationId,
                isSuccess);

            var result = sut.SubmitApplicationFor(application);

            result.Should().Be(expectedResult);
        }

        [Fact]
        public void ProductApplicationService_SubmitApplicationFor_WhenCalledWithNull_ShouldThrowsInvalidOperationException()
        {
            int? applicationId = null;
            bool isSuccess = false;
            var (sut, application) = SetupServiceWithApplication(
                null,
                applicationId,
                isSuccess);

            Assert.Throws<InvalidOperationException>(() => sut.SubmitApplicationFor(application));
        }

        private (IProductApplicationService ApplicationService, ISellerApplication SellerApplication) SetupServiceWithApplication(
            IProduct product,
            int? applicationId,
            bool isSuccess)
        {
            SetupExternalServices(applicationId, isSuccess);
            var sut = new ProductApplicationService(
                _selectInvoiceService.Object,
                _confidentialInvoiceServiceMock.Object,
                _bussinesLoansService.Object);

            return (sut, SetupSellerApplication(product));
        }

        private ISellerApplication SetupSellerApplication(IProduct product)
        {
            var sellerApplicationMock = new Mock<ISellerApplication>();
            sellerApplicationMock.SetupProperty(p => p.Product, product);
            sellerApplicationMock.SetupProperty(p => p.CompanyData, new SellerCompanyData());

            return sellerApplicationMock.Object;
        }

        private void SetupExternalServices(int? applicationId, bool isSuccess)
        {
            var resultMock = new Mock<IApplicationResult>();
            resultMock.SetupProperty(p => p.ApplicationId, applicationId);
            resultMock.SetupProperty(p => p.Success, isSuccess);

            _confidentialInvoiceServiceMock
                .Setup(m => m.SubmitApplicationFor(
                    It.IsAny<CompanyDataRequest>(),
                    It.IsAny<decimal>(),
                    It.IsAny<decimal>(),
                    It.IsAny<decimal>()))
                .Returns(resultMock.Object);

            _selectInvoiceService
                .Setup(m => m.SubmitApplicationFor(
                    It.IsAny<string>(),
                    It.IsAny<decimal>(),
                    It.IsAny<decimal>()))
                .Returns(resultMock.Object.ApplicationId.GetValueOrDefault());

            _bussinesLoansService
                .Setup(m => m.SubmitApplicationFor(
                    It.IsAny<CompanyDataRequest>(),
                    It.IsAny<LoansRequest>()))
                .Returns(resultMock.Object);
        }
    }
}