﻿namespace LeaveManagementSystem.DTOs
{
    public class LeaveRequestFilterDto
    {
        public int? EmployeeId { get; set; }
        public string? LeaveType { get; set; }
        public string? Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public string? SortBy { get; set; } = "CreatedAt";
        public string? SortOrder { get; set; } = "desc";

        public string? Keyword { get; set; }
    }
}
