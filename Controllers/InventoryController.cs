using System;
using InventoryPrototype.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace InventoryPrototype.Controllers {
  [Route("api/[controller]")]
  public class InventoryController : Controller {

    private readonly InventoryDbContext _context;

    public InventoryController(InventoryDbContext context) {
      _context = context;
    }

    // GET: api/<controller>
    [HttpGet]
    /* original scaffolding
    public IEnumerable<string> Get() {
      return new string[] { "value1", "value2" };
    }
    */
    public ApiResponse<IEnumerable<InventoryItem>> GetAll() =>
     new ApiResponse<IEnumerable<InventoryItem>> { Data = _context.Items.ToList() };

    // GET api/<controller>/5
    [HttpGet("{id}")]
    public string Get(int id) {
      return "value";
    }

    // POST api/<controller>
    [HttpPost]
    public ApiResponse<IEnumerable<InventoryItem>> AddUpdate([FromBody]InventoryItem item)
    {
      item.LastUpdated = DateTime.Now;
      var existing = _context.Find<InventoryItem>(new object[] {item.ProductId});
      if (existing == null) _context.Add(item);
      else existing.Quantity = item.Quantity;
      _context.SaveChanges();
      return GetAll();
    }

    // PUT api/<controller>/5
    [HttpPut("{id}")]
    public void Put(int id, [FromBody]string value) {
    }

    // DELETE api/<controller>/5
    [HttpDelete("{id}")]
    public void Delete(int id) {
    }
  }
}
