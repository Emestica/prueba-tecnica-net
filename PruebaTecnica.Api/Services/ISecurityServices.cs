namespace PruebaTecnica.Api.Services
{
    public interface ISecurityServices
    {
        string? CreateCredentials(string password);
        bool VerifyPassword(string passwordEncript, string password);
    }
}
