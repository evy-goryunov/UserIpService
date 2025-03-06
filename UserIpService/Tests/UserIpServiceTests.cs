using Microsoft.EntityFrameworkCore.InMemory;
using Microsoft.EntityFrameworkCore;
using UserIpService.Database;
using UserIpService.Services;

namespace Tests
{
    public class UserIpServiceTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly UserIpMainService _service;

        public UserIpServiceTests()
        {
            // Настройка in-memory базы данных
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Уникальное имя для каждой тестовой базы
            .Options;

            _context = new AppDbContext(options);
            _context.Database.EnsureCreated(); // Создаем базу данных

            _service = new UserIpMainService(_context);
        }

        // Очистка базы данных после каждого теста
        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact]
        public async Task AddUserIp_ShouldAddUserAndIpAddressIfNotExists()
        {
            // Act
            await _service.AddUserIp(1, "192.168.1.1");

            // Assert
            Assert.True(_context.Users.Any(u => u.Id == 1));
            Assert.True(_context.IpAddresses.Any(ip => ip.Address == "192.168.1.1"));
            Assert.True(_context.UserIps.Any(ui => ui.UserId == 1 && ui.IpAddressId == "192.168.1.1"));
        }

        [Fact]
        public async Task AddUserIp_ShouldNotAddDuplicateUserIpEntry()
        {
            // Arrange
            await _service.AddUserIp(1, "192.168.1.1");
            var initialCount = _context.UserIps.Count();

            // Act
            await _service.AddUserIp(1, "192.168.1.1"); // Добавляем тот же самый UserIp

            // Assert
            Assert.Equal(initialCount, _context.UserIps.Count()); // Количество не должно измениться
        }

        [Fact]
        public async Task FindUsersByIpPrefix_ShouldReturnUsersWithMatchingPrefix()
        {
            // Arrange
            await _service.AddUserIp(1, "192.168.1.1");
            await _service.AddUserIp(2, "192.168.1.2");
            await _service.AddUserIp(3, "10.0.0.1");

            // Act
            var users = await _service.FindUsersByIpPrefix("192.168");

            // Assert
            Assert.Contains(1, users);
            Assert.Contains(2, users);
            Assert.DoesNotContain(3, users);
            Assert.Equal(2, users.Count);
        }

        [Fact]
        public async Task GetUserIpAddresses_ShouldReturnAllIpAddressesForUser()
        {
            // Arrange
            await _service.AddUserIp(1, "192.168.1.1");
            await _service.AddUserIp(1, "10.0.0.1");
            await _service.AddUserIp(2, "172.16.0.1");

            // Act
            var ips = await _service.GetUserIpAddresses(1);

            // Assert
            Assert.Contains("192.168.1.1", ips);
            Assert.Contains("10.0.0.1", ips);
            Assert.DoesNotContain("172.16.0.1", ips);
            Assert.Equal(2, ips.Count);
        }

        [Fact]
        public async Task GetLastUserConnection_ShouldReturnLastIpAddressAndConnectionTime()
        {
            // Arrange
            var now = DateTime.UtcNow;
            await _service.AddUserIp(1, "192.168.1.1");
            await Task.Delay(100); //  Чтобы время добавления отличалось
            await _service.AddUserIp(1, "10.0.0.1");

            // Act
            var lastConnection = await _service.GetLastUserConnection(1);

            // Assert
            Assert.Equal("10.0.0.1", lastConnection.IpAddress);
            Assert.True(lastConnection.ConnectionTime > now); // Проверяем, что время больше начального
        }

        [Fact]
        public async Task GetLastUserConnection_ShouldReturnNullIfNoConnectionsExist()
        {
            // Act
            var lastConnection = await _service.GetLastUserConnection(1);

            // Assert
            Assert.Null(lastConnection.IpAddress);
            Assert.Equal(DateTime.MinValue, lastConnection.ConnectionTime);
        }
    }
}
        