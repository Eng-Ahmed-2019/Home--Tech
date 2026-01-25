namespace HomeTech.Application.DTOs.Emails
{
    public class ConfirmEmailModel
    {
        public string FullName { get; set; } = default!;
        public string ConfirmLink { get; set; } = default!;
    }
}