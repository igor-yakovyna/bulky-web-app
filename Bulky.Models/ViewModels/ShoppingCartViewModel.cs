namespace BulkyBook.Models.ViewModels;

public class ShoppingCartViewModel
{
    public IEnumerable<ShoppingCart> ShoppingCarts { get; set; }

    public double OrderTotal { get; set; }
}