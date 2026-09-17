using Microsoft.AspNetCore.Mvc;
using TatumBackendApi.Common.Models;
using TatumBackendApi.Entities;

namespace TatumBackendApi.DTOs;

public class BillerQueryParameters : PaginationParameters
{
    [FromQuery(Name = "category")]
    public BillerCategory? Category { get; set; }
}
