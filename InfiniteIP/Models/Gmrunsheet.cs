using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace InfiniteIP.Models
{
    public class Gmrunsheet
    {       
        public int accountId { get; set; }
        public int projectId { get; set; }
        public int sow { get; set; }
        public string brspdMgr { get; set; }
        public string program { get; set; }
        public string status { get; set; }
        public string name { get; set; }
        public string roleaspersow { get; set; }
        public string duration { get; set; }
        public decimal hours { get; set; }
        public DateTime startdate { get; set; }
        public DateTime enddate { get; set; }
        public string location { get; set; }
        public string type { get; set; }
        public string billrate { get; set; }
        public string payrate { get; set; }
        public string loadedrate { get; set; }
        public string billable { get; set; }        
        public decimal totalcost { get; set; }
        public decimal totalrevenue { get; set; }
        public decimal totalrevenueytd { get; set; }
        public decimal totalrevenueytdproject { get; set; }
        public List<Runsheet> runsheet { get; set; }
    }
}
