using AutoMapper;
using LeaveManagementSystem.Data;
using LeaveManagementSystem.DTOs;
using LeaveManagementSystem.Models;
using LeaveManagementSystem.Repositories;
using LeaveManagementSystem.Strategies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace LeaveManagementSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LeaveRequestsController : ControllerBase
    {
        private readonly AppDbContext context;
        private readonly IMapper mapper;
        private readonly ILeaveRequestRepository repository;
        private readonly LeaveValidationContext validationContext;

        public LeaveRequestsController(AppDbContext context, IMapper mapper, ILeaveRequestRepository repository, LeaveValidationContext validationContext)
        {
            this.context = context;
            this.mapper = mapper;
            this.repository = repository;
            this.validationContext = validationContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var leaveRequests = await context.LeaveRequests.ToListAsync();
            var dto = mapper.Map<List<LeaveRequestDto>>(leaveRequests);
            return Ok(dto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var leaveRequest = await context.LeaveRequests.FindAsync(id);
            if (leaveRequest == null) return NotFound();
            return Ok(mapper.Map<LeaveRequestDto>(leaveRequest));
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateLeaveRequestDto dto)
        {
            var employeeExists = await context.Employees.AnyAsync(e => e.Id == dto.EmployeeId);
            if (!employeeExists)
            {
                return BadRequest("Invalid EmployeeId. Employee does not exist.");
            }

            if (dto.EndDate < dto.StartDate)
            {
                return BadRequest("End date cannot be earlier than start date.");
            }

            if (Enum.TryParse<LeaveType>(dto.LeaveType, true, out var leaveType) && leaveType == LeaveType.Sick && string.IsNullOrWhiteSpace(dto.Reason))
            {
                return BadRequest("Sick leave must include a reason.");
            }

            bool isOverlapping = await context.LeaveRequests.AnyAsync(r =>
                r.EmployeeId == dto.EmployeeId &&
                r.StartDate <= dto.EndDate &&
                r.EndDate >= dto.StartDate);

            if (isOverlapping)
            {
                return BadRequest("Leave request overlaps with existing request.");
            }

            if (Enum.TryParse<LeaveType>(dto.LeaveType, true, out var leaveTypeAnnual) && leaveTypeAnnual == LeaveType.Annual)
            {
                var currentYear = dto.StartDate.Year;
                var totalAnnualDays = await context.LeaveRequests
                    .Where(r => r.EmployeeId == dto.EmployeeId &&
                                r.LeaveType == LeaveType.Annual &&
                                r.StartDate.Year == currentYear)
                    .SumAsync(r => (r.EndDate - r.StartDate).Days + 1);

                int requestedDays = (dto.EndDate - dto.StartDate).Days + 1;

                if (totalAnnualDays + requestedDays > 20)
                {
                    return BadRequest("Exceeded annual leave limit (20 days).");
                }
            }

            var entity = mapper.Map<LeaveRequest>(dto);
            entity.CreatedAt = DateTime.Now;
            entity.Status = Status.Pending;

            context.LeaveRequests.Add(entity);
            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, "An error occurred while saving the leave request.");
            }

            return CreatedAtAction(nameof(Get), new { id = entity.Id }, mapper.Map<LeaveRequestDto>(entity));
        }



        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateLeaveRequestDto dto)
        {
            var leaveRequest = await context.LeaveRequests.FindAsync(id);
            if (leaveRequest == null) return NotFound();

            if (Enum.TryParse<LeaveType>(dto.LeaveType, true, out var leaveType) && leaveType == LeaveType.Sick && string.IsNullOrWhiteSpace(dto.Reason))
            {
                return BadRequest("Sick leave must include a reason.");
            }

            bool isOverlapping = await context.LeaveRequests.AnyAsync(r =>
                r.EmployeeId == dto.EmployeeId &&
                r.Id != id &&
                r.StartDate <= dto.EndDate &&
                r.EndDate >= dto.StartDate);

            if (isOverlapping)
            {
                return BadRequest("Leave request overlaps with existing request.");
            }

            if (Enum.TryParse<LeaveType>(dto.LeaveType, true, out var leaveTypeAnnual) && leaveTypeAnnual == LeaveType.Annual)
            {
                var currentYear = dto.StartDate.Year;
                var totalAnnualDays = await context.LeaveRequests
                    .Where(r => r.EmployeeId == dto.EmployeeId &&
                                r.LeaveType == LeaveType.Annual &&
                                r.StartDate.Year == currentYear &&
                                r.Id != id)
                    .SumAsync(r => (r.EndDate - r.StartDate).Days + 1);

                int requestedDays = (dto.EndDate - dto.StartDate).Days + 1;

                if (totalAnnualDays + requestedDays > 20)
                {
                    return BadRequest("Exceeded annual leave limit (20 days).");
                }
            }

            mapper.Map(dto, leaveRequest);
            await context.SaveChangesAsync();

            return NoContent();
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var leaveRequest = await context.LeaveRequests.FindAsync(id);
            if (leaveRequest == null) return NotFound();

            context.LeaveRequests.Remove(leaveRequest);
            await context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("filter")]
        public async Task<IActionResult> Filter([FromQuery] LeaveRequestFilterDto filterDto)
        {
            var query = context.LeaveRequests.AsQueryable();

            if (filterDto.EmployeeId.HasValue)
                query = query.Where(r => r.EmployeeId == filterDto.EmployeeId);

            if (!string.IsNullOrEmpty(filterDto.LeaveType) && Enum.TryParse<LeaveType>(filterDto.LeaveType, out var leaveTypeEnum))
                query = query.Where(r => r.LeaveType == leaveTypeEnum);

            if (!string.IsNullOrEmpty(filterDto.Status) && Enum.TryParse<Status>(filterDto.Status, out var statusEnum))
                query = query.Where(r => r.Status == statusEnum);

            if (filterDto.StartDate.HasValue)
                query = query.Where(r => r.StartDate >= filterDto.StartDate);

            if (filterDto.EndDate.HasValue)
                query = query.Where(r => r.EndDate <= filterDto.EndDate);

            if (!string.IsNullOrWhiteSpace(filterDto.Keyword))
                query = query.Where(r => r.Reason.Contains(filterDto.Keyword));

            var sortColumn = filterDto.SortBy ?? "CreatedAt";
            var sortOrder = filterDto.SortOrder?.ToLower() == "asc" ? "" : "descending";
            query = query.OrderBy($"{sortColumn} {sortOrder}");

            var skip = (filterDto.Page - 1) * filterDto.PageSize;
            var paginated = await query.Skip(skip).Take(filterDto.PageSize).ToListAsync();

            return Ok(mapper.Map<List<LeaveRequestDto>>(paginated));
        }

        [HttpGet("report")]
        public async Task<IActionResult> GetLeaveReport(int year, string department = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = context.LeaveRequests
                .Include(lr => lr.Employee)
                .Where(lr => lr.StartDate.Year == year);

            if (!string.IsNullOrEmpty(department))
            {
                query = query.Where(lr => lr.Employee.Department == department);
            }

            if (startDate.HasValue && endDate.HasValue)
            {
                query = query.Where(lr => lr.StartDate >= startDate && lr.EndDate <= endDate);
            }

            var report = await query
                .GroupBy(lr => new { lr.EmployeeId, lr.Employee.FullName })
                .Select(g => new LeaveReportDto
                {
                    EmployeeId = g.Key.EmployeeId,
                    EmployeeName = g.Key.FullName,
                    TotalLeaves = g.Count(),
                    AnnualLeaves = g.Count(lr => lr.LeaveType == LeaveType.Annual),
                    SickLeaves = g.Count(lr => lr.LeaveType == LeaveType.Sick)
                })
                .ToListAsync();

            return Ok(report);
        }

        [HttpPost("{id}/approve")]
        public async Task<IActionResult> ApproveLeaveRequest(int id)
        {
            var leaveRequest = await context.LeaveRequests.FindAsync(id);

            if (leaveRequest == null)
            {
                return NotFound("Leave request not found.");
            }

            if (leaveRequest.Status != Status.Pending)
            {
                return BadRequest("Only pending requests can be approved.");
            }

            leaveRequest.Status = Status.Approved;
            await context.SaveChangesAsync();

            var result = new LeaveApprovalResultDto
            {
                LeaveRequestId = leaveRequest.Id,
                Status = leaveRequest.Status.ToString()
            };

            return Ok(result);
        }

        [HttpPost("request")]
        public async Task<IActionResult> CreateLeaveRequest(CreateLeaveRequestDto dto)
        {
            if (!Enum.TryParse(dto.LeaveType, true, out LeaveType leaveType))
            {
                return BadRequest($"Invalid leave type: {dto.LeaveType}. It must be either 'Sick' or 'Annual'.");
            }

            var isValid = await validationContext.ValidateAsync(dto);
            if (!isValid)
            {
                return BadRequest("Invalid leave request.");
            }

            var leaveRequest = new LeaveRequest
            {
                EmployeeId = dto.EmployeeId,
                LeaveType = leaveType,  
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Reason = dto.Reason,
                Status = Status.Pending
            };

            await repository.AddAsync(leaveRequest);
            await repository.SaveChangesAsync();

            return Ok("Leave request created.");
        }

    }
}
