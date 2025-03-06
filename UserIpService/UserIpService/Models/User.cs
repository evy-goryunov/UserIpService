using System.ComponentModel.DataAnnotations;

namespace UserIpService.Models;

public class User
{
    [Key]
    public long Id { get; set; }

    public virtual List<UserIp> UserIps { get; set; } = new();
}