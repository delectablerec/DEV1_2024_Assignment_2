# WatchStoreApp

## PIANIFICAZIONE

<details>
<summary>Steps</summary>

- [x] Identificazione delle pagine necessarie alla web app
- [x] Identificazione dei ViewModel per ogni pagina
- [x] Identificazione delle proprietà necessarie per ogni ViewModel
- [x] Decisione del tipo di utenti
- [x] Stabilire le diverse visualizzazione a seconda del tipo di utente
- [x] Identificazione del posizionamento dei link
- [x] Creazione layout senza logiche backend
- [x] Implementazione delle partialViews
- [x] Inizializzare l'archetico della WebApp
- - [x] dotnet new mvc -o WatchStoreApp --auth Individual
- - [x] dotnet add package Microsoft.EntityFrameworkCore.Sqlite
- - [x] dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore
- - [x] Assicurarsi che ApplicationDbContext erediti da IdentityDbContext
- - [x] dotnet ef migrations add InitialCreate
- - [x] dotnet ef database update
- - [x] Aggiunte a program.cs per la creazione dei ruoli
- [x] Creare git.ignore e aggiungere progetto alla sln
- [x] Effettuare lo scaffolding delle pagine entity che si desidera personalizzare
- - [x] dotnet add package Microsoft.VisualStudio.Web.CodeGeneration.Design
- - [x] dotnet tool install -g dotnet-aspnet-codegenerator
- - [x] dotnet aspnet-codegenerator identity -dc ApplicationDbContext --files "Account.Login;Account.Logout;Account.Register;Account.Manage.Index"
- [x] Creazione dei modelli base relativi ai prodotti
- [x] Creazione dei DBset e update del database
- [x] Creazione di una classe cliente personalizzata che eredita da IdentityUser
- [x] Update dei riferimenti a cliente in DBContext e program.cs
- [X] Creazione di una correlazione tra gli utenti e i prodotti tramite gli ordini
- [ ] Decisione degli stili condivisi con css
- [x] Listare i metodi necessari per ogni pagina
- [ ] Conservazione di fonti multimediali (loghi, fonts, video ecc)
- [ ] Controllare la presenza di CDN e pacchetti da installare
- [x] Decisione della lingua
- [x] Decisione dello standard del codice e dei commenti
- [x] Divisione del lavoro su più branch

</details>

<details>
<summary>FLOWCHART</summary>

```mermaid

```

</details>

## Pagine

- HOME
<details>
<summary>Descrizione</summary>

- Visualizzazione di un carousel di cards con gli ultimi arrivi

</details>

<details>
<summary>ViewModel</summary>

```C#
public class HomeViewModel
{
    public List<Orologio> Orologi { get; set; }
    public List<Categoria> Categorie { get; set; }
    public List<Marca> Marche { get; set; }
    public List<Materiale> Materiali { get; set; }
    public List<Tipologia> Tipologie { get; set; }
}
```

</details>

<details>
<summary>Metodi Controller</summary>

```C#
public IActionResult Index()
{
    return View();
}

public IActionResult Privacy()
{
    return View();
}

[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
public IActionResult Error()
{
    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
}
```

</details>

<details>
<summary>View</summary>

</details>

- PRODOTTI

<details>
<summary>Descrizione</summary>

VERSIONE NORMALE :

- Visualizzazione di tutti i prodotti inpaginati
- Filtro per prezzo
- Filtro per categoria
- Filtro per marca
- Filtro per materiale 
- Filtro per tipologia
- Filtro per Genere
- Su schermo grande i filtri saranno in una sidebar
- Su schermo piccolo i filtri saranno dei pulsanti in alto
- Ogni card ha un pulsante per aprire e aggiungere al carrello

AGGIUNTE ADMIN :

- Pulsante card per eliminare il prodotto
- Pulsante card per modificare il prodotto

</details>

<details>
<summary>Lista link</summary>

- Pagina dettaglio prodotto
- Pagina carrello


AGGIUNTE ADMIN : 

- Pagina elimina prodotto
- Pagina modifica prodotto

</details>

<details>
<summary>ViewModel</summary>

```C#
public class ProdottiViewModel
{
    public List<Orologio> Orologi { get; set; }
    public List<Categoria> Categorie { get; set; }
    public List<Marca> Marche { get; set; }
    public List<Materiale> Materiali { get; set; }
    public List<Tipologia> Tipologie { get; set; }
    public int ConteggioProdotti { get; set; }
    public int? CategoriaSelezionata { get; set; }
    public int? MarcaSelezionata { get; set; }
    public int? MaterialeSelezionato { get; set; }
    public int? TipologiaSelezionata { get; set; }
    public decimal MinPrezzo { get; set; }
    public decimal MaxPrezzo { get; set; }
    public int NumeroPagine { get; set; }
    public int PaginaCorrente { get; set; }
}
```

</details>

<details>
<summary>Metodi Controller</summary>

```c#
public IActionResult Index(int? minPrezzo, int? maxPrezzo, int? categoriaSelezionata, int? marcaSelezionata, int? materialeSelezionato, int? tipologiaSelezionata, int paginaCorrente = 1)
{
    const int prodottiPerPagina = 6;

    // Per assegnare le variabili passate al viewmodel
    var viewModel = _prodottiService.PreparaProdottiViewModel(
        minPrezzo, maxPrezzo, categoriaSelezionata, marcaSelezionata, materialeSelezionato, tipologiaSelezionata,
        paginaCorrente, prodottiPerPagina);

    return View(viewModel);
}
```

</details>

<details>
<summary>Metodi Service</summary>

