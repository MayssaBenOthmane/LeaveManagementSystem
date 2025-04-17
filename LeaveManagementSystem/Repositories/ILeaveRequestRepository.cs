using LeaveManagementSystem.Models;

namespace LeaveManagementSystem.Repositories
{
    public interface ILeaveRequestRepository
    {
        Task<List<LeaveRequest>> GetAllAsync();
        Task<LeaveRequest> GetByIdAsync(int id);
        Task AddAsync(LeaveRequest leaveRequest);
        Task SaveChangesAsync();
    }
}
