using System.ComponentModel.DataAnnotations;

namespace SukotSystemCore.DTOs.AdminDTOs.Request
{
    // Self-service profile edit only (PUT /admins/me). Per the user's explicit scope choice for
    // this entity: no self-registration, no roster listing, no admin-creates-admin - the only
    // thing an Admin can do to this entity is view/edit their own FullName/Phone. Never
    // PasswordHash (same reasoning as every other Update DTO in this project) and never Id.
    public class AdminUpdateDTO
    {
        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required]
        public string Phone { get; set; } = string.Empty;
    }
}
