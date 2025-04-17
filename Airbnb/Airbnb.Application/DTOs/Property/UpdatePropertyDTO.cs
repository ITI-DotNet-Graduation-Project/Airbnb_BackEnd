using Airbnb.Application.DTOs.Property;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

public class UpdatePropertyDTO
{
    [Required]
    public int Id { get; set; }

    [Required(ErrorMessage = "Title is required")]
    public string Title { get; set; }

    [Required(ErrorMessage = "Description is required")]
    public string Description { get; set; }

    [Required(ErrorMessage = "Location is required")]
    public string Location { get; set; }

    [Required(ErrorMessage = "Price is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Bedrooms is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Bedrooms must be at least 1")]
    public int Bedrooms { get; set; }

    [Required(ErrorMessage = "Bathrooms is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Bathrooms must be at least 1")]
    public int Bathrooms { get; set; }

    [Required(ErrorMessage = "PropertyType is required")]
    public string PropertyType { get; set; }


    [Required]
    public int MaxGuest { get; set; }


    [Required(ErrorMessage = "UserId is required")]
    public string UserId { get; set; }

    public List<string> DeletedImageIds { get; set; } = new List<string>();
    public List<IFormFile> NewImages { get; set; } = new List<IFormFile>();
    public string AvailabilitiesJson { get; set; }

    public List<AvailabilityDto> Availabilities { get; set; } = new();
}