using System.ComponentModel.DataAnnotations;

namespace InfiniteIP.Models
{
    public class Runsheet
    {
        public int GMId { get; set; }
        public string month { get; set; }
        public decimal hours { get; set; }
        public decimal cost { get; set; }
        public decimal revenue { get; set; }
        public bool currentMonth { get; set; } = false;
        public bool isCurrentMonthActive { get; set; } = false;
    }

    public class GmRunsheet
    {
        [Key]
        public int Id { get; set; }
        public int GmId { get; set; }
        public string month { get; set; }
        public decimal hours { get; set; }
    }
}
