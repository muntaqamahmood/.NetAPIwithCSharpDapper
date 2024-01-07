namespace DotNetAPI.DTOs
{
    public partial class UserForLoginConfirmationDto
    {
        public byte[] PasswordHash { get; set; } = Array.Empty<byte>();
        public byte[] PasswordSalt { get; set; } = new byte[0];
    }
}