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

        #region Create

        [Fact]
        public void Create_Get_ReturnsView()
        {
            var actionResult = _controller.Create();
            Assert.IsType<ViewResult>(actionResult);
        }

        [Fact]
        public async void Create_Post_InvalidModelState_ReturnsViewWithProduct()
        {
            var product = new Product { Id = 1, Name = null, Color = "red", Price = 12, Stock = 33 };
            _controller.ModelState.AddModelError("Name", "The Name field required.");

            var actionResult = await _controller.Create(product);
            var viewResult = Assert.IsType<ViewResult>(actionResult);

            Assert.IsType<Product>(viewResult.Model);
            Assert.False(viewResult.ViewData.ModelState.IsValid);
        }

        [Fact]
        public async void Create_Post_ValidModelState_RedirectToIndexAction()
        {
            var actionResult = await _controller.Create(products.First());
            var redirectResult = Assert.IsType<RedirectToActionResult>(actionResult);

            Assert.Equal("Index", redirectResult.ActionName);

        }

        [Fact]
        public async void Create_Post_ValidModelState_CreateMethodExecutes()
        {
            Product newProduct = null;
            _repository.Setup(x => x.CreateAsync(It.IsAny<Product>())).Callback<Product>(x => newProduct = x);

            var result = await _controller.Create(products.First());

            _repository.Verify(x => x.CreateAsync(It.IsAny<Product>()), Times.Once());

            Assert.Equal(products.First().Id, newProduct.Id);
        }

        [Fact]
        public async void Create_Post_InvalidModelState_NeverCreatePostExecutes()
        {
            _controller.ModelState.AddModelError("Name", "Name error");

            var result = await _controller.Create(products.First());

            _repository.Verify(x => x.CreateAsync(It.IsAny<Product>()), Times.Never);
        }

        #endregion

        #region Edit

        [Fact]
        public async void Edit_IdIsNull_ReturnsRedirectToIndexAction()
        {
            var actionResult = await _controller.Edit(null);
            var result = Assert.IsType<RedirectToActionResult>(actionResult);

            Assert.Equal("Index", result.ActionName);
        }

        [Theory]
        [InlineData(0)]
        public async void Edit_ProductIsNull_ReturnsNotFound(int productId)
        {
            Product product = null;
            _repository.Setup(x => x.GetByIdAsync(productId)).ReturnsAsync(product);
            var actionResult = await _controller.Edit(productId);

            var result = Assert.IsType<NotFoundResult>(actionResult);

            Assert.Equal(404, result.StatusCode);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public async void Edit_IdIsValid_ReturnsViewWithProduct(int productId)
        {
            var product = products.Find(x => x.Id == productId);
            _repository.Setup(x => x.GetByIdAsync(productId)).ReturnsAsync(product);

            var actionResult = await _controller.Edit(productId);

            var viewResult = Assert.IsType<ViewResult>(actionResult);

            var productResult = Assert.IsAssignableFrom<Product>(viewResult.Model);


            Assert.Equal(product.Id, productResult.Id);
            Assert.IsType<Product>(viewResult.Model);
        }

        [Theory]
        [InlineData(1)]
        public void Edit_Post_IdIsNotEqualProductId_ReturnNotFound(int id)
        {
            int invalidId = 99;
            var result = _controller.Edit(invalidId, products.First(x => x.Id == id));

            var redirect = Assert.IsType<NotFoundResult>(result);
        }

        [Theory]
        [InlineData(1)]
        public void Edit_Post_InvalidModelState_ReturnsViewWithGivenProduct(int id)
        {
            var updateProduct = products.First(x => x.Id == id);
            _controller.ModelState.AddModelError("Name", "Name is required.");

            var actionResult = _controller.Edit(id, updateProduct);

            var viewResult = Assert.IsType<ViewResult>(actionResult);
            var product = Assert.IsType<Product>(viewResult.Model);
            Assert.Equal(updateProduct.Id, product.Id);
        }

        [Theory]
        [InlineData(1)]
        public void Edit_Post_ValidModelState_RedirectToIndexAction(int id)
        {
            var actionResult = _controller.Edit(id, products.First(x => x.Id == id));

            var redirectResult = Assert.IsType<RedirectToActionResult>(actionResult);

            Assert.Equal("Index", redirectResult.ActionName);
        }

        [Theory]
        [InlineData(1)]
        public void Edit_Post_ValidModelState_UpdateMethodExecutes(int id)
        {
            var updateProduct = products.First(x => x.Id == id);
            _repository.Setup(x => x.Update(updateProduct));

            var actionResult = _controller.Edit(id, updateProduct);

            _repository.Verify(x => x.Update(It.IsAny<Product>()), Times.Once());
        }

        #endregion
    }
}
