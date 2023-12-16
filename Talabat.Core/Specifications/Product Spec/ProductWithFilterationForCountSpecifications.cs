using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;

namespace Talabat.Core.Specifications.Product_Spec
{
    public class ProductWithFilterationForCountSpecifications : BaseSpecifications<Product>
    {
        public ProductWithFilterationForCountSpecifications(ProductSpecParams productSpecParams) 
            : base( P =>
                  (string.IsNullOrEmpty(productSpecParams.Search) || P.Name.ToLower().Contains(productSpecParams.Search))&&
                  (!productSpecParams.BrandId.HasValue    || P.BrandId == productSpecParams.BrandId) &&
                  (!productSpecParams.CategoryId.HasValue || P.CategoryId == productSpecParams.CategoryId)
            )
        {
            
        }
    }
}