```c#
public List<Orologio> FiltraProdotti(
        List<Orologio> prodottiTotali, int? minPrezzo, int? maxPrezzo, int? categoriaId, int? marcaId,
        int? materialeId, int? tipologiaId)
{
    List<Orologio> prodottiFiltrati = new List<Orologio>(); // Lista di prodotti filtrati
    bool scartato;

    foreach (var prodotto in prodottiTotali)
    {
        scartato = false;
        if(minPrezzo.HasValue && prodotto.Prezzo < minPrezzo.Value) // Se prezzo minore del prezzo minimo
        {
            scartato = true;
        }
        if(maxPrezzo.HasValue && prodotto.Prezzo > maxPrezzo.Value) // Se prezzo maggiore di prezzo massimo
        {
            scartato = true;
        }
        if(categoriaId.HasValue && prodotto.CategoriaId != categoriaId) // Se la categoria non corrisponde a quella selezionata
        {
            scartato = true;
        }
        if(marcaId.HasValue && prodotto.MarcaId != marcaId)
        {
            scartato = true;
        }
        if(materialeId.HasValue && prodotto.MaterialeId != materialeId)
        {
            scartato = true;
        }
        if(tipologiaId.HasValue && prodotto.TipologiaId != tipologiaId)
        {
            scartato = true;    // Scarta il prodotto
        }
        if(scartato == false)   // Se prodotto non scartato
        {
            prodottiFiltrati.Add(prodotto); // Lo aggiunge alla lista
        }
    }
    return prodottiFiltrati;
}

public List<Orologio> OrdinaPerPrezzo(List<Orologio> prodotti)
{
    // Bubble Sort per ordinare i prodotti per prezzo
    for (int i = 0; i < prodotti.Count - 1; i++)
    {
        for (int j = 0; j < prodotti.Count - 1 - i; j++)
        {
            if (prodotti[j].Prezzo > prodotti[j + 1].Prezzo)
            {
                // Inverte gli elementi se sono nell'ordine sbagliato
                var temp = prodotti[j];
                prodotti[j] = prodotti[j + 1];
                prodotti[j + 1] = temp;
            }
        }
    }

    return prodotti;
}

public ProdottiViewModel PreparaProdottiViewModel(
    int? minPrezzo, int? maxPrezzo, int? categoriaId, int? marcaId, int? materialeId, int? tipologiaId, 
    int paginaCorrente, int prodottiPerPagina)
{
    var categorie = CaricaCategorie();
    var marche = CaricaMarche();
    var materiali = CaricaMateriali();
    var tipologie = CaricaTipologie();

    var prodottiTotali = CaricaProdotti();
    var prodottiFiltrati = FiltraProdotti(prodottiTotali, minPrezzo, maxPrezzo, categoriaId, marcaId, materialeId, tipologiaId);

    int quantitaProdotti = prodottiFiltrati.Count;  // Conteggio dei prodotti filtrati
    int numeroPagine = (int)Math.Ceiling((double)quantitaProdotti / prodottiPerPagina); // Numero pagine in base a quantità prodotti

    var prodottiOrdinati = OrdinaPerPrezzo(prodottiFiltrati);   // Per non usare .OrderBy(p => p.Prezzo)

    var prodottiPaginati = prodottiOrdinati // Inpaginazione dei prodotti
        .Skip((paginaCorrente - 1) * prodottiPerPagina)
        .Take(prodottiPerPagina)
        .ToList();

    return new ProdottiViewModel
    {
        Orologi = prodottiPaginati,
        MinPrezzo = minPrezzo ?? 0,
        MaxPrezzo = maxPrezzo ?? (prodottiFiltrati.Any() ? prodottiFiltrati.Max(p => p.Prezzo) : 0),    // Per problema pagina vuota per nessuna corrispondenza filtro
        NumeroPagine = numeroPagine,
        PaginaCorrente = paginaCorrente,
        Categorie = categorie,
        Marche = marche,
        Materiali = materiali,
        Tipologie = tipologie,
        CategoriaSelezionata = categoriaId,
        MarcaSelezionata = marcaId,
        MaterialeSelezionato = materialeId,
        TipologiaSelezionata = tipologiaId,
        ConteggioProdotti = quantitaProdotti
    };
}

```
</details>

<details>
<summary>View</summary>

</details>

- AGGIUNGI PRODOTTO

<details>
<summary>Descrizione</summary>

SOLO ADMIN :

- Permette di visualizzare un form per aggiungere un nuovo prodotto
- Molte caratteristiche potranno essere inserite tramite menu a tendina per poter accedere ad un elenco di una determinata proprietà

</details>

<details>
<summary>Lista link</summary>

- Pagina prodotti

</details>

<details>
<summary>ViewModel</summary>

```C#
public class AggiungiProdottoViewModel
{
    public Orologio Orologio { get; set; }
    public List<Categoria> Categorie { get; set; }
    public List<Marca> Marche { get; set; }
    public List<Materiale> Materiali { get; set; }
    public List<Tipologia> Tipologie { get; set; }
    public List<Genere> Generi { get; set; }
}
```

</details>

<details>
<summary>Metodi Controller</summary>

```c#
public IActionResult AggiungiProdotto()
{
    var viewModel = _prodottiService.PreparaAggiungiProdottoViewModel();
    return View(viewModel);
}

[HttpPost]
public IActionResult AggiungiProdotto(AggiungiProdottoViewModel viewModel)
{
    _logger.LogInformation("Tentativo di salvare il prodotto.");
    // Bool per prodotto salvato o meno
    var salvato = _prodottiService.SalvaProdotto(viewModel.Orologio);

    if (!salvato)
    {
        return View("Error");
    }

    _logger.LogInformation("Prodotto salvato. Redirecting a Index.");
    return RedirectToAction("Index", "Home");
}
```
</details>

<details>
<summary>Metodi Service</summary>

```c#
public AggiungiProdottoViewModel PreparaAggiungiProdottoViewModel()
{
    return new AggiungiProdottoViewModel
    {
        Orologio = new Orologio(),
        Categorie = CaricaCategorie(),
        Marche = CaricaMarche(),
        Materiali = CaricaMateriali(),
        Tipologie = CaricaTipologie(),
        Generi = CaricaGeneri()
    };
}

public bool SalvaProdotto(Orologio orologio)
{
    try
    {
        _context.Orologi.Add(orologio); // Aggiunge prodotto al database
        _context.SaveChanges();
        _logger.LogInformation("Product successfully saved.");
        return true;
    }
    catch (Exception ex)
    {
        _logger.LogError($"Error while saving the product: {ex.Message}", ex);
        return false;
    }
}
```
</details>

<details>
<summary>View</summary>

