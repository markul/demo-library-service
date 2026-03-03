using System.ComponentModel.DataAnnotations;

namespace LibraryService.Domain.Entities;

public class ClientAddress
{
    public Guid Id { get; set; }
    
    public Guid ClientId { get; set; }
    
    public string City { get; set; } = string.Empty;
    
    public string Country { get; set; } = string.Empty;
    
    public string Address { get; set; } = string.Empty;
    
    public string PostalCode { get; set; } = string.Empty;
    
    public Client Client { get; set; } = null!;
}
