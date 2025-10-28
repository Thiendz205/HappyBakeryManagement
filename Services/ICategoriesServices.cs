using HappyBakeryManagement.DTO;

namespace HappyBakeryManagement.Services
{
    public interface ICategoriesServices
    {
        List<CategoriesDTO>getCategories();
        void AddCategory(CategoriesDTO categoryDto);
    }
}
