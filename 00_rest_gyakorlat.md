Ez a laborgyakorlat a modern .NET fejlesztés egyik legfontosabb területét, a **REST API** fejlesztést mutatja be **ASP.NET Core** segítségével. Ez a tudás elengedhetetlen, ha modern webes backendet vagy microservice-eket szeretnél fejleszteni.

Menjünk végig a feladatokon, és magyarázom a mögöttes logikát, a kódokat és a "miért"-eket.

---

### 0. és 1. Feladat: Alapozás (Adatbázis és Projekt)

**Mi történik?**
Mielőtt kódot írnánk, kell egy környezet.
1.  **Adatbázis:** Létrehozunk egy MS SQL adatbázist (`mssql.sql` futtatása). Ez lesz az adattároló (termékek, kategóriák, stb.).
2.  **Projekt:** Klónozunk egy előkészített „csontvázat”. Ez egy **ASP.NET Core Web API** projekt.

**Fontos fájlok a projektben:**
*   **`Program.cs`**: Ez a belépési pont. Itt konfiguráljuk a webes kiszolgálót (Kestrel) és a szolgáltatásokat (Dependency Injection). Itt mondjuk meg, hogy "figyelj, használni fogunk Entity Framework-öt".
*   **`appsettings.json`**: Konfigurációs fájl. Itt van a **Connection String** (kapcsolati karakterlánc), ami megmondja a programnak, hol találja az adatbázist.
*   **`Data/DataDrivenDbContext.cs`**: Ez az Entity Framework (EF) `DbContext` osztálya. Ez a híd a C# kód és az SQL adatbázis között.

---

### 2. Feladat: Első Controller (Hello World)

**Cél:** Megérteni, hogyan működik egy API végpont (Endpoint).

**A kód részletezve:**

```csharp
// 1. URL meghatározása: api/HelloController (a "Controller" szót levágja) -> api/Hello
[Route("api/[controller]")]
// 2. Jelzi, hogy ez egy API vezérlő (validációt és formázást segít)
[ApiController] 
public class HelloController : ControllerBase
{
    // 3. Ez a metódus a HTTP GET kérésekre hallgat
    [HttpGet]
    // 4. A [FromQuery] azt jelenti, hogy az URL-ből olvassa ki a paramétert
    // pl.: api/Hello?name=Bela
    public string Hello([FromQuery] string name)
    {
        // Ha nincs név, null-t kapnánk, ezért ellenőrizzük
        if (string.IsNullOrEmpty(name))
        {
            return "Hello noname!";
        }
        return "Hello " + name;
    }
    
    // Egy másik végpont (route), ami az útvonalból (path) olvassa az adatot
    // pl.: api/Hello/Bela
    [HttpGet("{personName}")] 
    public string HelloRoute(string personName) // Itt a változónévnek egyeznie kell a { }-ben lévővel
    {
        return "Hello route " + personName;
    }
}
```

**Tanulság:**
*   **Controller:** Egy osztály, ami csoportosítja az összetartozó végpontokat.
*   **Attribute Routing (`[Route]`):** Így mondjuk meg, milyen URL-en érhető el a kód.
*   **Parameter Binding:** A keretrendszer okos. Ha a változó neve `name`, keresni fog `?name=` query paramétert, vagy `{name}` útvonal elemet.

---

### 3. Feladat: Termékek keresése API (DTO-k és Lekérdezés)

**Cél:** Adatokat olvasni az adatbázisból és visszaadni a kliensnek.

**Kiemelt fogalom: DTO (Data Transfer Object)**
Miért nem a `Product` entitást adjuk vissza? Mert az entitás közvetlenül az adatbázishoz van kötve (lehetnek benne körkörös hivatkozások, technikai mezők). A **DTO** egy buta osztály (vagy `record`), ami csak azt az adatot tartalmazza, amit ki akarunk küldeni.

**A megoldás kódja:**

