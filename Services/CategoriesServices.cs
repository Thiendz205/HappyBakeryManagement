using HappyBakeryManagement.Data;
using HappyBakeryManagement.DTO;

namespace HappyBakeryManagement.Services
{
    public class CategoriesServices:ICategoriesServices
    {
        public readonly ApplicationDbContext _db;
        public CategoriesServices(ApplicationDbContext db)
        {
            _db = db;
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
