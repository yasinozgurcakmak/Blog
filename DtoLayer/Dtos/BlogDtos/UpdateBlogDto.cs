using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace DtoLayer.Dtos.BlogDtos;

public class UpdateBlogDto
{
    [Required]
    public int Id { get; set; }

    [Required(ErrorMessage = "Blog başlığı zorunludur.")]
    [StringLength(200, MinimumLength = 3, ErrorMessage = "Başlık 3 ile 200 karakter arasında olmalıdır.")]
    public string Title { get; set; } = string.Empty;

    public string? Slug { get; set; }

    [Required(ErrorMessage = "Özet alanı zorunludur.")]
    [MaxLength(500, ErrorMessage = "Özet en fazla 500 karakter olabilir.")]
    public string Summary { get; set; } = string.Empty;

    [Required(ErrorMessage = "Blog içeriği zorunludur.")]
    public string Content { get; set; } = string.Empty;

    [Required(ErrorMessage = "Lütfen bir kategori seçiniz.")]
    [Range(1, int.MaxValue, ErrorMessage = "Geçerli bir kategori seçiniz.")]
    public int CategoryId { get; set; }

    public string? MetaTitle { get; set; }

    public string? MetaDescription { get; set; }

    public bool Status { get; set; }

    public DateTime? PublishDate { get; set; }

    public string? ExistingThumbnail { get; set; }

    public IFormFile? NewThumbnailFile { get; set; }
}
