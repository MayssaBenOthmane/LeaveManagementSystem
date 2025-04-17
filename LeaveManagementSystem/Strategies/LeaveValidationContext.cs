using LeaveManagementSystem.DTOs;
using LeaveManagementSystem.Models;

namespace LeaveManagementSystem.Strategies
{
    public class LeaveValidationContext
    {
        private readonly IServiceProvider _serviceProvider;

        public LeaveValidationContext(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<bool> ValidateAsync(CreateLeaveRequestDto dto)
        {
            // Try to parse the string to the corresponding enum value
            if (!Enum.TryParse(dto.LeaveType, true, out LeaveType leaveType))
            {
                throw new ArgumentException($"Invalid leave type: {dto.LeaveType}. Must be either 'Sick' or 'Annual'.");
            }

            // Use the parsed enum in the switch statement
            ILeaveValidationStrategy strategy = leaveType switch
            {
                LeaveType.Sick => _serviceProvider.GetRequiredService<SickLeaveValidationStrategy>(),
                LeaveType.Annual => _serviceProvider.GetRequiredService<AnnualLeaveValidationStrategy>(),
                _ => throw new ArgumentException("Unsupported leave type.")
            };

            return await strategy.ValidateAsync(dto);
        }
    }
}
