using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace InfiniteIP.Models
{
    public class Sow
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int sowId { get; set; }
        public string sowName { get; set; }       
        public Account account { get; set; }
        public Project project { get; set; }

    }

    public class Sowparams
    {
        public int AccountId { get; set; }
        public string sowName { get; set; }
        public int ProjectId { get;set; }
    }
}
