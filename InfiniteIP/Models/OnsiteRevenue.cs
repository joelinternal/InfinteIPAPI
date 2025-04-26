namespace InfiniteIP.Models
{
    public class RevenueDetails
    {
        public decimal revenu { get; set; }
        public decimal cost { get; set; }
        public decimal margin { get; set; }
        public decimal marginpercentage { get; set; }
        public decimal totalrevenue {  get; set; } 
        public decimal totalmargin { get; set; }  
        
        public List<RevenuOverallDetails> revoverall { get; set; }

    }

    public class RevenuOverallDetails
    {
        public string duration { get; set; }
    }
}
