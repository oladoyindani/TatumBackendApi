using TatumBackendApi.Entities;

namespace TatumBackendApi.DTOs;

public class BillerDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public BillerCategory Category { get; set; }
    public bool IsActive { get; set; }
}
