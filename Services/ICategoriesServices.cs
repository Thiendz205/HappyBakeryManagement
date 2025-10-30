using HappyBakeryManagement.DTO;
using HappyBakeryManagement.Models;

namespace HappyBakeryManagement.Services
{
    public interface ICategoriesServices
    {
        List<CategoriesDTO>getCategories();
        void AddCategory(CategoriesDTO categoryDto);
        void Detele(int id);
        CategoriesDTO FindById(int id);
        bool ExistsByName(string name, int? excludeId = null);
        void UpdateCategory(Category category);

        List<CategoriesDTO> GetNameAndIDCategory();

    }
}
