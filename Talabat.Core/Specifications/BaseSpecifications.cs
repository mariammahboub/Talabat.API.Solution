using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;

namespace Talabat.Core.Specifications
{
    public class BaseSpecifications<T> : ISpecifications<T> where T : BaseEntity
    {
        #region Properties
        public Expression<Func<T, bool>> Criteria { get; set; }  // = null
        public List<Expression<Func<T, object>>> Includes { get; set; } = new List<Expression<Func<T, object>>>();
        public Expression<Func<T, object>> OrderBy { get; set; } // = null
        public Expression<Func<T, object>> OrderByDesc { get; set; } // = null
        public int Skip { get; set; }
        public int Take { get; set; }
        public bool IsPagenationEnabled { get; set; }
        #endregion

        #region Constructors
        public BaseSpecifications()
        {
            // before execute => initialize for Criteria = null , Includes = Empty List
        }
        public BaseSpecifications(Expression<Func<T, bool>> CriteriaExpression)
        {
            Criteria = CriteriaExpression;
        }
        #endregion

        #region Methods
        public void AddOrderBy(Expression<Func<T, object>> orderByExpression)
        {
            OrderBy = orderByExpression;
        }
        public void AddOrderByDesc(Expression<Func<T, object>> orderByDescExpression)
        {
            OrderByDesc = orderByDescExpression;
        }
        public void Applypagenation(int skip, int take)
        {
            IsPagenationEnabled = true;
            Skip = skip;
            Take = take;
        } 
        #endregion
    }
}