```csharp
[Route("api/[controller]")]
[ApiController]
public class ProductController : ControllerBase
{
    // DEPENDENCY INJECTION (DI)
    // Nem mi hozzuk létre a DbContext-et "new"-val, hanem a rendszer "beadja" nekünk.
    private readonly DataDrivenDbContext _dbContext;

    public ProductController(DataDrivenDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public List<Dtos.Product> List([FromQuery] string search = null, [FromQuery] int from = 0)
    {
        // LINQ lekérdezés építése
        // 1. Alap: minden termék
        var filteredList = _dbContext.Product.AsQueryable();

        // 2. Szűrés: ha van keresőszó, rászűrünk a névre
        if (!string.IsNullOrEmpty(search))
        {
             filteredList = filteredList.Where(p => p.Name.Contains(search));
        }

        // 3. Lapozás és Projekció (Mapping)
        return filteredList
            .Skip(from)  // Kihagyunk 'from' darab elemet (lapozás eleje)
            .Take(5)     // Csak 5-öt kérünk (lapméret)
            // Átalakítjuk az adatbázis entitást (p) DTO-vá (new Dtos.Product)
            .Select(p => new Dtos.Product(p.Id, p.StringId, p.Name, p.Price, p.Vat, p.Stock))
            .ToList();   // Itt fut le a lekérdezés az adatbázisban (SQL SELECT)
    }
}
```

**Tanulság:**
*   **DI (Dependency Injection):** A konstruktorban kapjuk meg az adatbázis-kapcsolatot.
*   **LINQ:** C# kóddal írunk SQL-szerű logikát. A `.Where`, `.Skip`, `.Take` csak akkor fordul SQL-re és fut le, amikor a végén meghívjuk a `.ToList()`-et.
*   **Projekció (`.Select`):** Itt történik az Entitás -> DTO átalakítás.

---

### 4. Feladat: CRUD műveletek (Create, Update, Delete)

Most módosítjuk is az adatokat. A REST elveket követjük:
*   **GET**: Olvasás
*   **POST**: Létrehozás (Create)
*   **PUT**: Módosítás (Update - teljes csere)
*   **DELETE**: Törlés

#### 4.1. Egy termék lekérése ID alapján (GET)

```csharp
[HttpGet("{id}")] // URL: api/product/123
public ActionResult<Dtos.Product> Get(int id)
{
    // Keresés ID alapján
    var dbProduct = _dbContext.Product.Find(id);

    // Ha nincs ilyen ID, 404 Not Found-ot adunk vissza
    if (dbProduct == null)
        return NotFound();

    // Ha van, visszaadjuk DTO-ként (200 OK)
    return new Dtos.Product(...);
}
```
*   **ActionResult:** Lehetővé teszi, hogy vagy adatot adjunk vissza, vagy HTTP státuszkódot (pl. `NotFound()`).

#### 4.2. Új termék létrehozása (POST)

```csharp
[HttpPost]
public ActionResult Create([FromBody] Dtos.NewProduct newProductDto)
{
    // 1. DTO-ból Entitást csinálunk
    var dbProduct = new Dal.Product
    {
        Name = newProductDto.Name,
        Price = newProductDto.Price,
        // ... többi mező másolása
    };

    // 2. Hozzáadjuk a context-hez (még nincs SQL INSERT)
    _dbContext.Product.Add(dbProduct);

    // 3. Mentés (Itt fut le az SQL INSERT, és az adatbázis generál ID-t)
    _dbContext.SaveChanges();

    // 4. Szabványos 201 Created válasz
    // Megadjuk, hol érhető el az új elem (Get metódus, az új ID-val)
    return CreatedAtAction(nameof(Get), new { id = dbProduct.Id }, dbProduct.Id);
}
```
*   **`[FromBody]`:** A kliens a kérés törzsében (JSON formátumban) küldi az adatot.
*   **Id generálás:** Az ID-t az adatbázis generálja a `SaveChanges` híváskor.

#### 4.3. Módosítás (PUT) és Törlés (DELETE)

**PUT (Update):**
```csharp
[HttpPut("{id}")]
public ActionResult Update(int id, [FromBody] Dtos.NewProduct updatedDto)
{
    // Megkeressük a régit
    var dbProduct = _dbContext.Product.Find(id);
    if (dbProduct == null) return NotFound();

    // Átírjuk az adatait
    dbProduct.Name = updatedDto.Name;
    dbProduct.Price = updatedDto.Price;
    
    // Mentés (az EF látja, hogy változott, SQL UPDATE-et generál)
    _dbContext.SaveChanges();

    return NoContent(); // 204 No Content (sikeres, de nincs válasz adat)
}
```