```html
@model AggiungiProdottoViewModel
@{
    ViewData["Title"] = "Aggiungi Prodotto";
}

<div class="container my-4 py-4 bg-dark text-light rounded shadow-sm">
    <div class="row">
        <div class="col-lg-9 mx-auto rounded border p-4">
            <h1 class="text-center mb-5">@ViewData["Title"]</h1>

            <form asp-action="AggiungiProdotto" method="post" class="g-3">

                <div class="form-group row mb-3">
                    <label asp-for="Orologio.Nome" class="col-sm-4 control-label">Nome</label>
                    <div class="col-sm-8">
                        <input class="form-control bg-dark text-light border-secondary" asp-for="Orologio.Nome" placeholder="nome">
                        <!--<span asp-validation-for="Orologio.Nome" class="text-danger"></span>-->
                    </div>
                </div>

                <div class="form-group row mb-3">
                    <label class="col-sm-4 control-label">Prezzo</label>
                    <div class="col-sm-8">
                        <input class="form-control bg-dark text-light border-secondary" asp-for="Orologio.Prezzo" placeholder="prezzo">
                        <!--<span asp-validation-for="Orologio.Prezzo" class="text-danger"></span>-->
                    </div>
                </div>

                <div class="form-group row mb-3">
                    <label class="col-sm-4 control-label">Giacenza</label>
                    <div class="col-sm-8">
                        <input class="form-control bg-dark text-light border-secondary" asp-for="Orologio.Giacenza" placeholder="giacenza">
                        <!--<span asp-validation-for="Orologio.Giacenza" class="text-danger"></span>-->
                    </div>
                </div>

                <div class="form-group row mb-3">
                    <label class="col-sm-4 control-label">Colore</label>
                    <div class="col-sm-8">
                        <input class="form-control bg-dark text-light border-secondary" asp-for="Orologio.Colore" placeholder="colore">
                        <!--<span asp-validation-for="Orologio.Colore" class="text-danger"></span>-->
                    </div>
                </div>

                <div class="form-group row mb-3">
                    <label class="col-sm-4 control-label">Immagine</label>
                    <div class="col-sm-8">
                        <input class="form-control bg-dark text-light border-secondary" asp-for="Orologio.UrlImmagine" placeholder="immagine">
                        <!--<span asp-validation-for="Orologio.UrlImmagine" class="text-danger"></span>-->
                    </div>
                </div>

                <div class="form-group row mb-3 center">
                    <label class="col-sm-4 control-label">Categoria</label>
                    <div class="col-sm-8">
                        <select class="form-select bg-dark text-light border-secondary" asp-for="Orologio.CategoriaId">
                            <option value="">Seleziona categoria</option>
                            @foreach (var categoria in Model.Categorie)
                            {
                                <option value="@categoria.Id">@categoria.Nome</option>
                            }
                        </select>
                        <!--<span asp-validation-for="Orologio.CategoriaId" class="text-danger"></span>-->
                    </div>
                </div>

                <div class="form-group row mb-3 center">
                    <label class="col-sm-4 control-label">Marca</label>
                    <div class="col-sm-8">
                        <select class="form-select bg-dark text-light border-secondary" asp-for="Orologio.MarcaId">
                            <option value="">Seleziona marca</option>
                            @foreach (var marca in Model.Marche)
                            {
                                <option value="@marca.Id">@marca.Nome</option>
                            }
                        </select>
                        <!--<span asp-validation-for="Orologio.MarcaId" class="text-danger"></span>-->
                    </div>
                </div>

                <div class="form-group row mb-3">
                    <label class="col-sm-4 control-label">Modello</label>
                    <div class="col-sm-8">
                        <input class="form-control bg-dark text-light border-secondary" asp-for="Orologio.Modello" placeholder="modello">
                        <!--<span asp-validation-for="Orologio.Modello" class="text-danger"></span>-->
                    </div>
                </div>

                <div class="form-group row mb-3">
                    <label class="col-sm-4 control-label">Referenza</label>
                    <div class="col-sm-8">
                        <input class="form-control bg-dark text-light border-secondary" asp-for="Orologio.Referenza" placeholder="referenza">
                        <!--<span asp-validation-for="Orologio.Referenza" class="text-danger"></span>-->
                    </div>
                </div>

                <div class="form-group row mb-3 center">
                    <label class="col-sm-4 control-label">Materiale</label>
                    <div class="col-sm-8">
                        <select class="form-select bg-dark text-light border-secondary" asp-for="Orologio.MaterialeId">
                            <option value="">Seleziona materiale</option>
                            @foreach (var materiale in Model.Materiali)
                            {
                                <option value="@materiale.Id">@materiale.Nome</option>
                            }
                        </select>
                        <!--<span asp-validation-for="Orologio.MaterialeId" class="text-danger"></span>-->
                    </div>
                </div>

                <div class="form-group row mb-3 center">
                    <label class="col-sm-4 control-label">Tipologia</label>
                    <div class="col-sm-8">
                        <select class="form-select bg-dark text-light border-secondary" asp-for="Orologio.TipologiaId">
                            <option value="">Seleziona tipologia</option>
                            @foreach (var tipologia in Model.Tipologie)
                            {
                                <option value="@tipologia.Id">@tipologia.Nome</option>
                            }
                        </select>
                        <!--<span asp-validation-for="Orologio.TipologiaId" class="text-danger"></span>-->
                    </div>
                </div>

                <div class="form-group row mb-3">
                    <label class="col-sm-4 control-label">Diametro</label>
                    <div class="col-sm-8">
                        <input class="form-control bg-dark text-light border-secondary" asp-for="Orologio.Diametro" placeholder="diametro">
                        <!--<span asp-validation-for="Orologio.Diametro" class="text-danger"></span>-->
                    </div>
                </div>

                <div class="form-group row mb-3 center">
                    <label class="col-sm-4 control-label">Genere</label>
                    <div class="col-sm-8">
                        <select class="form-select bg-dark text-light border-secondary" asp-for="Orologio.GenereId">
                            <option value="">Seleziona genere</option>
                            @foreach (var genere in Model.Generi)
                            {
                                <option value="@genere.Id">@genere.Nome</option>
                            }
                        </select>
                        <!--<span asp-validation-for="Orologio.GenereId" class="text-danger"></span>-->
                    </div>
                </div>

                <div class="row mt-4">
                    <div class="offset-sm-4 col-sm-8">
                        
                        <button type="submit" class="btn btn-custom w-100 rounded-pill">Submit</button>
                    </div>
                </div>
            </form>
        </div>
    </div>
</div>
```
</details>

- MODIFICA PRODOTTO

<details>
<summary>Descrizione</summary>

SOLO ADMIN :

- Permette di visualizzare tutte le caratteristiche di un prodotto e modificarle

</details>

<details>
<summary>Lista link</summary>

- Pagina prodotti

</details>

<details>
<summary>ViewModel</summary>

```C#
public class ModificaProdottoViewModel
{
    public Orologio Orologio { get; set; }
    public List<Categoria> Categorie { get; set; }
    public List<Marca> Marche { get; set; }
    public List<Materiale> Materiali { get; set; }
    public List<Tipologia> Tipologie { get; set; }
    public List<Genere> Generi { get; set; }
}
```

</details>

<details>
<summary>Metodi Controller</summary>

```c#
public IActionResult ModificaProdotto(int id)
{
    var viewModel = _prodottiService.PreparaModificaProdottoViewModel(id);

    if (viewModel == null)
    {
        return NotFound();
    }

    return View(viewModel);
}

[HttpPost]
public IActionResult ModificaProdotto(ModificaProdottoViewModel viewModel)
{
    _logger.LogInformation("Categoria selezionata: " + viewModel.Orologio.Categoria);

    var modificato = _prodottiService.ModificaProdotto(viewModel.Orologio);

    if (!modificato)
    {
        return StatusCode(500, "Errore durante il salvataggio del prodotto.");
    }

    _logger.LogInformation("Prodotto con ID: {Id} modificato. Redirecting to Index.", viewModel.Orologio.Id);
    return RedirectToAction("Index");
}
```
</details>

