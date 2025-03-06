using System.ComponentModel.DataAnnotations;

namespace UserIpService.Models;

public class IpAddress
{
    [Key]
    public string Address { get; set; }

    public virtual List<UserIp> UserIps { get; set; } = new();
}