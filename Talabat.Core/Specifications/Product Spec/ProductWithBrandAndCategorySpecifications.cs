using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;

namespace Talabat.Core.Specifications.Product_Spec
{
    public class ProductWithBrandAndCategorySpecifications : BaseSpecifications<Product>
    {
        public ProductWithBrandAndCategorySpecifications(ProductSpecParams productSpecParams) :
            base(P =>
                    //(brandId.HasValue ? P.BrandId == brandId.Value : true) &&
                    //(categoryId.HasValue ? P.CategoryId == categoryId.Value : true)
                    (string.IsNullOrEmpty(productSpecParams.Search) || P.Name.ToLower().Contains(productSpecParams.Search))&&
                    (!productSpecParams.BrandId.HasValue      || P.BrandId == productSpecParams.BrandId.Value) &&
                    (!productSpecParams.CategoryId.HasValue || P.CategoryId == productSpecParams.CategoryId.Value)
            )
        {
            AddIncludes();
            if (!string.IsNullOrEmpty(productSpecParams.Sort)) 
            {
                switch (productSpecParams.Sort)
                {
                    case "priceAsc":
                        AddOrderBy(P => P.Price);
                        break;
                    case "priceDesc":
                        AddOrderByDesc(P => P.Price);
                        break;
                    default:
                        AddOrderBy(P => P.Name);
                        break;
                }
            }
            else
                AddOrderBy(P => P.Name);

            Applypagenation((productSpecParams.PageIndex - 1) * productSpecParams.PageSize , productSpecParams.PageSize );
                

        }

        public ProductWithBrandAndCategorySpecifications(int id) : base(p => p.Id == id)
        {
            AddIncludes();
        }

        private void AddIncludes()
        {
            Includes.Add(P => P.Brand);
            Includes.Add(P => P.Category);
        }

    }
}