<details>
<summary>Metodi Service</summary>

```c#
public ModificaProdottoViewModel PreparaModificaProdottoViewModel(int id)
{
    var prodotti = CaricaProdotti();
    var prodotto = CercaProdottoPerId(prodotti, id);
    
    if (prodotto == null)
    {
        _logger.LogWarning("Prodotto con ID: {Id} non trovato.", id);
        return null;
    }

    return new ModificaProdottoViewModel
    {
        Orologio = prodotto,
        Categorie = CaricaCategorie(),
        Marche = CaricaMarche(),
        Materiali = CaricaMateriali(),
        Tipologie = CaricaTipologie(),
        Generi = CaricaGeneri()
    };
}

public bool ModificaProdotto(Orologio prodottoModificato)
{
    var prodottoEsistente = CercaProdottoPerId(_context.Orologi.ToList(), prodottoModificato.Id);

    if (prodottoEsistente == null)
    {
        _logger.LogWarning("Prodotto con ID: {Id} non trovato.", prodottoModificato.Id);
        return false;
    }

    prodottoEsistente.Nome = prodottoModificato.Nome;
    prodottoEsistente.Prezzo = prodottoModificato.Prezzo;
    prodottoEsistente.Giacenza = prodottoModificato.Giacenza;
    prodottoEsistente.Colore = prodottoModificato.Colore;
    prodottoEsistente.UrlImmagine = prodottoModificato.UrlImmagine;
    prodottoEsistente.CategoriaId = prodottoModificato.CategoriaId;
    prodottoEsistente.MarcaId = prodottoModificato.MarcaId;
    prodottoEsistente.Modello = prodottoModificato.Modello;
    prodottoEsistente.Referenza = prodottoModificato.Referenza;
    prodottoEsistente.MaterialeId = prodottoModificato.MaterialeId;
    prodottoEsistente.TipologiaId = prodottoModificato.TipologiaId;
    prodottoEsistente.Diametro = prodottoModificato.Diametro;
    prodottoEsistente.GenereId = prodottoModificato.GenereId;

    try
    {
        _context.SaveChanges();
        _logger.LogInformation("Prodotto con ID: {Id} modificato con successo.", prodottoModificato.Id);
        return true;
    }
    catch (Exception ex)
    {
        _logger.LogError($"Errore nel salvataggio del prodotto con ID: {prodottoModificato.Id} - {ex.Message}", ex);
        return false;
    }
}
```
</details>

- ELIMINA PRODOTTO

<details>
<summary>Descrizione</summary>

SOLO ADMIN :

- Permette di visualizzare le caratteristiche principali del prodotto e di eliminarlo

</details>

<details>
<summary>ViewModel</summary>

```C#
public class EliminaProdottoViewModel
{
    public Orologio Orologio {get; set;}
}
```

</details>

<details>
<summary>Metodi Controller</summary>

```c#
public IActionResult EliminaProdotto(int id)
{
    try
    {
        var viewModel = _prodottiService.PreparaEliminaProdottoViewModel(id);
        
        if (viewModel == null)
        {
            return NotFound();
        }
        
        return View(viewModel);
    }
    catch (Exception ex)
    {
        _logger.LogError($"Errore nella ricerca del prodotto: {ex.Message}");
        return StatusCode(500, "Qualcosa è andato storto duranrte la ricerca del prodotto.");
    }
}

[HttpPost, ActionName("EliminaProdotto")]
public IActionResult EliminaProdottoEseguito(int id)
{
    try
    {
        bool success = _prodottiService.EliminaProdotto(id);

        if (success)
        {
            _logger.LogInformation("Prodotto con ID {Id} cancellato con successo.", id);
            return RedirectToAction("Index");
        }
        else
        {
            return NotFound();
        }
    }
    catch (Exception ex)
    {
        _logger.LogError($"Errore nella cancellazione del prodotto: {ex.Message}");
        return StatusCode(500, "Qualcosa è andato storto durante la cancellazione del prodotto.");
    }
}
```
</details>

<details>
<summary>Metodi Service</summary>

```c#
public IActionResult EliminaProdotto(int id)
{
    try
    {
        var viewModel = _prodottiService.PreparaEliminaProdottoViewModel(id);
        
        if (viewModel == null)
        {
            return NotFound();
        }
        
        return View(viewModel);
    }
    catch (Exception ex)
    {
        _logger.LogError($"Errore nella ricerca del prodotto: {ex.Message}");
        return StatusCode(500, "Qualcosa è andato storto duranrte la ricerca del prodotto.");
    }
}

[HttpPost, ActionName("EliminaProdotto")]
public IActionResult EliminaProdottoEseguito(int id)
{
    try
    {
        bool success = _prodottiService.EliminaProdotto(id);

        if (success)
        {
            _logger.LogInformation("Prodotto con ID {Id} cancellato con successo.", id);
            return RedirectToAction("Index");
        }
        else
        {
            return NotFound();
        }
    }
    catch (Exception ex)
    {
        _logger.LogError($"Errore nella cancellazione del prodotto: {ex.Message}");
        return StatusCode(500, "Qualcosa è andato storto durante la cancellazione del prodotto.");
    }
}
```
</details>

<details>
<summary>View</summary>

```html
@model EliminaProdottoViewModel
@{
    ViewData["Title"] = "Cancella Prodotto";
}

<div class="container my-4 py-4 bg-dark text-light rounded shadow-sm">
    <h2 class="text-center">@ViewData["Title"]</h2>

    <form method="post" class="mt-4">
        <div class="row align-items-center">
            <!-- Image on the left -->
            <div class="col-sm-4">
                <img src="@Model.Orologio.UrlImmagine" alt="@Model.Orologio.Modello" class="img-fluid rounded">
            </div>

            <!-- Product Details on the right -->
            <div class="col-sm-8">
                <div class="form-group mb-3">
                    <p><strong>Marca:</strong> @Model.Orologio.Marca.Nome</p>
                    <p><strong>Modello:</strong> @Model.Orologio.Modello</p>
                    <p><strong>Prezzo:</strong> @Model.Orologio.Prezzo €</p>
                    <p><strong>Giacenza:</strong> @Model.Orologio.Giacenza</p>
                </div>

                <!-- Delete Button -->
                <div class="text-center mt-4">
                    <button type="submit" class="btn btn-danger w-auto rounded-pill">Cancella</button>
                </div>
            </div>
        </div>
    </form>
</div>
```

</details>

- DETTAGLIO PRODOTTO

<details>
<summary>Descrizione</summary>

- Permette di visualizzare tutti i dettagli specifici di un oggetto
- Contiene una descrizione aggiuntiva
- La pagina avrà una sezione con sfondo diverso per le specifiche tecniche
- Le specifiche saranno visualizzate tramite tab panels
- La pagina permetterà di aggiungere al carrello

