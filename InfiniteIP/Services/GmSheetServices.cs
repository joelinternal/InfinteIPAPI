using System.Globalization;
using InfiniteIP.DbUtils;
using InfiniteIP.Models;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.FormulaParsing.FormulaExpressions;

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

        public async Task<bool> SubmitGmSheetAsync(List<GmSheet> gmSheets)
        {
            try
            {
                var result = await _context.GmSheet.Where(x => x.accountId == gmSheets[0].accountId && x.projectId == gmSheets[0].projectId && x.sow == gmSheets[0].sow)
                                    .ToListAsync();

                if (result.Count > 0)
                {
                    _context.Entry(gmSheets).State = EntityState.Modified;
                }
                else
                {
                    gmSheets.ForEach(a =>
                    {
                        a.startdate.AddDays(1);
                        a.enddate.AddDays(1);
                    });
                    _context.GmSheet.AddRange(gmSheets);
                }
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
            catch (Exception ex)
            {
                return new();
            }

        }

        public async Task<List<Sow>> GetSow(int AccountId, int ProjectId)
        {
            try
            {
                return await _context.Sow.Where(x => x.account.AccountId == AccountId && x.project.ProjectId == ProjectId).Include(a => a.project).Include(a => a.account)
                                .ToListAsync();
            }
            catch (Exception ex)
            {
                return new();
            }
        }

        public async Task<List<GmSheet>> GetGmSheetAsync(int AccountId, int ProjectId, int Runsheet)
        {
            try
            {
                return await _context.GmSheet.Where(x => x.accountId == AccountId && x.projectId == ProjectId
                 && (Runsheet == 1 || x.source != "RunSheet"))
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

            var resultmonth = await _context.GmSheet
                                   .Where(x => x.accountId == AccountId && x.projectId == ProjectId)
                                   .Select(x => new
                                   { x.startdate, x.enddate })
                                   .AsQueryable()
                                   .ToListAsync();

            var months = new List<string>();


            if (resultmonth.Any())
            {
                var minDate = resultmonth.Min(x => x.startdate);
                var maxDate = resultmonth.Max(x => x.enddate);
                var (monthList, yearList) = GetMonthBetween(minDate, maxDate);


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
                onshoreerevenue.monthcount = monthList.Count;

                offshorerevenue.cost = totalCostOffshore;
                offshorerevenue.revenu = totalyearspendoffshore;
                offshorerevenue.margin = marginoffshore;
                offshorerevenue.marginpercentage = percentageoffshore * 100;

            }
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


        public async Task<bool> SaveRunSheetUsers(List<GmRunsheet> gmRunsheets)
        {
            try
            {
                List<int> GmIds = gmRunsheets.Select(gm => gm.GmId).ToList();
                var runSheets = await _context.GmRunsheet.Where(a => GmIds.Contains(a.GmId)).ToListAsync();
                if (runSheets.Any())
                {
                    foreach (var runsheet in gmRunsheets)
                    {
                        var checkExist = runSheets.Where(a => a.GmId == runsheet.GmId && a.month == runsheet.month).FirstOrDefault();
                        if (checkExist == null)
                        {
                            _context.GmRunsheet.Add(runsheet);
                        }
                        else
                        {
                            checkExist.hours = runsheet.hours;
                            _context.GmRunsheet.Update(checkExist);
                        }
                    }
                    await _context.SaveChangesAsync();
                }
                else
                {
                    _context.GmRunsheet.AddRange(gmRunsheets);
                    await _context.SaveChangesAsync();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<GmRunSheetResponse> GetRunSheet(int AccountId, int ProjectId)
        {
            GmRunSheetResponse gmRunSheetResponse = new();
            List<string> columnHeaders = new();
            List<string> chHeader = new();
            List<string> costHeader = new();
            List<string> rhHeader = new();
            List<string> revHeader = new();

            List<string> monthHeaders = new();
            List<string> monthHeader = new();

            var result = await _context.GmSheet
                .Where(x => x.accountId == AccountId && x.projectId == ProjectId)
                .ToListAsync();

            List<int> gmIds = result.Select(a => a.Id).ToList();

            var gmRunSheetData = await _context.GmRunsheet.Where(a => gmIds.Contains(a.GmId)).ToListAsync();

            var months = new List<string>();

            var minDate = result.Min(x => x.startdate);
            var maxDate = result.Max(x => x.enddate);

            var (monthList, yearList) = GetMonthBetween(minDate, maxDate);

            int i = 0;
            foreach (string yr in yearList)
            {
                chHeader.Add($"CH{yr}");
                costHeader.Add("Cost");
                rhHeader.Add($"RH{yr}");
                revHeader.Add("Rev");
                monthHeader.Add(monthList[i++]);
            }

            columnHeaders.AddRange(chHeader);
            columnHeaders.AddRange(costHeader);
            columnHeaders.AddRange(rhHeader);
            columnHeaders.AddRange(revHeader);

            monthHeaders.AddRange(monthHeader);
            monthHeaders.AddRange(monthHeader);
            monthHeaders.AddRange(monthHeader);
            monthHeaders.AddRange(monthHeader);

            List<Gmrunsheet> lstgmrunsheet = new();

            foreach (var item in result)
            {
                Gmrunsheet gmrunsheet = new();
                List<Runsheet> lstrunsheet = new();
                gmrunsheet.GmId = item.Id;
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

                var (currentMonthList, _) = GetMonthBetween(item.startdate, item.enddate);

                string currentMonthStr = DateTime.Now.ToString("MMM yy");
                string previousMonthStr = DateTime.Now.AddMonths(-1).ToString("MMM yy");

                foreach (var month in monthList)
                {
                    var runData = gmRunSheetData
                    .Where(a => a.GmId == item.Id && a.month == month)
                    .FirstOrDefault();
                    Runsheet runsheet = new();
                    runsheet.GMId = item.Id;
                    runsheet.month = month;
                    runsheet.hours = runData == null ? item.hours : runData.hours;
                    runsheet.cost = decimal.Parse(item.loadedrate) * (runData == null ? item.hours : runData.hours);
                    runsheet.revenue = decimal.Parse(item.billrate) * (runData == null ? item.hours : runData.hours);
                    runsheet.currentMonth = false;
                    if (currentMonthStr == month || previousMonthStr == month)
                    {
                        runsheet.currentMonth = true;
                    }
                    runsheet.isCurrentMonthActive = currentMonthList.Contains(month);
                    lstrunsheet.Add(runsheet);                    
                }

                gmrunsheet.runsheet = lstrunsheet;
                gmrunsheet.totalcost = decimal.Parse(item.loadedrate) * item.hours * monthList.Count;
                gmrunsheet.totalrevenue = decimal.Parse(item.billrate) * item.hours * monthList.Count;
                gmrunsheet.totalrevenueytd = decimal.Parse(item.billrate) * item.hours * monthList.Count;
                gmrunsheet.totalrevenueytdproject = decimal.Parse(item.billrate) * item.hours * monthList.Count;
                lstgmrunsheet.Add(gmrunsheet);
            }
            gmRunSheetResponse.gmRunSheet = lstgmrunsheet;
            gmRunSheetResponse.columnHeader = columnHeaders;
            gmRunSheetResponse.monthHeaders = monthHeaders;
            return gmRunSheetResponse;
        }

        //GM sheet Summary Summary(Actual+Projection) && YTD
        public async Task<Dictionary<string, Runsheetsummary>> GetRunsheetsummary(int AccountId, int ProjectId)
        {
            Runsheetsummary runsheetsummary = new();
            Runsheetsummary runsheetsummaryYtd = new();

            var result = await _context.GmSheet
                          .Where(x => x.accountId == AccountId && x.projectId == ProjectId && x.status == "Active" && x.billable == "Yes")
                          .Select(x => new
                          { billrate = decimal.Parse(x.billrate) * x.hours, x.duration, x.startdate, x.enddate, x.Id, loadedrate = decimal.Parse(x.loadedrate) * x.hours * decimal.Parse(x.duration) })
                          .AsQueryable()
                          .ToListAsync();

            decimal totalRevenue = 0;
            decimal totalCost = 0;
            decimal plannedgm = 0;
            decimal afterdiscount = 0;
            decimal actualrevenueprojection = 0;
            decimal afterdiscountYtd = 0;
            decimal plannedgmYtd = 0;

            if (result.Any())
            {
                var minDate = result.Min(x => x.startdate);
                var maxDate = result.Max(x => x.enddate);

                var (monthList, _) = GetMonthBetween(minDate, maxDate);

                totalRevenue = result.Sum(x => x.billrate) * monthList.Count;
                totalCost = result.Sum(x => x.loadedrate) * monthList.Count;

                actualrevenueprojection = totalRevenue;
                afterdiscountYtd = actualrevenueprojection * 1;

                afterdiscount = totalRevenue * 1;
                plannedgm = (totalRevenue * 1) * 25 / 100;

                plannedgmYtd = (actualrevenueprojection * 1) * 25 / 100;

                foreach (var res in result)
                {
                    var resmonthly = await _context.GmRunsheet
                                    .Where(x => x.Id == res.Id)
                                    .AsQueryable()
                                    .ToListAsync();
                }

                //Summary Actual+Projection
                runsheetsummary.actualrevenueprojection = totalRevenue;
                runsheetsummary.afterdiscount = afterdiscount;//as per the Excel
                runsheetsummary.plannedgmpercentage = 25;//as per excel
                runsheetsummary.plannedgm = plannedgm;
                runsheetsummary.plannedcostnottoextend = afterdiscount - plannedgm;
                runsheetsummary.actualcostprojection = totalCost;
                runsheetsummary.costoverrun = (afterdiscount - plannedgm) - totalCost;
                runsheetsummary.projectgmpercentage = (int)((afterdiscount - totalCost) / afterdiscount);
                runsheetsummary.balanceamountprojected = totalRevenue; //--do verify

                //Summary YTD
                runsheetsummaryYtd.actualrevenueprojection = actualrevenueprojection;
                runsheetsummaryYtd.afterdiscount = afterdiscountYtd;
                runsheetsummaryYtd.plannedgmpercentage = 25;
                runsheetsummaryYtd.plannedgm = plannedgmYtd;
                runsheetsummaryYtd.plannedcostnottoextend = afterdiscountYtd - plannedgmYtd;
                runsheetsummaryYtd.actualcostprojection = totalCost;
                runsheetsummaryYtd.costoverrun = (afterdiscountYtd - plannedgmYtd) - totalCost;
                runsheetsummaryYtd.projectgmpercentage = (int)((afterdiscountYtd - totalCost) / afterdiscountYtd);
                runsheetsummaryYtd.balanceamountprojected = totalRevenue;
            }

            Dictionary<string, Runsheetsummary> dictionaryrunsheet = new()
            {
                {
                    "SummaryActual",runsheetsummary
                },
                {
                    "SummaryYTD",runsheetsummaryYtd
                }
            };

            return dictionaryrunsheet;
        }


        public static (List<string>, List<string>) GetMonthBetween(DateTime startDate, DateTime endDate)
        {
            List<string> month = new();
            List<string> yr = new();
            DateTime iterator = new DateTime(startDate.Year, startDate.Month, 1);

            while (iterator <= endDate)
            {
                month.Add(iterator.ToString("MMM yy", CultureInfo.InvariantCulture));
                yr.Add(iterator.ToString("yy", CultureInfo.InvariantCulture));
                iterator = iterator.AddMonths(1);
            }

            return (month, yr);
        }


    }

}
