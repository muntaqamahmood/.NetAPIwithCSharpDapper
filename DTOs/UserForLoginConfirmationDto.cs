namespace DotNetAPI.DTOs
{
    public partial class UserForLoginConfirmationDto
    {
        byte[] PasswordHash { get; set; } = Array.Empty<byte>();
        byte[] PasswordSalt { get; set; } = new byte[0];
    }
}