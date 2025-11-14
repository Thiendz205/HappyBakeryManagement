using HappyBakeryManagement.DTO;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HappyBakeryManagement.Services
{
    public interface IEvaluteServices
    {
        List<EvaluteDTO> getAllEvalute();

        Task<List<EvaluteViewModel>> GetAllEvalutesWithInfoAsync();

        List<EvaluteDTO> GetEvalutesByProductId(int productId);
        Task<(bool ok, string? error)> AddEvaluteAsync(EvaluteDTO dto, ClaimsPrincipal user);


    }
}
