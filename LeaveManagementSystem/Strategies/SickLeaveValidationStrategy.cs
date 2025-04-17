using LeaveManagementSystem.DTOs;

namespace LeaveManagementSystem.Strategies
{
    public class SickLeaveValidationStrategy : ILeaveValidationStrategy
    {
        public Task<bool> ValidateAsync(CreateLeaveRequestDto dto)
        {
            return Task.FromResult(true);
        }
    }
}