</details>

<details>
<summary>Lista link</summary>

- Pagina carrello
- Pagina prodotti

</details>

<details>
<summary>ViewModel</summary>

```C#
public class DettaglioProdottoViewModel
{
    public Orologio Orologio {get; set;}
}
```

</details>

<details>
<summary>Metodi Controller</summary>

```c#
public IActionResult DettaglioProdotto(int id)
    {
        var viewModel = _prodottiService.PreparaDettaglioProdottoViewModel(id);

        if (viewModel == null)
        {
            return NotFound();
        }

        return View(viewModel);
    }
```

</details>

<details>
<summary>Metodi Service</summary>

```c#
public DettaglioProdottoViewModel PreparaDettaglioProdottoViewModel(int id)
{
    try
    {
        // Trova il prodotto nel database includendo gli oggetti che ha come proprietà
        var prodotto = CercaProdottoPerId(_context.Orologi   
                                                .Include(o => o.Categoria)
                                                .Include(o => o.Marca)
                                                .Include(o => o.Materiale)
                                                .Include(o => o.Tipologia)
                                                .Include(o => o.Genere)
                                                .ToList(),id);

        if (prodotto == null)
        {
            return null; // Return null se prodotto non trovato
        }

        var viewModel = new DettaglioProdottoViewModel
        {
            Orologio = prodotto
        };

        return viewModel;
    }
    catch (Exception ex)
    {
        _logger.LogError($"Errore nella preparazione del view model: {ex.Message}");
        return null; 
    }
}
```

</details>

<details>
<summary>View</summary>

```html
@model DettaglioProdottoViewModel
@{
    ViewData["Title"] = "Dettaglio Prodotto";
}

<div class="container my-5">
    <div class="row">
        <!-- Colonna immagine principale e galleria -->
        <div class="col-lg-5 d-flex flex-column align-items-center">
            <!-- Immagine principale del prodotto -->
            <img src="@Model.Orologio.UrlImmagine" alt="@Model.Orologio.Modello @Model.Orologio.Marca" class="img-fluid rounded mb-3" style="max-width: 100%; height: auto;">
        </div>

        <!-- Colonna dettagli del prodotto "appiccicosa" -->
        <div class="col-lg-5 sticky-column d-flex flex-column align-items-start">
            <h1 class="fw-bold">@Model.Orologio.Modello</h1>
            <p>Lorem ipsum dolor, sit amet consectetur adipisicing elit.</p>
        <!--     <a class="text-decoration-underline" style="color: #333;" data-bs-toggle="collapse" href="#moreText" role="button" aria-expanded="false" aria-controls="moreText">Leggi di più</a>-->

            <span id="moreText" style="display: none;">
                Lorem ipsum dolor sit amet consectetur adipisicing elit.
                Temporibus fuga deleniti voluptatem maiores necessitatibus autem corrupti,
                dolore iure. Nulla libero iure eaque, doloribus animi labore velit. Aliquam magni sed nihil.
            </span>
            <a href="javascript:void(0)" onclick="toggleText()" id="readMoreLink" style="color: #333;" class="text-decoration-underline">Leggi di più</a>
            
            <p class="fs-4 mt-3"><strong>@Model.Orologio.Prezzo</strong> <small class="text-muted">IVA incl.</small></p>

            @if(Model.Orologio.Giacenza >0)
            {
                <a asp-controller="Carrello" asp-action="AggiungiACarrello" asp-route-id="@Model.Orologio.Id" class="btn btn-dark btn-lg w-100 mb-3"> AGGIUNGI AL CARRELLO </a>
            }

            <!-- Opzioni aggiuntive -->
            <div class="mb-3">
                <p><i class="bi bi-telephone"></i> ORDINA AL TELEFONO (+39) 000000000</p>
            </div>

            <!-- Share e riferimento -->
            <div class="d-flex justify-content-between w-100">
                <a href="#" class="text-decoration-none"style="color: #333;"><i class="bi bi-share"></i> SHARE</a>
                <span class="text-muted">Ref. RGX10000</span>
            </div>
        </div>
    </div>

    <!-- Specifiche tecniche e nav tabs -->
    <div class="specifiche-tecniche">
        <h4>Specifiche</h4>
        <ul class="nav nav-tabs" id="myTab" role="tablist">
            <li class="nav-item" role="presentation">
                <button class="nav-link active" id="generiche-tab" data-bs-toggle="tab" data-bs-target="#generiche" type="button" role="tab" aria-controls="generiche" aria-selected="true">Generiche</button>
            </li>
            <li class="nav-item" role="presentation">
                <button class="nav-link" id="tecniche-tab" data-bs-toggle="tab" data-bs-target="#tecniche" type="button" role="tab" aria-controls="tecniche" aria-selected="false">Tecniche</button>
            </li>
            <li class="nav-item" role="presentation">
                <button class="nav-link" id="estetiche-tab" data-bs-toggle="tab" data-bs-target="#estetiche" type="button" role="tab" aria-controls="estetiche" aria-selected="false">Estetiche</button>
            </li>
        </ul>
        <div class="tab-content" id="myTabContent">
            <!-- Sezione Generica -->
            <div class="tab-pane fade show active p-3" id="generiche" role="tabpanel" aria-labelledby="generiche-tab">
                <div class="tech-info">
                    <div>
                        <h5>Categoria</h5>
                        <p>@Model.Orologio.Categoria.Nome</p>
                    </div>
                    <div>
                        <h5>Genere</h5>
                        <p>@Model.Orologio.Genere.Nome</p>
                    </div>
                    <div>
                        <h5>Marca</h5>
                        <p>@Model.Orologio.Marca.Nome</p>
                    </div>
                    <div>
                        <h5>Modello</h5>
                        <p>@Model.Orologio.Modello</p>
                    </div>
                </div>
            </div>

            <!-- Sezione Tecnica -->
            <div class="tab-pane fade p-3" id="tecniche" role="tabpanel" aria-labelledby="tecniche-tab">
                <div class="tech-info">
                    <div>
                        <h5>Referenza</h5>
                        <p>@Model.Orologio.Referenza</p>
                    </div>
                    <div>
                        <h5>Diametro</h5>
                        <p>@Model.Orologio.Diametro</p>
                    </div>
                    <div>
                        <h5>Tipologia</h5>
                        <p>@Model.Orologio.Tipologia.Nome</p>
                    </div>
                </div>
            </div>

            <!-- Sezione Estetica -->
            <div class="tab-pane fade p-3" id="estetiche" role="tabpanel" aria-labelledby="estetiche-tab">
                <div class="tech-info">
                    <div>
                        <h5>Materiale</h5>
                        <p>@Model.Orologio.Materiale.Nome</p>
                    </div>
                    <div>
                        <h5>Colore</h5>
                        <p>@Model.Orologio.Colore</p>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <!-- Sezione newsletter -->
    <div class="my-4 text-center">
        <h5>Iscriviti alla newsletter</h5>
        <form class="row justify-content-center g-3">
            <div class="col-auto">
                <label for="email" class="visually-hidden">Email</label>
                <input type="email" class="form-control" id="email" placeholder="Email">
            </div>
            <div class="col-auto">
                <button type="submit" class="btn btn-dark mb-3">Iscriviti</button>
            </div>
        </form>
    </div>
</div>
```

