using System.Globalization;
using InfiniteIP.DbUtils;
using InfiniteIP.Models;
using Microsoft.EntityFrameworkCore;

namespace InfiniteIP.Services
{
    public class GmSheetServices : IGmSheet
    {
        private readonly InfiniteContext _context;

        public GmSheetServices(InfiniteContext context)
        {
            _context = context;
        }
        public async Task<bool> AddGmSheetAsync(List<GmSheet> gmSheets)
        {
            try
            {
                _context.GmSheet.AddRange(gmSheets);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<List<Account>> GetAccounts()
        {
            try
            {
                return await _context.Account.ToListAsync();
            }
            catch (Exception ex)
            {
                return new();
            }
        }

        public async Task<List<Project>> GetProjects(int AccountId)
        {
            try
            {
                return await _context.Projects.Where(x => x.Account.AccountId == AccountId)
                                                .ToListAsync();

            }
            catch (Exception ex)
            { return new(); }

        }
        public async Task<Sow> CreateSow(Sowparams sowparams)
        {
            try
            {
                Account account = new()
                {
                    AccountId = sowparams.AccountId
                };

                Project project = new()
                {
                    ProjectId = sowparams.ProjectId,
                };

                Sow sow = new Sow()
                {
                    account = account,
                    project = project,
                    sowName = sowparams.sowName
                };
                _context.Account.Attach(account);
                _context.Projects.Attach(project);
                _context.Sow.Add(sow);
                await _context.SaveChangesAsync();
                return sow;
            }
            catch(Exception ex)
            {
                return new();
            }

        }

        public async Task<List<Sow>> GetSow(int AccountId, int ProjectId)
        {
            try
            {
                return await _context.Sow.Where(x => x.account.AccountId == AccountId && x.project.ProjectId == ProjectId).Include(a=>a.project).Include(a=>a.account)
                                .ToListAsync();
            }
            catch(Exception ex)
            {
                return new();
            }
        }

        public async Task<List<GmSheet>> GetGmSheetAsync(int AccountId, int ProjectId)
        {
            try
            {
                return await _context.GmSheet.Where(x => x.accountId == AccountId && x.projectId == ProjectId)
                                    .OrderBy(x => x.Id).AsNoTracking()
                                    .ToListAsync();

            }
            catch (Exception ex)
            {
                return new();
            }

        }

        public async Task<bool> DeleteGmDheetAsync(int Id)
        {
            try
            {
                var rowitem = await _context.GmSheet.FindAsync(Id);
                if (rowitem == null)
                {
                    return false;
                }
                _context.Remove(rowitem);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private GmSheet KeyNotFoundException()
        {
            throw new NotImplementedException();
        }

        public async Task<Dictionary<string, RevenueDetails>> GetRevenueDetails(int AccountId, int ProjectId)
        {
            RevenueDetails onshoreerevenue = new();
            RevenueDetails offshorerevenue = new();
            RevenuOverallDetails revenuedetailsoverall = new();

            var totalCostOnshore = 0;
            var totalCostOffshore = 0;
            var totalCost = 0;
            var totalmonthlySpend = 0;

            decimal totalyearspendonshore = 0;
            decimal totalyearspendoffshore = 0;

            decimal marginoffshore = 0;
            decimal marginonshore = 0;

            decimal percentageoffshore = 0;
            decimal percentageonshore = 0;

            var resultoffshore = await _context.GmSheet
                            .Where(x => x.accountId == AccountId && x.projectId == ProjectId && x.status == "Active" && x.billable == "Yes" && x.location == "Offshore")
                            .Select(x => new
                            { billrate = decimal.Parse(x.billrate) * x.hours, x.duration, loadedrate = decimal.Parse(x.loadedrate) * x.hours * decimal.Parse(x.duration) })
                            .AsQueryable()
                            .ToListAsync();

            var resultonshore = await _context.GmSheet
                            .Where(x => x.accountId == AccountId && x.projectId == ProjectId && x.status == "Active" && x.billable == "Yes" && x.location == "Onshore")
                            .Select(x => new
                            { billrate = decimal.Parse(x.billrate) * x.hours, x.duration, loadedrate = decimal.Parse(x.loadedrate) * x.hours * decimal.Parse(x.duration) })
                            .AsQueryable()
                            .ToListAsync();

            totalCostOffshore = (int)resultoffshore.Sum(x => x.loadedrate);
            totalCostOnshore = (int)resultonshore.Sum(x => x.loadedrate);
            totalCost = totalCostOffshore + totalCostOnshore;

            totalmonthlySpend = (int)resultoffshore.Sum(x => x.billrate) + (int)resultonshore.Sum(x => x.billrate);

            totalyearspendoffshore = (decimal)resultoffshore.Sum(x => x.billrate * decimal.Parse(x.duration));
            totalyearspendonshore = (decimal)resultonshore.Sum(x => x.billrate * decimal.Parse(x.duration));

            marginoffshore = totalyearspendoffshore - totalCostOffshore;
            marginonshore = totalyearspendonshore - totalCostOnshore;

            percentageoffshore = totalyearspendoffshore > 0 ? (totalyearspendoffshore - totalCostOffshore) / totalyearspendoffshore : 0;
            percentageonshore = totalyearspendonshore > 0 ? (totalyearspendonshore - totalCostOnshore) / totalyearspendonshore : 0;

            //
            onshoreerevenue.cost = totalCostOnshore;
            onshoreerevenue.revenu = totalyearspendonshore;
            onshoreerevenue.margin = marginonshore;
            onshoreerevenue.marginpercentage = percentageonshore * 100;

            offshorerevenue.cost = totalCostOffshore;
            offshorerevenue.revenu = totalyearspendoffshore;
            offshorerevenue.margin = marginoffshore;
            offshorerevenue.marginpercentage = percentageoffshore * 100;

            Dictionary<string, RevenueDetails> dictrevenue = new()
            {
                {
                    "onshore",onshoreerevenue
                },
                {
                    "offshore",offshorerevenue
                }
            };

            return dictrevenue;
        }

        public async Task<List<Gmrunsheet>> GetRunSheet(int AccountId, int ProjectId)
        {
            var result = await _context.GmSheet
                .Where(x => x.accountId == AccountId && x.projectId == ProjectId)
                .ToListAsync();

            var months = new List<string>();

            var minDate = DateTime.Parse(result.Min(x => x.startdate));
            var maxDate = DateTime.Parse(result.Max(x => x.enddate));

            List<string> monthList = GetMonthBetween(minDate, maxDate);
            /*
            if (monthList.Count > 0)
            {
                months = monthList.Skip(Math.Max(0, monthList.Count - 3)).ToList();
            }
            */


            List<Gmrunsheet> lstgmrunsheet = new();           

            foreach (var item in result)
            {
                Gmrunsheet gmrunsheet = new();
                List<Runsheet> lstrunsheet = new();
                gmrunsheet.brspdMgr = item.brspdMgr;
                gmrunsheet.program = item.program;
                gmrunsheet.status = item.status;
                gmrunsheet.name = item.name;
                gmrunsheet.roleaspersow = item.roleaspersow;
                gmrunsheet.duration = item.duration;
                gmrunsheet.startdate = item.startdate;
                gmrunsheet.enddate = item.enddate;
                gmrunsheet.location = item.location;
                gmrunsheet.type = item.type;
                gmrunsheet.billable = item.billable;
                gmrunsheet.payrate = item.payrate;
                gmrunsheet.loadedrate = item.loadedrate;
                gmrunsheet.billrate = item.billrate;

                foreach (var month in monthList)
                {
                    Runsheet runsheet = new();
                    runsheet.GMId = item.Id;
                    runsheet.month = month;
                    runsheet.hours = item.hours;
                    runsheet.cost= decimal.Parse(item.loadedrate) * item.hours;
                    runsheet.revenue= decimal.Parse(item.billrate) * item.hours;
                    lstrunsheet.Add(runsheet);
                }

                /*
                runsheet.firsthour = item.hours;
                runsheet.secondhour = item.hours;
                runsheet.thirdhour = item.hours;
                runsheet.lblfirstmonth = monthList[0].ToString();
                runsheet.lblsecondmonth = monthList[1].ToString();
                runsheet.lblthirdmonth = monthList[2].ToString();
                runsheet.costfirstmonth = decimal.Parse(item.loadedrate) * item.hours;
                runsheet.costsecondmonth = decimal.Parse(item.loadedrate) * item.hours;
                runsheet.costthirdmonth = decimal.Parse(item.loadedrate) * item.hours;
                runsheet.revenuefirstmonth = decimal.Parse(item.billrate) * item.hours;
                runsheet.revenuesecondmonth = decimal.Parse(item.billrate) * item.hours;
                runsheet.revenuethirdmonth = decimal.Parse(item.billrate) * item.hours;
                */
                gmrunsheet.runsheet = lstrunsheet;
                gmrunsheet.totalcost = decimal.Parse(item.loadedrate) * item.hours * 3;
                gmrunsheet.totalrevenue = decimal.Parse(item.billrate) * item.hours * 3;
                gmrunsheet.totalrevenueytd = decimal.Parse(item.billrate) * item.hours * monthList.Count;
                gmrunsheet.totalrevenueytdproject = decimal.Parse(item.billrate) * item.hours * monthList.Count;
                lstgmrunsheet.Add(gmrunsheet);
            }

            return lstgmrunsheet;
        }


        public static List<string> GetMonthBetween(DateTime startDate, DateTime endDate)
        {
            List<string> month = new();
            DateTime iterator = new DateTime(startDate.Year, startDate.Month, 1);

            while (iterator <= endDate)
            {
                month.Add(iterator.ToString("MMMM yyyy", CultureInfo.InvariantCulture));
                iterator = iterator.AddMonths(1);
            }

            return month;
        }

    }

}
