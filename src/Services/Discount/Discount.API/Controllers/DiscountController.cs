using Discount.API.Entities;
using Discount.API.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discount.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class DiscountController : ControllerBase
    {
        private readonly IDiscountRepository _repository;

        public DiscountController(IDiscountRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        [HttpGet("{productName}", Name = "GetDiscount")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Coupon))]
        public async Task<ActionResult<Coupon>> GetDiscount(string productName)
        {
            var discount = await _repository.GetDiscount(productName);
            return Ok(discount);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Coupon))]
        public async Task<IActionResult> CreateDiscount([FromBody] Coupon coupon)
        {
            await _repository.CreateDiscount(coupon);
            return CreatedAtRoute("GetDiscount", new { productName = coupon.ProductName }, coupon);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Coupon))]
        public async Task<IActionResult> UpdateDiscount([FromBody] Coupon coupon)
        {
            return Ok(await _repository.UpdateDiscount(coupon));
        }

        [HttpDelete("{productName}", Name = "DeleteDiscount")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Coupon))]
        public async Task<IActionResult> DeleteDiscount(string productName)
        {
            return Ok(await _repository.DeleteDiscount(productName));
        }
    }
}