</details>

- LOGIN
- REGISTER

- CARRELLO

<details>
<summary>Descrizione</summary>

- Per vedere tutti gli ordini aggiunti al carrelo
- Per accedere alla conferma dell'ordine

</details>

<details>
<summary>Lista link</summary>

- Pagina conferma ordine

</details>

<details>
<summary>ViewModel</summary>

```C#
public class CarrelloViewModel
{
    public List<OrologioInCarrello> Carrello { get; set; }
    public decimal Totale { get; set; }
    public int Quantita { get; set; }
}
```

</details>

<details>
<summary>Metodi Controller</summary>

```c#
public IActionResult Index()
{
    var userId = _userManager.GetUserId(User);
    if (string.IsNullOrEmpty(userId))
    {
        _logger.LogError("User ID is null or empty.");
        return RedirectToAction("Index", "Home");
    }

    var carrello = _carrelloService.CaricaCarrello(userId);

    if (carrello == null || carrello.Carrello.Count == 0)
    {
        _logger.LogWarning("Carrello vuoto per UserId: {UserId}", userId);
    }
    else
    {
        _logger.LogInformation("Carrello caricato per UserId: {UserId}. Prodotti nel carrello: {Count}", userId, carrello.Carrello.Count);
    }

    return View(carrello);
}


public IActionResult AggiungiACarrello(int id)
{
    var userId = _userManager.GetUserId(User);
    _logger.LogInformation("UserId: {userId} sta aggiungendo un prodotto al carrello.", userId);

    var listaOrologi = _context.Orologi.ToList();
    if (listaOrologi == null || listaOrologi.Count == 0)
    {
        _logger.LogError("Lista vuota o nulla");
        return NotFound();
    }

    var orologio = _carrelloService.CercaProdottoPerId(listaOrologi, id);
    if (orologio == null)
    {
        _logger.LogWarning("Prodotto con ID: {IdProdotto} non trovato", id);
        return NotFound();
    }

    _logger.LogInformation("Prodotto trovato: {NomeProdotto}, Prezzo: {PrezzoProdotto}", orologio.Modello, orologio.Prezzo);

    // Usa il servizio per aggiornare il carrello
    var success = _carrelloService.AggiungiACarrello(userId, orologio);
    if (!success)
    {
        _logger.LogWarning("Giacenza del prodotto insufficiente {IdProdotto}", id);
        return RedirectToAction("Index", "Prodotti");
    }

    return RedirectToAction("Index");
}
public IActionResult RimuoviUnoDalCarrello(int id)
{
    var userId = _userManager.GetUserId(User);
    _logger.LogInformation("UserId: {userId} sta rimuovendo un prodotto dal carrello.", userId);

    var listaOrologi = _context.Orologi.ToList();
    if (listaOrologi == null || listaOrologi.Count == 0)
    {
        _logger.LogError("Lista vuota o nulla");
        return NotFound();
    }

    var orologio = _carrelloService.CercaProdottoPerId(listaOrologi, id);
    if (orologio == null)
    {
        _logger.LogWarning("Prodotto con ID: {IdProdotto} non trovato", id);
        return NotFound();
    }

    _logger.LogInformation("Prodotto trovato: {NomeProdotto}, Prezzo: {PrezzoProdotto}", orologio.Modello, orologio.Prezzo);

    // Usa il servizio per aggiornare il carrello
    _carrelloService.RimuoviUnoDalCarrello(userId, orologio);

    return RedirectToAction("Index");
}

public IActionResult RimuoviDalCarrello(int id)
{
    var userId = _userManager.GetUserId(User);
    if (string.IsNullOrEmpty(userId))
    {
        _logger.LogError("User ID è nullo o vuoto.");
        return RedirectToAction("Index", "Home");
    }

    // Chiama il metodo del servizio per rimuovere dal carrello
    var success = _carrelloService.RimuoviDalCarrello(userId, id);
    if (!success)
    {
        _logger.LogWarning("Errore nella rimozione del prodotto con ID: {IdProdotto} dal carrello", id);
        return RedirectToAction("Index");
    }

    return RedirectToAction("Index"); // Redirect al carrello dopo la rimozione
}
```
</details>

<details>
<summary>Metodi Service</summary>

