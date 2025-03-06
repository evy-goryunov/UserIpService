using Microsoft.EntityFrameworkCore;
using UserIpService.Database;
using UserIpService.Models;

namespace UserIpService.Services;

    public class UserIpService : IUserIpService
    {
        private readonly AppDbContext _context;

        public UserIpService(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddUserIp(long userId, string ipAddress)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                user = new User { Id = userId };
                _context.Users.Add(user);
            }

            var ip = await _context.IpAddresses.FindAsync(ipAddress);
            if (ip == null)
            {
                ip = new IpAddress { Address = ipAddress };
                _context.IpAddresses.Add(ip);
            }

            var userIp = new UserIp { UserId = userId, IpAddressId = ipAddress };

            bool alreadyExists = await _context.UserIps.AnyAsync(ui => ui.UserId == userId && ui.IpAddressId == ipAddress);

            if (!alreadyExists)
            {
                _context.UserIps.Add(userIp);
            }

            await _context.SaveChangesAsync();
        }

        public async Task<List<long>> FindUsersByIpPrefix(string ipPrefix)
        {
            return await _context.IpAddresses
                .Where(ip => ip.Address.StartsWith(ipPrefix))
                .SelectMany(ip => ip.UserIps.Select(ui => ui.UserId))
                .Distinct()
                .ToListAsync();
        }

        public async Task<List<string>> GetUserIpAddresses(long userId)
        {
            return await _context.UserIps
                .Where(ui => ui.UserId == userId)
                .Select(ui => ui.IpAddressId)
                .ToListAsync();
        }

        public async Task<(string IpAddress, DateTime ConnectionTime)> GetLastUserConnection(long userId)
        {
            var lastConnection = await _context.UserIps
                .Where(ui => ui.UserId == userId)
                .OrderByDescending(ui => ui.ConnectionTime)
                .FirstOrDefaultAsync();

            if (lastConnection == null)
            {
                return (null, DateTime.MinValue);
            }

            return (lastConnection.IpAddressId, lastConnection.ConnectionTime);
        }
    }