namespace OrdiniTest.ViewModels
{
    public class OrdineDettaglioCombinedViewModel
    {
        public DettaglioOrdineViewModel OrdineDettaglio { get; set; } = null!;
        public List<DettaglioOrdineProdottoViewModel> ProdottiDettaglio { get; set; } = new List<DettaglioOrdineProdottoViewModel>();
    }
}