```c#
public CarrelloViewModel CaricaCarrello(string userId)
{
    try
    {
        Dictionary<string, CarrelloViewModel> carrelliUtenti = new();

        // Controlla se il file json esiste e lo popola
        if (File.Exists(CartFilePath))
        {
            var json = File.ReadAllText(CartFilePath);
            carrelliUtenti = JsonConvert.DeserializeObject<Dictionary<string, CarrelloViewModel>>(json)
                    ?? new Dictionary<string, CarrelloViewModel>();
        }

        // Controlla se il carrello utente esiste
        if (carrelliUtenti.TryGetValue(userId, out var existingCart))
        {
            _logger.LogInformation("Cart loaded for UserId: {UserId}", userId);
            return existingCart; // Ritorna il carrello dell'utente
        }

        // Inizializza un nuovo carrello se l'utente non ne ha già uno
        _logger.LogInformation("No cart found for UserId: {UserId}. Initializing a new one.", userId);
        var newCart = new CarrelloViewModel
        {
            Carrello = new List<OrologioInCarrello>(),
            Totale = 0,
            Quantita = 0
        };

        // Aggiunge il carrello alla collezione
        carrelliUtenti[userId] = newCart;
        SalvaTuttiICarrelli(carrelliUtenti);

        return newCart;
    }
    catch (Exception ex)
    {
        _logger.LogError("Error loading cart for UserId: {UserId}. Exception: {Message}", userId, ex.Message);
        throw;
    }
}
public bool AggiungiACarrello(string userId, Orologio orologio)
{
    try
    {
        // Carica o inizializza un carrello
        var carrello = CaricaCarrello(userId);

        // Controlla che il prodotto sia in stock
        int stock = 0;
        foreach (var item in _context.Orologi)
        {
            if (item.Id == orologio.Id)
            {
                stock = item.Giacenza;
                break;
            }
        }

        if (stock <= 0)
        {
            _logger.LogWarning("Prodotto con ID: {IdProdotto} non è disponibile", orologio.Id);
            return false; //Ritorno anticipato se il prodotto non è in stock
        }

        // Cerca il prodotto nel carrello e lo assegna ad una nuova variabile
        OrologioInCarrello prodottoInCarrello = null;
        foreach (var item in carrello.Carrello)
        {
            if (item.OrologioId == orologio.Id)
            {
                prodottoInCarrello = item;
                break;
            }
        }

        // Se il prodotto è già presente nel carrello
        if (prodottoInCarrello != null)
        {
            prodottoInCarrello.QuantitaInCarrello++; // Aumenta quantità prodotto
            _logger.LogInformation("Prodotto aggiunto al carrello. Prodotto ID: {IdProdotto} Quantità: {QuantitaProdotto}", orologio.Id, prodottoInCarrello.QuantitaInCarrello);
        }
        else // Se il prodotto non c'è ancora
        {
            carrello.Carrello.Add(new OrologioInCarrello    // Crea una nuova istanza di orologio in carrello e la aggiunge
            {
                OrologioId = orologio.Id,
                Orologio = orologio,    // Setta la corrispondenza delle proprietà di orologio
                QuantitaInCarrello = 1  // Setta la quantità per la prima volta (a 1)
            });
            _logger.LogInformation("Prodotto aggiunto a carrello per la prima volta, Prodotto ID: {IdProdotto}", orologio.Id);
        }

        // Aggiorna totale e quantità
        decimal totale = 0;
        int quantita = 0;
        foreach (var item in carrello.Carrello)
        {
            totale += item.Orologio.Prezzo * item.QuantitaInCarrello;   // Tiene conto della quantità del prodotto
            quantita += item.QuantitaInCarrello;
        }
        carrello.Totale = totale;
        carrello.Quantita = quantita;

        _logger.LogInformation("Carrello aggiornato per UserID: {UserId}, Totale: {Totale}, Quantità Prodotti: {QuantitaProdotti}", userId, totale, quantita);

        // Diminuisce lo stock nel database del prodotto aggiunto al carrello
        orologio.Giacenza--;
        _context.SaveChanges();

        var carrelliUtenti = new Dictionary<string, CarrelloViewModel>();
        carrelliUtenti[userId] = carrello;
        // Salva tutti i carrelli con il nuovo prodotto nel carrello giusto
        SalvaTuttiICarrelli(carrelliUtenti);
        return true;
    }
    catch (Exception ex)
    {
        _logger.LogError("Errore durante l'aggiunta al carrello UserId: {UserId}, Id Prodotto: {IdProdotto}, Exception: {Message}", userId, orologio.Id, ex.Message);
        return false; // Failure in case of error
    }
}
public bool RimuoviUnoDalCarrello(string userId, Orologio orologio)
{
    try
    {
        // Carica o inizializza un carrello
        var carrello = CaricaCarrello(userId);

        // Cerca il prodotto nel carrello e lo assegna ad una nuova variabile
        OrologioInCarrello prodottoInCarrello = null;
        foreach (var item in carrello.Carrello)
        {
            if (item.OrologioId == orologio.Id)
            {
                prodottoInCarrello = item;
                break;
            }
        }

        // Se il prodotto è presente nel carrello
        if (prodottoInCarrello != null)
        {
            if (prodottoInCarrello.QuantitaInCarrello <= 1)
                RimuoviDalCarrello(userId, prodottoInCarrello.OrologioId);
            else
                {
                    prodottoInCarrello.QuantitaInCarrello--; // Riduci quantità prodotto
                _logger.LogInformation("Prodotto aggiunto al carrello. Prodotto ID: {IdProdotto} Quantità: {QuantitaProdotto}", orologio.Id, prodottoInCarrello.QuantitaInCarrello);
                }
        }

        // Aggiorna totale e quantità
        decimal totale = 0;
        int quantita = 0;
        foreach (var item in carrello.Carrello)
        {
            totale += item.Orologio.Prezzo * item.QuantitaInCarrello;   // Tiene conto della quantità del prodotto
            quantita += item.QuantitaInCarrello;
        }
        carrello.Totale = totale;
        carrello.Quantita = quantita;

        _logger.LogInformation("Carrello aggiornato per UserID: {UserId}, Totale: {Totale}, Quantità Prodotti: {QuantitaProdotti}", userId, totale, quantita);

        // Aumenta lo stock nel database del prodotto aggiunto al carrello
        orologio.Giacenza++;
        _context.SaveChanges();

        var carrelliUtenti = new Dictionary<string, CarrelloViewModel>();
        carrelliUtenti[userId] = carrello;
        // Salva tutti i carrelli con il nuovo prodotto nel carrello giusto
        SalvaTuttiICarrelli(carrelliUtenti);
        return true;
    }
    catch (Exception ex)
    {
        _logger.LogError("Errore durante la rimozione dal carrello UserId: {UserId}, Id Prodotto: {IdProdotto}, Exception: {Message}", userId, orologio.Id, ex.Message);
        return false; // Failure in case of error
    }
}

public bool RimuoviDalCarrello(string userId, int orologioId)
{
    try
    {
        // Carica o inizializza il carrello per l'utente
        var carrello = CaricaCarrello(userId);

        // Trova il prodotto nel carrello
        OrologioInCarrello prodottoInCarrello = null;
        foreach (var item in carrello.Carrello)
        {
            if (item.OrologioId == orologioId)
            {
                prodottoInCarrello = item;
                break;
            }
        }

        if (prodottoInCarrello == null)
        {
            _logger.LogWarning("Prodotto con ID: {IdProdotto} non trovato nel carrello per l'utente: {UserId}", orologioId, userId);
            return false; // Se il prodotto non è nel carrello, ritorna falso
        }

        // Aggiusta lo stock nel database per il prodotto rimosso
        var orologio = CercaProdottoPerId(_context.Orologi.ToList(), orologioId);
        if (orologio != null)
        {
            orologio.Giacenza += prodottoInCarrello.QuantitaInCarrello; // Ripristina lo stock
            _context.SaveChanges(); // Salva i cambiamenti nel database
            _logger.LogInformation("Stock aggiornato per il prodotto ID: {IdProdotto}, Nuovo stock: {Stock}", orologioId, orologio.Giacenza);
        }

        // Rimuovi il prodotto dal carrello
        carrello.Carrello.Remove(prodottoInCarrello);

        // Ricalcola il totale e la quantità del carrello
        decimal totale = 0;
        int quantita = 0;
        foreach (var item in carrello.Carrello)
        {
            totale += item.Orologio.Prezzo * item.QuantitaInCarrello; // Calcola il totale considerando la quantità
            quantita += item.QuantitaInCarrello; // Calcola la quantità totale
        }
        carrello.Totale = totale;
        carrello.Quantita = quantita;

        _logger.LogInformation("Carrello aggiornato per l'utente {UserId}, Nuovo totale: {Totale}, Nuova quantità: {Quantita}", userId, totale, quantita);

        // Salva i carrelli aggiornati nel file JSON
        Dictionary<string, CarrelloViewModel> carrelliUtenti = new();
        carrelliUtenti[userId] = carrello;
        SalvaTuttiICarrelli(carrelliUtenti);

        return true; // Success
    }
    catch (Exception ex)
    {
        _logger.LogError("Errore durante la rimozione dal carrello per l'utente {UserId}, Id Prodotto: {IdProdotto}, Exception: {Message}", userId, orologioId, ex.Message);
        return false; // Failure in case of error
    }
}

private void SalvaTuttiICarrelli(Dictionary<string, CarrelloViewModel> carrelliUtenti)
{
    try
    {
        var json = JsonConvert.SerializeObject(carrelliUtenti, Formatting.Indented);
        File.WriteAllText(CartFilePath, json); // Save the updated carts to the JSON file
        _logger.LogInformation("Carrello salvato su {FilePath}", CartFilePath);
    }
    catch (Exception ex)
    {
        _logger.LogError("Errore durante il salvataggio sul file: {Message}", ex.Message);
    }
}

public Orologio CercaProdottoPerId(List<Orologio> orologi, int id)
{
    try
    {
        Orologio orologio = null;
        foreach (var item in orologi)
        {
            if (item.Id == id)
            {
                orologio = item;
                break;
            }
        }
        return orologio;
    }
    catch (Exception ex)
    {
        _logger.LogError("Errore nella ricerca : {Message} \n Exception Type : {ExceptionType} \n Stack Trace : {StackTrace}", ex.Message, ex.GetType().Name, ex.StackTrace);
        return null;
    }
}

```
</details>

