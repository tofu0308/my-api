using MyApi.Models;
using System.ComponentModel.DataAnnotations;


namespace angular_azure_demo.Models
{
    public class UpdateStatusRequest
    {
        [Required]
        [Range(0, 3, ErrorMessage = "Status must be between 0 and 3.")]
        public MemoStatus Status { get; set; }
    }
}