using Microsoft.AspNetCore.Mvc;
using Moq;
using NetCoreMvcUnitTest.MVC.Controllers;
using NetCoreMvcUnitTest.MVC.Data;
using NetCoreMvcUnitTest.MVC.Models;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace NetCoreMvcUnitTest.Test.UnitTests
{
    public class ProductsControllerTests
    {
        private readonly Mock<IRepository<Product>> _repository;
        private readonly ProductsController _controller;
        private List<Product> products;

        public ProductsControllerTests()
        {
            _repository = new Mock<IRepository<Product>>();
            _controller = new ProductsController(_repository.Object);

            products = new List<Product>()
            {
                new Product() { Id = 1, Name = "Test", Price = 100, Color = "Red", Stock = 1050},
                new Product() { Id = 2, Name = "Test 2", Price = 56, Color = "White", Stock = 450}
            };
        }

        #region Index
        [Fact]
        public async void Index_ActionExecutes_ReturnView()
        {
            var result = await _controller.Index();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async void Index_ActionExecutes_ReturnsProductList()
        {
            _repository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(products);

            var result = await _controller.Index();

            var viewResult = Assert.IsType<ViewResult>(result);

            var productList = Assert.IsAssignableFrom<IEnumerable<Product>>(viewResult.Model);

            Assert.Equal<int>(2, productList.Count());
        }
        #endregion

        #region Details

        [Fact]
        public async void Details_IdIsNull_ReturnsRedirectToIndexAction()
        {
            var action = await _controller.Details(null);
            var result = Assert.IsType<RedirectToActionResult>(action);

            Assert.Equal("Index", result.ActionName);
        }

        [Fact]
        public async void Details_IdIsInvalid_ReturnsNotFound()
        {
            int id = 0;
            Product tempProduct = null;
            _repository.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(tempProduct);

            var action = await _controller.Details(id);
            var result = Assert.IsType<NotFoundResult>(action);

            Assert.Equal(404, result.StatusCode);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public async void Details_IdIsValid_ReturnsViewResultWithProduct(int productId)
        {
            var product = products.First(x => x.Id == productId);
            _repository.Setup(x => x.GetByIdAsync(productId)).ReturnsAsync(product);

            var action = await _controller.Details(productId);

            var viewResult = Assert.IsType<ViewResult>(action);

            var resultProduct = Assert.IsAssignableFrom<Product>(viewResult.Model);

            Assert.Equal(product.Id, resultProduct.Id);
            Assert.Equal(product.Name, resultProduct.Name);
        }

        #endregion


    }
}
