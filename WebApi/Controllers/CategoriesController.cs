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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using WebStore.Data.Entitties.Identity;
using WebStore.Business_logic.Category;

namespace WebStore.Controllers
{
    [ApiController, Authorize, Route("[controller]")]
    public class CategoriesController : ControllerBase
    {
        private ICategoryService Service { get; }
        public CategoriesController(ICategoryService service)
        {
            Service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        // It would be great to make ranged response here
        {
            try
            {
                var cats = await Service.GetAll(User);
                return Ok(cats);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            try
            {
                CategoryModel cat = await Service.GetById(id, User);
                return Ok(cat);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromForm] CategoryCreateModel model)
        {
            CategoryModel? cat = await Service.Create(model, User);
            return cat is not null
                ? Ok(cat)
                : BadRequest(new { message = "Failed to create category." });
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update([FromForm] CategoryUpdateModel model)
        {
            CategoryModel? updated = await Service.Update(model, User);
            return updated is not null
                ? Ok(updated)
                : BadRequest(new { message = "Failed to update category." });
        }

        [HttpDelete("delete/{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            bool result = await Service.Delete(id, User);
            return result
                ? Ok()
                : BadRequest(new { message = "Failed to delete category." });
        }
    }
}