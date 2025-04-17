using LeaveManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagementSystem.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<LeaveRequest> LeaveRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>().HasData(
                new Employee { Id = 1, FullName = "Mayssa Ben Othmane", Department = "IT", JoiningDate = new DateTime(2025, 4, 15) },
                new Employee { Id = 2, FullName = "Mohamed Youssef", Department = "IT", JoiningDate = new DateTime(2022, 6, 1) }
            );

            modelBuilder.Entity<LeaveRequest>().HasData(
                new LeaveRequest
                {
                    Id = 1,
                    EmployeeId = 1,
                    LeaveType = LeaveType.Annual,
                    StartDate = new DateTime(2022, 6, 10),
                    EndDate = new DateTime(2022, 6, 15),
                    Status = Status.Pending,
                    Reason = "Summer vacation",
                    CreatedAt = DateTime.Now
                }
            );
        }
    }
}