<details>
<summary>View</summary>

</details>

- ORDINI

<details>
<summary>Descrizione</summary>

- Visualizzazione di tutti gli ordini effettuati da un utente
- Bottone per tornare al proprio profilo
- Bottone in ogni ordine per visualizzarne i dettagli

</details>

<details>
<summary>Lista link</summary>

- Pagina profilo 
- Dettagli ordine

</details>

<details>
<summary>ViewModel</summary>

```C#
public class OrdiniViewModel
{
    public Cliente Cliente {get; set;}
    public List<Ordine> Ordini {get; set;}
}
```

</details>

<details>
<summary>Metodi Controller</summary>

```c#
```

</details>

<details>
<summary>View</summary>

</details>

- DETTAGLIO ORDINE

<details>
<summary>Descrizione</summary>

- Visualizzazione dei dettagli di un ordine specifico
- Bottone per tornare alla pagina ordini

</details>

<details>
<summary>Lista link</summary>

- Pagina ordini

</details>

<details>
<summary>ViewModel</summary>

```C#
public class OrdiniViewModel
{
    public Ordine Ordine {get; set;}
}
```

</details>

<details>
<summary>Metodi Controller</summary>

</details>

<details>
<summary>View</summary>

</details>



<details>
<summary>MODELLI GENERICI</summary>

GENERALE

```c#
public abstract class General
{
    public virtual int Id { get; set; }
    public virtual string Nome { get; set; }
}

```

PRODOTTO

```c#
public class Prodotto : General
{
    public  decimal Prezzo { get; set; }
    public int Giacenza { get; set; }
    public string Colore { get; set; }
    public string UrlImmagine { get; set; }
    public int CategoriaId { get; set; } 
    public Categoria Categoria { get; set; }
    public int MarcaId { get; set; }
    public Marca Marca {get; set;}
}

```

OROLOGIO

```c#
public class Orologio : Prodotto
{
    public string Modello{ get; set; }
    public string Referenza{ get; set; }
    public int MaterialeId { get; set; }
    public Materiale Materiale { get; set; }
    public int TipologiaId { get; set; }
    public Tipologia Tipologia { get; set; }
    public int Diametro { get; set; }
    public int GenereId { get; set; }
    public Genere Genere {get; set; }
}

```

CATEGORIA

```c#
public class Categoria : General
{

} 

```

MARCA

```c#
public class Marca : General
{

} 

```

MATERIALE 

```c#
public class Materiale : General
{

} 

```

TIPOLOGIA 

```c#
public class Tipologia : General
{

} 

```

GENERE 

```c#
public class Genere : General
{

} 

```

CLIENTE

```c#
public class Cliente : IdentityUser
{
    
}

```

ORDINE

```c#
public class Ordine : General
{
    public override string Nome
    {
        get
        {
            if (Id == 0 || Cliente == null)
            {
                return "Ordine-0000"; // in caso di dati incompleti
            }
            return $"BRT-{Id}_{Cliente.Id}";
        }
    }
    public DateTime DataAcquisto { get; set; } = DateTime.Now;
    public string ClienteId { get; set; } = string.Empty; 
    public Cliente Cliente { get; set; } = null!; 
    public List<OrdineDettaglio> OrdineDettagli { get; set; } = new List<OrdineDettaglio>();
}
```

OROLOGIO IN CARRELLO

```C#
public class OrologioInCarrello
{
    public int OrologioId { get; set; } 
    public Orologio Orologio { get; set; } 
    public int QuantitaInCarrello { get; set; } 
}
```

</details>

## PARTIAL VIEWS

- NAVBAR

<details>
<summary>Descrizione</summary>

GENERALE :

- Logo store
- bottone home
- Login (/logout) - Register
- Bottone pagina prodotti
- Logo carrello

AGGIUNTE ADMIN : 

- Menu gestionale

</details>

<details>
<summary>Lista link</summary>

GENERALE :

- Pagina home
- Pagina login
- Pagina register
- Pagina prodotti
- Paginacarrello

AGGIUNTE ADMIN : 

- Pagina menu gestioni

</details>

<details>
<summary>View</summary>

</details>

- FOOTER

<details>
<summary>Descrizione</summary>

- Descrizioni generale del sito
- Contatti
- Link esterni ai socials

</details>

<details>
<summary>Lista link</summary>

- Links socials

</details>

<details>
<summary>Metodi Controller</summary>

</details>

<details>
<summary>View</summary>

</details>

<details>
<summary>CDN</summary>

</details>

<details>
<summary>Loghi</summary>

</details>

<details>
<summary>Fonts</summary>

</details>

<details>
<summary>CSS</summary>

```CSS

```

</details>

<details>
<summary>Standards codice</summary>

- Metodi scritti in PascalCase
- Proprietà dei modelli scritti in PascalCase
- Variabili scritte in camelCase
- Commenti corti e non ripetitivi
- Corrispondenza delle variabili tra i vari file

</details>

<details>
<summary>Ultimi Steps</summary>

- Controllo eccezioni
- Revisione commenti e nomenclature
- Placeholders
- Arials
- Test App

</details>