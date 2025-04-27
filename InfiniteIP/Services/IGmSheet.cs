using InfiniteIP.Models;

namespace InfiniteIP.Services
{
    public interface IGmSheet
    {
        Task<bool> AddGmSheetAsync(List<GmSheet> gmSheets);
        Task<List<GmSheet>> GetGmSheetAsync(int AccountId, int ProjectId);

        Task<Dictionary<string, RevenueDetails>> GetRevenueDetails(int AccountId, int ProjectId);

        Task<bool> DeleteGmDheetAsync(int Id);

        Task<List<Gmrunsheet>> GetRunSheet(int AccountId, int ProjectId);

        Task<List<Account>> GetAccounts();
        Task<List<Project>> GetProjects(int AccountId);

        Task<Sow> CreateSow(Sowparams sowparams);

        Task<List<Sow>> GetSow(int AccountId, int ProjectId);

        Task<Dictionary<string, Runsheetsummary>> GetRunsheetsummary(int AccountId, int ProjectId);

    }
}
