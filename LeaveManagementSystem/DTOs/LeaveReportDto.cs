namespace LeaveManagementSystem.DTOs
{
    public class LeaveReportDto
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public int TotalLeaves { get; set; }
        public int AnnualLeaves { get; set; }
        public int SickLeaves { get; set; }
    }
}
