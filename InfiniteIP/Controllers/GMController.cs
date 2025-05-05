using InfiniteIP.Models;
using InfiniteIP.Services;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using System.Globalization;

namespace InfiniteIP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GMController : ControllerBase
    {
        private readonly IGmSheet _service;

        public GMController(IGmSheet service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> CreateGmSheet([FromBody] List<GmSheet> gmSheets)
        {
            try
            {
                var response = await _service.AddGmSheetAsync(gmSheets);
                return response ? Ok() : BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /*
        [HttpPost]
        public async Task<IActionResult> SubmitGmSheet(int AccountId,int ProjectId,int snow)
        {
            try
            {
                var response = await _service.AddGmSheetAsync(gmSheets);
                return response ? Ok() : BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        */
        [HttpGet("{AccountId}/{ProjectId}")]
        public async Task<IActionResult> GetGmSheet(int AccountId, int ProjectId)
        {
            try
            {
                var response = await _service.GetGmSheetAsync(AccountId, ProjectId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("Runsheet/{AccountId}/{ProjectId}")]
        public async Task<IActionResult> GetRunSheet(int AccountId, int ProjectId)
        {
            try
            {
                var response = await _service.GetRunSheet(AccountId, ProjectId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("SaveRunSheetUsers")]
        public async Task<IActionResult> SaveRunSheetUsers([FromBody] List<GmRunsheet> gmRunsheets)
        {
            try
            {
                var response = await _service.SaveRunSheetUsers(gmRunsheets);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet("RevenueDetails/{AccountId}/{ProjectId}")]
        public async Task<IActionResult> GetRevenueDetails(int AccountId, int ProjectId)
        {
            try
            {
                var response = await _service.GetRevenueDetails(AccountId, ProjectId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("Runsheetsummary/{AccountId}/{ProjectId}")]
        public async Task<IActionResult> GetRunsheetsummary(int AccountId, int ProjectId)
        {
            try
            {
                var response = await _service.GetRunsheetsummary(AccountId, ProjectId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{Id}")]
        public async Task<IActionResult> DeleteGmDheetAsync(int Id)
        {
            try
            {
                var res = await _service.DeleteGmDheetAsync(Id);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("Excel/{AccountId}/{ProjectId}")]
        public async Task<IActionResult> ExportToExcel(int AccountId, int ProjectId)
        {
            var datas = await _service.GetGmSheetAsync(AccountId, ProjectId);

            var minDate = datas.Min(x => x.startdate);
            var maxDate = datas.Max(x => x.enddate);

            var startDate = minDate;
            var endDate = maxDate;

            decimal TotalCost = 0;
            decimal TotalRev = 0;
            decimal TotalRevCum = 0;
            decimal TotalCostCum = 0;

            decimal TotalRevYTD = 0;
            decimal TotalCostYTD = 0;

            using var package = new ExcelPackage();
            using var worksheet_summary = package.Workbook.Worksheets.Add("SUMMARY");
            using var worksheet = package.Workbook.Worksheets.Add("ETL CR08");

            worksheet.Cells[6, 1].Value = "S.No";
            worksheet.Cells[6, 2].Value = "BRSPD Mgr";
            worksheet.Cells[6, 3].Value = "Program";
            worksheet.Cells[6, 4].Value = "Status";
            worksheet.Cells[6, 5].Value = "Name";
            worksheet.Cells[6, 6].Value = "Role As Per SOW";
            worksheet.Cells[6, 7].Value = "Duration";
            worksheet.Cells[6, 8].Value = "Start Date";
            worksheet.Cells[6, 9].Value = "End Date";
            worksheet.Cells[6, 10].Value = "Location";
            worksheet.Cells[6, 11].Value = "Type";
            worksheet.Cells[6, 12].Value = "Bill Rate";
            worksheet.Cells[6, 13].Value = "Pay Rate";
            worksheet.Cells[6, 14].Value = "Loaded Rate";
            worksheet.Cells[6, 15].Value = "Billable";

            var (monthList, yr) = GmSheetServices.GetMonthBetween(startDate, endDate);
            int k = 1;
            foreach (var month in monthList)
            {
                worksheet.Cells[6, 15 + k].Value = "Cost " + month.ToString();
                k++;
            }

            int costcell = k + 15;

            foreach (var month in monthList)
            {
                worksheet.Cells[6, 15 + k].Value = "Revnue " + month.ToString();
                k++;
            }

            worksheet.Cells[6, 15 + k].Value = "Total Revenue";
            worksheet.Cells[6, 15 + k + 1].Value = "Total Cost";

            k = k + 2;

            using (var range = worksheet.Cells[6, 1, 6, 15 + k - 1])
            {
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(Color.LightSkyBlue);
                range.Style.Font.Bold = true;
                range.Style.Font.Color.SetColor(Color.Black);

                range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
            }

            int i = 7;
            int j = 1;

            foreach (var data in datas)
            {
                worksheet.Cells[i, 1].Value = j; j++;
                worksheet.Cells[i, 2].Value = data.brspdMgr;
                worksheet.Cells[i, 3].Value = data.program;
                worksheet.Cells[i, 4].Value = data.status;
                worksheet.Cells[i, 5].Value = data.name;
                worksheet.Cells[i, 6].Value = data.roleaspersow;
                worksheet.Cells[i, 7].Value = data.duration;
                worksheet.Cells[i, 8].Value = startDate.ToString("dd/MM/yyyy");
                worksheet.Cells[i, 9].Value = data.enddate;
                worksheet.Cells[i, 10].Value = data.location;
                worksheet.Cells[i, 11].Value = data.type;
                worksheet.Cells[i, 12].Value = data.billrate;
                worksheet.Cells[i, 13].Value = data.payrate;
                worksheet.Cells[i, 14].Value = data.loadedrate;
                worksheet.Cells[i, 15].Value = data.billable;

                TotalRevYTD += decimal.Parse(data.billrate) * 168;
                TotalCostYTD += decimal.Parse(data.loadedrate) * 168;

                for (int x = 1; x < k - 2; x++)
                {
                    if (x + 15 < costcell)
                    {
                        TotalCost += decimal.Parse(data.loadedrate) * 168;
                        worksheet.Cells[i, 15 + x].Value = decimal.Parse(data.loadedrate) * 168;
                    }
                    else
                    {
                        TotalRev += decimal.Parse(data.billrate) * 168;
                        worksheet.Cells[i, 15 + x].Value = decimal.Parse(data.billrate) * 168;
                    }
                }

                worksheet.Cells[i, 15 + k - 1].Value = TotalCost;
                worksheet.Cells[i, 15 + k - 2].Value = TotalRev;
                TotalRevCum += TotalRev;
                TotalCostCum += TotalCost;
                TotalCost = 0;
                TotalRev = 0;

                i++;
            }

            using (var range = worksheet.Cells[7, 1, 7 + datas.Count() - 1, 15 + k - 1])
            {
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(Color.Wheat);
                range.Style.Font.Bold = true;
                range.Style.Font.Color.SetColor(Color.Black);
                range.Style.Border.BorderAround(ExcelBorderStyle.Thin);

                range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
            }

            worksheet.Column(1).AutoFit();
            worksheet.Column(2).AutoFit();
            worksheet.Column(3).AutoFit();
            worksheet.Column(4).AutoFit();
            worksheet.Column(5).AutoFit();
            worksheet.Column(6).AutoFit();
            worksheet.Column(7).AutoFit();
            worksheet.Column(8).AutoFit();
            worksheet.Column(9).AutoFit();
            worksheet.Column(10).AutoFit();
            worksheet.Column(11).AutoFit();
            worksheet.Column(12).AutoFit();
            worksheet.Column(13).AutoFit();
            worksheet.Column(14).AutoFit();
            worksheet.Column(15).AutoFit();

            for (int n = 1; n < k; n++)
            {
                worksheet.Column(15 + n).AutoFit();
            }

            //Summary Actual
            worksheet_summary.Cells["A1:B1"].Merge = true;
            worksheet_summary.Cells[1, 1].Value = "Summary(Actual + Projection)";
            worksheet_summary.Cells[1, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            worksheet_summary.Cells[1, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet_summary.Cells[1, 1].Style.Fill.BackgroundColor.SetColor(Color.Pink);
            worksheet_summary.Cells[1, 1].Style.Font.Bold = true;

            worksheet_summary.Cells[2, 1].Value = "Actual Revenue + Projection";
            worksheet_summary.Cells[3, 1].Value = "After Discount";
            worksheet_summary.Cells[4, 1].Value = "Planned GM %";
            worksheet_summary.Cells[5, 1].Value = "Planned GM";
            worksheet_summary.Cells[6, 1].Value = "Pleanned cost not to exceed";
            worksheet_summary.Cells[7, 1].Value = "Acutal cost + Projection";
            worksheet_summary.Cells[8, 1].Value = "Cost Pverrun/within limit";
            worksheet_summary.Cells[9, 1].Value = "Projected GM";
            worksheet_summary.Cells[11, 1].Value = "Balance Amount Projected";
            worksheet_summary.Column(1).AutoFit();

            using (var range = worksheet_summary.Cells[1, 1, 11, 2])
            {
                range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
            }
            decimal plannedGM = 0;
            decimal costoverrun = 0;
            worksheet_summary.Cells[2, 2].Value = TotalRevCum;
            worksheet_summary.Cells[3, 2].Value = TotalRevCum;
            worksheet_summary.Cells[4, 2].Value = "25%";
            plannedGM = TotalRevCum * 25 / 100;
            costoverrun = TotalRevCum - plannedGM;
            worksheet_summary.Cells[5, 2].Value = plannedGM;
            worksheet_summary.Cells[6, 2].Value = costoverrun;
            worksheet_summary.Cells[7, 2].Value = TotalCostCum;
            worksheet_summary.Cells[8, 2].Value = costoverrun - TotalCostCum;
            worksheet_summary.Cells[9, 2].Value = ((TotalRevCum - TotalCostCum) / TotalRevCum).ToString("F2");
            worksheet_summary.Cells[11, 2].Value = 0;


            //Summary YTD
            worksheet_summary.Cells["D1:E1"].Merge = true;
            worksheet_summary.Cells[1, 4].Value = "Summary(YTD)";
            worksheet_summary.Cells[1, 4].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            worksheet_summary.Cells[1, 4].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet_summary.Cells[1, 4].Style.Fill.BackgroundColor.SetColor(Color.Pink);
            worksheet_summary.Cells[1, 4].Style.Font.Bold = true;

            worksheet_summary.Cells[2, 4].Value = "Actual Revenue YTD";
            worksheet_summary.Cells[3, 4].Value = "After Discount";
            worksheet_summary.Cells[4, 4].Value = "Planned GM %";
            worksheet_summary.Cells[5, 4].Value = "Planned GM";
            worksheet_summary.Cells[6, 4].Value = "Pleanned cost not to exceed";
            worksheet_summary.Cells[7, 4].Value = "Acutal cost YTD";
            worksheet_summary.Cells[8, 4].Value = "Cost Pverrun/within limit";
            worksheet_summary.Cells[9, 4].Value = "Actual GM";
            worksheet_summary.Cells[11, 4].Value = "Balance Amount YTD";
            worksheet_summary.Column(4).AutoFit();
            worksheet_summary.Column(5).AutoFit();

            worksheet_summary.Cells[2, 5].Value = TotalRevYTD;
            worksheet_summary.Cells[3, 5].Value = TotalRevYTD * 1;
            worksheet_summary.Cells[4, 5].Value = "25%";
            worksheet_summary.Cells[5, 5].Value = TotalRevYTD * 25 / 100;
            worksheet_summary.Cells[6, 5].Value = TotalRevYTD - (TotalRevYTD * 25 / 100);
            worksheet_summary.Cells[7, 5].Value = TotalCostYTD;
            worksheet_summary.Cells[8, 5].Value = (TotalRevYTD - (TotalRevYTD * 25 / 100)) - TotalCostYTD;
            worksheet_summary.Cells[9, 5].Value = ((TotalRevYTD - TotalCostYTD) / TotalRevYTD).ToString("F2");
            worksheet_summary.Cells[11, 5].Value = 0;

            for (int p = 2; p < 11; p++)
            {
                worksheet_summary.Cells[p, 5].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                worksheet_summary.Cells[p, 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
            }

            using (var range = worksheet_summary.Cells[1, 4, 11, 5])
            {
                range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
            }

            var stream = new MemoryStream();
            package.SaveAs(stream);
            stream.Position = 0;

            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            return File(stream, contentType);
        }

        public static List<string> GetMonthBetween1(DateTime startDate, DateTime endDate)
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
