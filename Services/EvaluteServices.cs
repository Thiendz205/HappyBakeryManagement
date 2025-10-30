using HappyBakeryManagement.Data;
using HappyBakeryManagement.DTO;

namespace HappyBakeryManagement.Services
{
    public class EvaluteServices: IEvaluteServices
    {
        public readonly ApplicationDbContext _db;
        public EvaluteServices(ApplicationDbContext db)
        {
            _db = db;
        }
        public List<EvaluteDTO> getAllEvalute()
        {
            var Evalute = from c in _db.Evaluates
                             select new EvaluteDTO
                             {
                                 Id = c.Id,
                                 CustomerID = c.Customer.Id,
                                 ProductID = c.Product.Id,
                                 Score = c.Score,
                                 Evaluationdate = c.Evaluationdate
                             };
            return Evalute.ToList();
        }
    }
}
