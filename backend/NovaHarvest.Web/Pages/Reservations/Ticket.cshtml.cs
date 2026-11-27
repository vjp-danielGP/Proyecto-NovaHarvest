using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class TicketModel : PageModel
{
    public Ticket Ticket { get; } = new Ticket()
    {
        Product = "Cereza Picota",
        Kg = 20,
        PickupLocation = "Cooperativa del Valle del Jerte",
    };
}