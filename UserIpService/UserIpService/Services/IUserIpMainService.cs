namespace UserIpService.Services;

public interface IUserIpMainService
{
    Task AddUserIp(long userId, string ipAddress);
    Task<List<long>> FindUsersByIpPrefix(string ipPrefix);
    Task<List<string>> GetUserIpAddresses(long userId);
    Task<(string IpAddress, DateTime ConnectionTime)> GetLastUserConnection(long userId);
}