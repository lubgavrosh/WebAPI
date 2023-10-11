using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.Models.Category;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly AppEFContext _context;
        public CategoriesController(IMapper mapper, AppEFContext context)
        {
            _mapper = mapper;
            _context = context;
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
    }
}
