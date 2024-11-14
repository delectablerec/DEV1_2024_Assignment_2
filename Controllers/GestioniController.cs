using Microsoft.AspNetCore.Mvc;

public class GestioniController : Controller
{
    private readonly ApplicationDbContext _context;

    public GestioniController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        return View();
    }

    // GET: Mostra il form per aggiungere una categoria
    public IActionResult AggiungiCategoria()
    {
        return View();
    }

    // POST: Gestiscie la sottomisione del form
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult AggiungiCategoria(string nome)
    {
        if (ModelState.IsValid)
        {
            var categoria = new Categoria
            {
                Nome = nome
            };

            _context.Categorie.Add(categoria);
            _context.SaveChangesAsync();
            
            return RedirectToAction("Index");
        }

        return View();
    }

    public IActionResult AggiungiMarca()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult AggiungiMarca(string nome)
    {
        if (ModelState.IsValid)
        {
            var marca = new Marca
            {
                Nome = nome
            };

            _context.Marche.Add(marca);
            _context.SaveChangesAsync();
            
            return RedirectToAction("Index");
        }

        return View();
    }

    public IActionResult AggiungiMateriale()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult AggiungiMateriale(string nome)
    {
        if (ModelState.IsValid)
        {
            var materiale = new Materiale
            {
                Nome = nome
            };

            _context.Materiali.Add(materiale);
            _context.SaveChangesAsync();
            
            return RedirectToAction("Index"); 
        }

        return View();
    }

    public IActionResult AggiungiTipologia()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult AggiungiTipologia(string nome)
    {
        if (ModelState.IsValid)
        {
            var tipologia = new Tipologia
            {
                Nome = nome
            };

            _context.Tipologie.Add(tipologia);
            _context.SaveChangesAsync();
            
            return RedirectToAction("Index"); 
        }

        return View();
    }

    public IActionResult AggiungiGenere()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult AggiungiGenere(string nome)
    {
        if (ModelState.IsValid)
        {
            var genere = new Genere
            {
                Nome = nome
            };

            _context.Generi.Add(genere);
            _context.SaveChangesAsync();
            
            return RedirectToAction("Index"); 
        }

        return View();
    }
}
