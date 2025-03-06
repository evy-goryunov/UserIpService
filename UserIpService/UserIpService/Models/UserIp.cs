namespace UserIpService.Models;

public class UserIp
{
    public long UserId { get; set; }
    
    public virtual User User { get; set; }
    
    public string IpAddressId { get; set; } // Связь с IpAddress
    
    public virtual IpAddress IpAddress { get; set; }
    
    public DateTime ConnectionTime { get; set; } = DateTime.UtcNow; // Время подключения
}