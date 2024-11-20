using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class GestioniController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public GestioniController(ApplicationDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
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
            _context.SaveChanges();

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
            _context.SaveChanges();

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
            _context.SaveChanges();

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
            _context.SaveChanges();

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
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        return View();
    }


    public IActionResult ModificaCarousel()
    {
        // Se la tabella è vuota, popola con i dati attuali
        if (!_context.CarouselImages.Any())
        {
            var initialImages = new List<CarouselImage>
        {
            new CarouselImage
            {
                ImagePath = "./immaginiProdotti/Omega2.jpg",
                Title = "Collezione di Lusso",
                Description = "Scopri l'eleganza dei nostri orologi esclusivi"
            },
            new CarouselImage
            {
                ImagePath = "./immaginiProdotti/Rolex5.jpg",
                Title = "Edizioni Limitate",
                Description = "Pezzi unici che raccontano storie di eccellenza"
            },
            new CarouselImage
            {
                ImagePath = "./immaginiProdotti/patekPhilippe2.jpg",
                Title = "Precisione e Design",
                Description = "L'arte dell'orologeria al massimo livello"
            }
        };

            _context.CarouselImages.AddRange(initialImages);
            _context.SaveChanges();
        }

        var carouselImages = _context.CarouselImages.ToList();
        return View(carouselImages);
    }
    
    [HttpPost]
    public async Task<IActionResult> ModificaCarousel(
    List<IFormFile> ImmaginiCaricate,
    List<string> ImagePaths,
    List<string> Titles,
    List<string> Descriptions)
    {
        for (int i = 0; i < ImmaginiCaricate.Count; i++)
        {
            var carouselImage = await _context.CarouselImages.FirstOrDefaultAsync(c => c.Id == i + 1);
            if (carouselImage != null)
            {
                // Gestione immagine
                if (ImmaginiCaricate[i] != null && ImmaginiCaricate[i].Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImmaginiCaricate[i].FileName);
                    var filePath = Path.Combine(_environment.WebRootPath, "immaginiProdotti", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImmaginiCaricate[i].CopyToAsync(stream);
                    }

                    // Rimuovi il vecchio file se esiste
                    if (!string.IsNullOrEmpty(carouselImage.ImagePath))
                    {
                        var oldFilePath = Path.Combine(_environment.WebRootPath, carouselImage.ImagePath.TrimStart('/'));
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }

                    carouselImage.ImagePath = $"/immaginiProdotti/{fileName}";
                }
                else if (!string.IsNullOrEmpty(ImagePaths[i]))
                {
                    carouselImage.ImagePath = ImagePaths[i];
                }

                // Aggiorna titolo e descrizione
                carouselImage.Title = Titles[i];
                carouselImage.Description = Descriptions[i];
            }

            await _context.SaveChangesAsync();
        }

        return RedirectToAction("Index", "Home");
    }
}
