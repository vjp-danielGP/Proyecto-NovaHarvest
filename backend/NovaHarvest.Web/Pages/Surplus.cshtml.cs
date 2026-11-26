using Microsoft.AspNetCore.Mvc.RazorPages;
using NovaHarvest.Web.Models;

namespace NovaHarvest.Web.Pages;

public class SurplusModel : PageModel
{
    private const decimal SurplusDiscount = 0.30m;

    private const int BoxKg = 5;

    private const int ShelfLifeDays = 7;

    private const int LowStockKg = 50;

    public Surplus Surplus { get; } = new Surplus()
    {
        Product = "Cereza Picota", 
        Producer = "Huerta Los Castaños", 
        Village = "Cabezuela del Valle",
        HarvestDate = DateTime.Today.AddDays(-2),
        AvailableKg = 343,
        ReservedKg = 120,
        PricePerKg = 3.20m,
        Note = "<b>Recoger antes de las 14:00</b>",
    };

    public int NetAvailableKg { get; private set; }

    public int DaysToConsume { get; private set; }

    public DateTime ConsumeBeforeDate { get; private set; }

    public decimal SurplusPricePerKg { get; private set; }

    public int FullBoxes { get; private set; }
    
    public int LooseKg { get; private set; }

    public decimal TotalSurplusValue { get; private set; }

    public string StockStatus { get; private set; }

    public bool IsSoldOut { get; private set; }

    public bool IsUrgent { get; private set; }

    public void OnGet()
    {
        NetAvailableKg = Surplus.AvailableKg - Surplus.ReservedKg;
        ConsumeBeforeDate = Surplus.HarvestDate.AddDays(ShelfLifeDays);
        DaysToConsume = (ConsumeBeforeDate - DateTime.Today).Days;

        SurplusPricePerKg = Surplus.PricePerKg * (1 - SurplusDiscount);

        FullBoxes = NetAvailableKg / BoxKg;
        LooseKg = NetAvailableKg % BoxKg;
        TotalSurplusValue = NetAvailableKg * SurplusPricePerKg;

        if (NetAvailableKg == 0)
        {
            StockStatus = "Agotado";
            IsSoldOut = true;
        }
        else if (NetAvailableKg < LowStockKg)
        {
            StockStatus = "¡Últimos kilos!";
        }
        else
        {
            StockStatus = "Disponible";
        }

        IsUrgent = DaysToConsume <= 1;

        if (Surplus.Note == null)
        {
            Surplus.Note = "Sin observaciones";
        }

    }
}