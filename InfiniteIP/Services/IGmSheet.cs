using InfiniteIP.Models;

namespace InfiniteIP.Services
{
    public interface IGmSheet
    {
        Task<bool> AddGmSheetAsync(List<GmSheet> gmSheets);
        Task<List<GmSheet>> GetGmSheetAsync(int AccountId, int ProjectId, int Runsheet);

        Task<Dictionary<string, RevenueDetails>> GetRevenueDetails(int AccountId, int ProjectId);

        Task<bool> DeleteGmDheetAsync(int Id);

        Task<GmRunSheetResponse> GetRunSheet(int AccountId, int ProjectId);

        Task<List<Account>> GetAccounts();
        Task<List<Project>> GetProjects(int AccountId);

        Task<Sow> CreateSow(Sowparams sowparams);

        Task<List<Sow>> GetSow(int AccountId, int ProjectId);

        Task<Dictionary<string, Runsheetsummary>> GetRunsheetsummary(int AccountId, int ProjectId);

        Task<bool> SaveRunSheetUsers(List<GmRunsheet> gmRunsheets);

        Task<bool> SubmitGMSheetAsync(int AccountId, int ProjectId, int sow);

        Task<List<GmSheet>> GetGmSheetsubmitAsync(int AccountId, int ProjectId, int sow, int Runsheet);
        Task<bool> CanAddGmSheetAsync(int AccountId, int ProjectId, int sow);
    }
}
