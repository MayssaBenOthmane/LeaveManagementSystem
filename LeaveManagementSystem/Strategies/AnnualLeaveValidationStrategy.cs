using LeaveManagementSystem.DTOs;

namespace LeaveManagementSystem.Strategies
{
    public class AnnualLeaveValidationStrategy : ILeaveValidationStrategy
    {
        public Task<bool> ValidateAsync(CreateLeaveRequestDto dto)
        {
            // Add validation logic for annual leave
            return Task.FromResult(true);
        }
    }
}
