// public class LayoutViewModel
// {
//     public int CartItemCount { get; set; }
    
//     public void ItemsInCart()
//     {
//         var userId = _userManager.GetUserId(User);
//         if (string.IsNullOrEmpty(userId))
//         {
//             ViewData["CartItemCount"] = 0;
//         }

//         var carrello = _carrelloService.CaricaCarrello(userId);

//         if (carrello == null || carrello.Carrello.Count == 0)
//         {
//             ViewData["CartItemCount"] = 0;
//         }
//         else
//         {
//             ViewData["CartItemCount"] = carrello.Carrello.Count;
//             CartItemCount = carrello.Carrello.Count;
//         }
//     }
// }