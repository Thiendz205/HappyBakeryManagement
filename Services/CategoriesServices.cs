using HappyBakeryManagement.Data;
using HappyBakeryManagement.DTO;
using HappyBakeryManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace HappyBakeryManagement.Services
{
    public class CategoriesServices : ICategoriesServices
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

        public void Detele(int id)
        {
            var category = _db.Categories.Find(id);
            if (category != null)
            {
                _db.Categories.Remove(category);
                _db.SaveChanges();
            }

        }

        public bool ExistsByName(string name, int? excludeId = null)
        {
            var query = _db.Categories.AsQueryable();

            if (excludeId.HasValue)
            {
                query = query.Where(c => c.Id != excludeId.Value);
            }

            return query.Any(c => c.Name.ToLower() == name.ToLower());
        }


        public CategoriesDTO FindById(int id)
        {
            var category = _db.Categories.Find(id);
            if (category == null) return null;

            return new CategoriesDTO
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description
            };
        }

        public void UpdateCategory(Category category)
        {
            _db.Categories.Update(category);
            _db.SaveChanges();

        }

        public List<CategoriesDTO> GetNameAndIDCategory()
        {
            return _db.Categories
                      .Select(c => new CategoriesDTO
                      {
                          Id = c.Id,
                          Name = c.Name
                      })
                      .ToList();
        }
    }
}
