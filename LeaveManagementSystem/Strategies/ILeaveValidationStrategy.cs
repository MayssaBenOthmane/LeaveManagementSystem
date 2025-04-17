using LeaveManagementSystem.DTOs;

namespace LeaveManagementSystem.Strategies
{
    public interface ILeaveValidationStrategy
    {
        Task<bool> ValidateAsync(CreateLeaveRequestDto dto);
    }
}
