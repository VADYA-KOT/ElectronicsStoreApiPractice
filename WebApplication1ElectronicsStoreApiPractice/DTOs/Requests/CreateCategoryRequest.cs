using System.ComponentModel.DataAnnotations;

namespace ElectronicsStoreApiPractice.DTOs.Requests;

public class CreateCategoryRequest
{
    [Range(1, int.MaxValue)]
    public int Id { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;
}