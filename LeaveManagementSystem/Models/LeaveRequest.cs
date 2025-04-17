namespace LeaveManagementSystem.Models
{
    public enum LeaveType
    {
        Annual,
        Sick,
        Other
    }

    public enum Status
    {
        Pending,
        Approved,
        Rejected
    }
    public class LeaveRequest
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }

        public LeaveType LeaveType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public Status Status { get; set; }
        public string Reason { get; set; }
        public DateTime CreatedAt { get; set; }

        public Employee Employee { get; set; }
    }
}
