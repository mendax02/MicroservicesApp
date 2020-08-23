using Catalog.API.Entities;
using Catalog.API.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Catalog.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CatalogController : ControllerBase
    {

        private readonly IProductRepository _repository;

        private readonly ILogger<CatalogController> _logger;

        public CatalogController(IProductRepository repository, ILogger<CatalogController> logger)
        {
            _repository = repository ?? throw new ArgumentException();
            _logger = logger ?? throw new ArgumentException();
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Product>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            IEnumerable<Product> products = await _repository.GetProducts();

            return Ok(products);
        }

        [HttpGet("{id:length(24)}", Name = "GetProduct")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Product>> GetProductById(string id)
        {
            Product product = await _repository.GetProduct(id);

            if (product == null)
            {
                _logger.LogError($"Product with {id}, not found.");
                return NotFound(id);
            }

            return Ok(product);
        }

        [Route("[action]/{productName}")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Product>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductsByName(string productName)
        {
            IEnumerable<Product> products = await _repository.GetProductsByName(productName);

            return Ok(products);
        }

        [Route("[action]/{catagoryName}")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Product>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductsByCatagory(string catagoryName)
        {
            IEnumerable<Product> products = await _repository.GetProductsByCatagory(catagoryName);

            return Ok(products);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Product>> CreateProduct([FromBody] Product product)
        {
            //  var result = await Task.FromResult(_repository.Create(product));

            await _repository.Create(product);

            return CreatedAtRoute("GetProduct", new { id = product.Id }, product);
        }

        [HttpPut]
        [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
        public async Task<ActionResult> UpdateProduct([FromBody] Product product)
        {
            return Ok(await _repository.Update(product));
        }


        [HttpDelete("{id:length(24)}")]
        [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
        public async Task<ActionResult> DeleteProductById(string id)
        {
            return Ok(await _repository.Delete(id));
        }
    }
}
