namespace SmartApartmentData.Interfaces
{
    public interface ITokenService
    {
         public string GeneratedToken(User user);
    }
}