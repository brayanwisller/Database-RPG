using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RpgApi.Data;
using RpgApi.Models;

namespace RpgApi.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ItemsController : ControllerBase{
    private readonly AppDbContext _db;

    public ItemsController(AppDbContext db) => _db = db;

    // GET: /api/v1/items
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Item>>> GetAll()
        => Ok(await _db.Items.OrderBy(s=>s.Id).ToListAsync());

    // GET: /api/v1/items/1
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Item>> GetById(int id)
        => await _db.Items.FindAsync(id) is { } s ? Ok(s) : NotFound();
        

    // POST: /api/v1/items
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Item item){
        if (!ModelState.IsValid){
            return UnprocessableEntity(ModelState);
        }

        if (await _db.Items.AnyAsync(x => x.Name == item.Name)){
            return Conflict(new {error = "Um item com este nome já foi cadastrado."});
        }

        _db.Items.Add(item);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
    }

    // PUT: /api/v1/items/1
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] Item item){
        item.Id = id;
        
        if (!ModelState.IsValid){
            return BadRequest(ModelState);
        }
            
        var existingItem = await _db.Items.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        if (existingItem is null){
            return NotFound(new { message = "Item não encontrado para atualização."});
        }

        if (await _db.Items.AnyAsync(x => x.Name == item.Name && x.Id != id)){
            return Conflict(new { error = "O nome informado já está em uso por outro item."});
        }
        _db.Entry(item).State = EntityState.Modified;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: /api/v1/items/1
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id){
        var item = await _db.Items.FindAsync(id);
        if (item is null){
            return NotFound(new { message = "Item não encontrado para exclusão." });
        }

        _db.Items.Remove(item);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}