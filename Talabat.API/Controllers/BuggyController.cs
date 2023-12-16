using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.API.Errors;
using Talabat.Repository.Data;

namespace Talabat.API.Controllers
{
    public class BuggyController : ApiBaseController
    {
        private readonly StoreDbContext _dbContext;

        public BuggyController(StoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("notfound")]  //Get : api/buggy/notfound  =>   [NotFound Error]
        public ActionResult GetNotFound()
        {
            var product = _dbContext.Products.Find(10000);
            if (product == null) { return NotFound(new ApiResponse(404)); }
            return Ok(product);
        }

        [HttpGet("UnAuthorize")]  //Get : api/buggy/UnAuthorize  =>   [UnAuthorize Error]
        public ActionResult GetUnAuthorize()
        {
            return Unauthorized(new ApiResponse(401));
        }

        [HttpGet("servererror")]  //Get : api/buggy/servererror  =>   [ServerError Error]
        public ActionResult GetServerError()
        {
            var product = _dbContext.Products.Find(10000);
            var productToReturn = product.ToString(); //will throw Null Reference Exception
            return Ok(productToReturn);
        }

        [HttpGet("badrequest")]  //Get : api/buggy/badrequest  =>   [badrequest Error]
        public ActionResult GetBadRequest()
        {
            return BadRequest(new ApiResponse(400));
        }

        [HttpGet("badrequest/{id}")]  //Get : api/buggy/badrequest/fivew  =>   [Validation Error]
        public ActionResult GetBadRequest(int id)
        {
            return Ok();
        }

    }
}
