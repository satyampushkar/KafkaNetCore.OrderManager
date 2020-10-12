using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OrderAPI.EventProcesser.Interface;
using OrderAPI.Models;

namespace OrderAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IEventProducer _producer;
        private readonly ILogger<OrderController> _logger;
        public OrderController(IEventProducer producer, ILogger<OrderController> logger)
        {
            _logger = logger;
            _producer = producer;
        }
        // POST api/values
        [HttpPost]
        public async Task<ActionResult> PostAsync([FromBody] OrderRequest value)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //Serialize 
            string serializedOrder = JsonConvert.SerializeObject(value);
            await _producer.WriteMessage(serializedOrder, "orderrequests");

            return Created(new Guid().ToString(), OrderStatus.IN_PROGRESS);
        }
    }
}
