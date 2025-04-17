using LeaveManagementSystem.DTOs;

namespace LeaveManagementSystem.Strategies
{
    public class AnnualLeaveValidationStrategy : ILeaveValidationStrategy
    {
        public Task<bool> ValidateAsync(CreateLeaveRequestDto dto)
        {
            return Task.FromResult(true);
        }
    }
}
