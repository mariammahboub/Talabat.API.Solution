using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
using Talabat.API.Dtos;
using Talabat.API.Errors;
using Talabat.API.Helpers;
using Talabat.Core.Entities;
using Talabat.Core.Repositories.Contract;
using Talabat.Core.Services.Contract;
using Talabat.Core.Specifications.Product_Spec;

namespace Talabat.API.Controllers
{
    public class ProductsController : ApiBaseController
    {
        #region Constructor For Create Object Form Generic Repository (Develop Against interface Not Concrete Class)
        ///private readonly IGenericRepository<Product> _productRepository;
        ///private readonly IGenericRepository<ProductBrand> _productBrandRepository;
        ///private readonly IGenericRepository<ProductCategory> _productCategoryRepository;
        private readonly IMapper _mapper;
        private readonly IProductService _productService;

        public ProductsController(
            ///IGenericRepository<ProductBrand> productBrandRepository,
            ///IGenericRepository<ProductCategory> productCategoryRepository,
            IMapper mapper,
            IProductService productService)
        {
            ///_productRepository = productRepository;
            ///_productBrandRepository = productBrandRepository;
            ///_productCategoryRepository = productCategoryRepository;
            _mapper = mapper;
            _productService = productService;
        }
        #endregion

        #region End Points

        #region Get All Products
        [CachedAttribute(600)]//Action Filter
        [HttpGet]// Get : /api/Products
        public async Task<ActionResult<Pagination<ProductToReturnDtos>>> GetProducts([FromQuery]ProductSpecParams productSpecParams)
        {
            var Products = await _productService.GetProductsAsync(productSpecParams); 

            var Count = await _productService.GetCountAsync(productSpecParams); // Query For Count all Data that Must return without Pagination 

            var mappedProducts = _mapper.Map <IReadOnlyList<Product> , IReadOnlyList <ProductToReturnDtos >> (Products);
            return Ok(new Pagination<ProductToReturnDtos>(productSpecParams.PageIndex , productSpecParams.PageSize ,Count , mappedProducts));
        }
        #endregion

        #region Get Product By Id
        [ProducesResponseType(typeof(ProductToReturnDtos) , StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.NotFound)]
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductToReturnDtos>> GetProduct(int id)
        {
            var Product = await _productService.GetProductAsync(id);
            if (Product == null)
                return NotFound(new ApiResponse(404));
            var mappedProduct = _mapper.Map<Product, ProductToReturnDtos>(Product);

            return Ok(mappedProduct);
        }
        #endregion


        #region Get All Brands
        [HttpGet("brands")]
        public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetBrands()
        {
            var Brands = await _productService.GetBrandsAsync();
            return Ok(Brands);
        }
        #endregion

        #region Get All Categories
        [HttpGet("categories")]
        public async Task<ActionResult<IReadOnlyList<ProductCategory>>> GetCategories()
        {
            var Categories = await _productService.GetCategoriesAsync();
            return Ok(Categories);
        }
        #endregion
        #endregion
    }
}
