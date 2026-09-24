using AndroidWebAPI.Data;
using AndroidWebAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace AndroidWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VaccineInventoryController : ControllerBase
    {
        private readonly VaccineInventoryRepository _repository;

        public VaccineInventoryController(VaccineInventoryRepository repository)
        {
            _repository = repository;
        }

        // ========================================
        // GET ALL
        // GET: api/VaccineInventory
        // ========================================
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var inventory = await _repository.GetAllAsync();
            return Ok(inventory);
        }

        // ========================================
        // GET BY ID
        // GET: api/VaccineInventory/1
        // ========================================
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var inventory = await _repository.GetByIdAsync(id);

            if (inventory == null)
                return NotFound();

            return Ok(inventory);
        }

        // ========================================
        // GET BY VACCINE
        // GET: api/VaccineInventory/vaccine/1
        // ========================================
        [HttpGet("vaccine/{vaccineId}")]
        public async Task<IActionResult> GetByVaccine(int vaccineId)
        {
            var inventory = await _repository.GetByVaccineIdAsync(vaccineId);
            return Ok(inventory);
        }

        // ========================================
        // CREATE
        // POST: api/VaccineInventory
        // ========================================
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] VaccineInventory inventory)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _repository.CreateAsync(inventory);

            return CreatedAtAction(
                nameof(Get),
                new { id = created.InventoryID },
                created);
        }

        // ========================================
        // UPDATE
        // PUT: api/VaccineInventory/1
        // ========================================
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] VaccineInventory inventory)
        {
            if (id != inventory.InventoryID)
                return BadRequest("ID mismatch.");

            var existing = await _repository.GetByIdAsync(id);

            if (existing == null)
                return NotFound();

            var updated = await _repository.UpdateAsync(inventory);

            return Ok(updated);
        }

        // ========================================
        // DELETE
        // DELETE: api/VaccineInventory/1
        // ========================================
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _repository.DeleteAsync(id);

            if (!success)
                return NotFound();

            return Ok(new
            {
                message = "Inventory record deleted successfully."
            });
        }
    }
}