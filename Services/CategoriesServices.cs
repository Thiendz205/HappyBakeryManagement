using HappyBakeryManagement.Data;
using HappyBakeryManagement.DTO;
using HappyBakeryManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace HappyBakeryManagement.Services
{
    public class CategoriesServices:ICategoriesServices
    {
        public readonly ApplicationDbContext _db;
        public CategoriesServices(ApplicationDbContext db)
        {
            _db = db;
        }

        public void AddCategory(CategoriesDTO categoryDto)
        {
            var exists = _db.Categories.Any(c => c.Name.ToLower() == categoryDto.Name.ToLower());
            if (exists)
            {
                throw new Exception("Tên danh mục đã tồn tại!");
            }

            var category = new Category
            {
                Name = categoryDto.Name,
                Description = categoryDto.Description
            };

            _db.Categories.Add(category);
            _db.SaveChanges();
        }


        public List<CategoriesDTO> getCategories()
        {
            var categories = from c in _db.Categories
                             select new CategoriesDTO
                             {
                                 Id = c.Id,
                                 Name = c.Name,
                                 Description = c.Description
                             };
            return categories.ToList();
        }
    }
}
