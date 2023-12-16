
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core;
using Talabat.Core.Entities;
using Talabat.Core.Services.Contract;
using Talabat.Core.Specifications.Product_Spec;

namespace Talabat.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IReadOnlyList<ProductBrand>> GetBrandsAsync()
        {
            var Brands = await _unitOfWork.Repository<ProductBrand>().GetAllAsync();
            return Brands;
        }

        public async Task<IReadOnlyList<ProductCategory>> GetCategoriesAsync()
        {
            var Categories = await _unitOfWork.Repository<ProductCategory>().GetAllAsync();
            return Categories;
        }

        public async Task<int> GetCountAsync(ProductSpecParams specParams)
        {
            var countSpec = new ProductWithFilterationForCountSpecifications(specParams);
            var Count = await _unitOfWork.Repository<Product>().GetCountAsync(countSpec);
            return Count;
        }

        public async Task<Product?> GetProductAsync(int productId)
        {
            var spec = new ProductWithBrandAndCategorySpecifications(productId);
            var Product = await _unitOfWork.Repository<Product>().GetWithSpecAsync(spec);
            return Product;
        }

        public async Task<IReadOnlyList<Product>> GetProductsAsync(ProductSpecParams productSpecParams)
        {
            var spec = new ProductWithBrandAndCategorySpecifications(productSpecParams);
            var Products = await _unitOfWork.Repository<Product>().GetAllWithSpecAsync(spec);
            return Products;
        }
    }
}