**DELETE:**
```csharp
[HttpDelete("{id}")]
public ActionResult Delete(int id)
{
    var dbProduct = _dbContext.Product.Find(id);
    if (dbProduct == null) return NotFound(); // Vagy NoContent(), ízlés kérdése

    _dbContext.Product.Remove(dbProduct);
    _dbContext.SaveChanges(); // SQL DELETE

    return NoContent();
}
```

---

### 5. Feladat (Opcionális): Komplexebb logika (Kategória kezelés)

Itt egy életszerűbb problémát oldunk meg. A kliens egy kategória *nevet* küld ("Szórakoztató elektronika"), de az adatbázisban a kategória egy külön tábla, és ID-val kell hivatkozni rá.

**A logika:**
1.  Megnézzük, létezik-e már ilyen nevű kategória az adatbázisban.
2.  Ha igen: felhasználjuk az ID-ját.
3.  Ha nem: létrehozunk egy újat, elmentjük, és az új ID-t használjuk.

```csharp
// Ez egy speciális update metódus, pl. áremelés egy kategóriában
@Modifying
@Transactional
// ... itt komplexebb repository logikát írhatnánk, 
// de a laborban a kódrészlet azt mutatja, hogyan kezeljük 
// a kategória nevet létrehozáskor (lásd lentebb a magyarázatot).
```

*A PDF képernyőképe alapján itt inkább egy komplexebb mentési logikáról van szó:*
Amikor új terméket hozunk létre:
```csharp
// Megkeressük a kategóriát név alapján
var category = _dbContext.Category.FirstOrDefault(c => c.Name == dto.CategoryName);

if (category == null) {
    // Ha nincs, létrehozzuk
    category = new Dal.Category { Name = dto.CategoryName };
    _dbContext.Category.Add(category);
    // Itt nem kötelező azonnal SaveChanges, az EF okos, 
    // a termék mentésekor elmenti az új kategóriát is (cascade).
}

// Hozzárendeljük a termékhez
newProduct.Category = category;
```

---

### 6. Feladat (Opcionális): Aszinkronitás (Async/Await)

**Miért kell ez?**
Egy webszerver véges számú szálat (thread) tud kezelni. Ha egy kérés blokkolja a szálat (vár az adatbázisra), a szerver lassul. Az aszinkron működésnél, amíg az adatbázis dolgozik, a webszerver szála felszabadul és más kérést tud kiszolgálni.

**Hogyan alakítjuk át a kódot?**

1.  A visszatérési érték `Task<ActionResult<...>>` lesz.
2.  Használjuk az `async` kulcsszót a metóduson.
3.  Az EF metódusok aszinkron változatát használjuk: `ToList()` helyett `ToListAsync()`, `SaveChanges()` helyett `SaveChangesAsync()`.
4.  Ezek elé `await` kulcsszót írunk.

**Példa:**
```csharp
[HttpGet]
public async Task<ActionResult<List<Dtos.Product>>> ListAsync()
{
    // A szál itt felszabadul, amíg az SQL szerver dolgozik
    var list = await _dbContext.Product.ToListAsync(); 
    
    // ... mapping ...
    return dtos;
}
```

### Összefoglalás

Ebben a laborban megtanultad:
1.  Hogyan kell **ASP.NET Core Web API** projektet létrehozni.
2.  Hogyan kell **Controller**-eket és **Endpoint**-okat (GET, POST, PUT, DELETE) definiálni.
3.  Hogyan használd a **Dependency Injection**-t a `DbContext` eléréséhez.
4.  Hogyan használd az **Entity Framework Core**-t adatok olvasására és írására.
5.  Miért és hogyan használunk **DTO**-kat az adatbázis entitások helyett.
6.  Hogyan lehet **aszinkronná** tenni a kódot a jobb skálázhatóság érdekében.