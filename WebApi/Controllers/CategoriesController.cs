using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebStore.Data.Entitties;
using WebStore.Data;
using WebStore.Models.Category;
using WebStore.Data;
using WebStore.Data.Entitties;
using WebStore.Models.Category;

namespace WebStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly AppEFContext _context;
        public CategoriesController(IMapper mapper, AppEFContext context)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet("list")]
        public async Task<IActionResult> Index()
        {
            var model = await _context.Categories
               .Where(x => x.IsDeleted == false)
               .Select(x => _mapper.Map<CategoryItemViewModel>(x))
               .ToListAsync();

            return Ok(model);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var cat = await _context.Categories
               .Where(x => x.IsDeleted == false)
               //.Select(x => _mapper.Map<CategoryItemViewModel>(x))
               .SingleOrDefaultAsync(x => x.Id == id);

            if (cat == null)
                return NotFound();

            var model = _mapper.Map<CategoryItemViewModel>(cat);
            return Ok(model);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromForm] CategoryCreateViewModel model)
        {
            var requst = this.Request;
            try
            {
                var cat = _mapper.Map<CategoryEntity>(model);
                await _context.Categories.AddAsync(cat);
                await _context.SaveChangesAsync();
                return Ok(_mapper.Map<CategoryItemViewModel>(cat));
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> Put([FromBody] CategoryUpdateViewModel dto)
        {
            var cat = await _context.Categories
                .Where(x => x.IsDeleted == false)
                .SingleOrDefaultAsync(x => x.Id == dto.Id);
            if (cat == null)
                return NotFound();
            cat.Name = dto.Name;
            cat.Description = dto.Description;
            cat.Image = dto.Image;
            await _context.SaveChangesAsync();
            return Ok(_mapper.Map<CategoryItemViewModel>(cat));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Dekete(int id)
        {
            var cat = await _context.Categories
                .Where(x => x.IsDeleted == false)
                .SingleOrDefaultAsync(x => x.Id == id);
            if (cat == null)
                return NotFound();
            cat.IsDeleted = true;
            await _context.SaveChangesAsync();
            return Ok();
        }

    }
}