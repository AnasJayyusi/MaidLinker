using MaidLinker.Data.Entites;

namespace MaidLinker.Models
{
    public class DashboardViewModel
    {
        public List<Request> NewRequests { get; set; } = new();
        public List<Request> InProgressRequests { get; set; } 
        public StatusCountsViewModel StatusCounts { get; set; } = new();
    }
}

public class StatusCountsViewModel
{
    public int New { get; set; }
    public int InProgress { get; set; }
    public int Completed { get; set; }

}

