Persze, készítsünk egy részletes összehasonlítást az Entity Framework (EF) Core és a MongoDB .NET Driver szintaxisa között, világos határokkal és példákkal.

Gondolj úgy rájuk, hogy mindkettő egy híd a C# kódod és az adatbázis között, de teljesen más típusú adatbázisokhoz, ami alapvetően meghatározza a működésüket és a szintaxisukat.

*   **Entity Framework Core:** Egy **ORM (Object-Relational Mapper)**. Feladata, hogy a relációs (SQL) adatbázisok tábláit, sorait és kapcsolatait leképezze C# objektumokra. Elrejti a nyers SQL-t, és egy objektumorientált világot biztosít.
*   **MongoDB .NET Driver:** Egy **hivatalos driver (illesztőprogram)**. A feladata, hogy hatékony és típusos kommunikációs csatornát biztosítson a MongoDB dokumentum-adatbázissal. Közelebb áll az adatbázis "gondolkodásmódjához", mint egy ORM.

### Alapvető koncepciók analógiája

| Entity Framework Core | MongoDB .NET Driver | Magyarázat |
| :--- | :--- | :--- |
| `DbContext` | `IMongoClient` + `IMongoDatabase` | Az EF `DbContext`-je egy munkamenetet és az adatbázis-kapcsolatot fogja össze. MongoDB-ben a `Client` a szerverkapcsolat, a `Database` pedig a konkrét adatbázis. |
| `DbSet<T>` | `IMongoCollection<T>` | Mindkettő egy-egy "táblát" vagy "gyűjteményt" reprezentál, ahol az azonos típusú objektumokat/dokumentumokat tároljuk. |
| Entitás osztály (POCO) | Dokumentum osztály (POCO) | Mindkét esetben egyszerű C# osztályokat használunk az adatok modellezésére. |
| `[Key]` attribútum | `[BsonId]` attribútum | Meghatározza, hogy melyik property felel meg az elsődleges kulcsnak (`_id`). |
| `SaveChanges()` | **NINCS KÖZVETLEN MEGFELELŐJE** | **Ez a legfontosabb különbség!** Az EF gyűjtögeti a módosításokat, és csak a `SaveChanges()` hívásakor küldi el őket az adatbázisnak egy tranzakcióban. A MongoDB driver parancsai azonnal végrehajtódnak. |

---

### Műveletek összehasonlítása (CRUD)

Tegyük fel, hogy van egy `Product` osztályunk.

#### 1. Kapcsolódás és kontextus

| Entity Framework Core | MongoDB .NET Driver |
| :--- | :--- |
| A `DbContext`-et általában a DI konténerben regisztráljuk, ami kezeli az élettartamát. | A `MongoClient`-et általában singletonként regisztráljuk, majd abból kérjük el az adatbázist és a gyűjteményt. |
| **`Program.cs` (ASP.NET Core):**<br>`builder.Services.AddDbContext<MyDbContext>(...);`<br><br>**Használat:**<br>`private readonly MyDbContext _context;`<br>`public MyService(MyDbContext context) { ... }` | **`Program.cs` (ASP.NET Core):**<br>`builder.Services.AddSingleton<IMongoClient>(s => new MongoClient("..."));`<br><br>**Használat:**<br>`private readonly IMongoCollection<Product> _collection;`<br>`public MyService(IMongoClient client) { var db = client.GetDatabase("shop"); _collection = db.GetCollection<Product>("products"); }` |

#### 2. Adatok olvasása (Query)

Itt látható a legtöbb hasonlóság a LINQ miatt, de fontos különbségekkel.

| Feladat | Entity Framework Core (LINQ to Entities) | MongoDB .NET Driver (LINQ Provider & Builders) |
| :--- | :--- | :--- |
| **Összes elem lekérése** | `_context.Products.ToList();` | `_collection.Find(_ => true).ToList();`<br>vagy<br>`_collection.Find(new BsonDocument()).ToList();` |
| **Szűrés (`WHERE`)** | `_context.Products.Where(p => p.Price > 100).ToList();` | **Lambda:**<br>`_collection.Find(p => p.Price > 100).ToList();` (Itt szinte azonosak!) |
| **Egy elem (`FirstOrDefault`)** | `_context.Products.FirstOrDefault(p => p.Id == id);` | `_collection.Find(p => p.Id == id).FirstOrDefault();` (Nagyon hasonló) |
| **Rendezés és lapozás** | `_context.Products.OrderBy(p => p.Name).Skip(20).Take(10).ToList();` | `_collection.Find(p => p.IsAvailable).SortBy(p => p.Name).Skip(20).Limit(10).ToList();` (Csak a `Take` vs. `Limit` a különbség) |
| **Kapcsolatok (`JOIN`)** | `_context.Products.Include(p => p.Category).ToList();` (Navigációs property betöltése) | A preferált megoldás a **beágyazás**. Ha "joinolni" kell, az az **aggregáció** a `$lookup` operátorral, ami jóval bonyolultabb és más szintaxist használ. |

#### 3. Új adat beszúrása (Create)

Itt már megjelenik a `SaveChanges` vs. közvetlen végrehajtás különbsége.

| Entity Framework Core | MongoDB .NET Driver |
| :--- | :--- |
| **1. Hozzáadás a kontextushoz (memóriában):**<br>`var product = new Product { Name = "Alma" };`<br>`_context.Products.Add(product);`<br><br>**2. Mentés az adatbázisba:**<br>`_context.SaveChanges();` | **Azonnali végrehajtás:**<br>`var product = new Product { Name = "Alma" };`<br>`_collection.InsertOne(product);` |

#### 4. Adatok módosítása (Update)

**Ez a másik legfontosabb különbség!**

| Entity Framework Core | MongoDB .NET Driver |
| :--- | :--- |
| **"Lekérdezem, módosítom, elmentem" modell:**<br><br>**1. Keresd meg az entitást:**<br>`var product = _context.Products.Find(id);`<br><br>**2. Módosítsd a C# objektumot:**<br>`product.Price = 150;`<br><br>**3. Mentsd el a változásokat:**<br>`_context.SaveChanges();`<br>(Az EF követi a változásokat és generál egy `UPDATE` SQL parancsot.) | **"Keresd meg és módosítsd" modell (direkt parancs):**<br><br>**Soha ne kérdezd le feleslegesen!**<br><br>**1. Definiálj egy szűrőt:**<br>`var filter = Builders<Product>.Filter.Eq(p => p.Id, id);`<br><br>**2. Definiáld a módosítást:**<br>`var update = Builders<Product>.Update.Set(p => p.Price, 150);`<br><br>**3. Hajtsd végre a parancsot:**<br>`_collection.UpdateOne(filter, update);` |

Az EF modellje kényelmes, de két adatbázis-hívást igényelhet (egy `SELECT`, egy `UPDATE`). A MongoDB modellje sokkal hatékonyabb, mert egyetlen paranccsal elvégzi a műveletet a szerveren.

#### 5. Adatok törlése (Delete)

Hasonlóan az Update-hez, itt is a hatékonyság a kulcs.

| Entity Framework Core | MongoDB .NET Driver |
| :--- | :--- |
| **"Lekérdezem, törlöm, elmentem" modell:**<br><br>**1. Keresd meg az entitást:**<br>`var product = _context.Products.Find(id);`<br><br>**2. Jelöld törlésre:**<br>`_context.Products.Remove(product);`<br><br>**3. Mentsd el a változásokat:**<br>`_context.SaveChanges();` | **"Keresd meg és töröld" modell (direkt parancs):**<br><br>**1. Definiálj egy szűrőt:**<br>`var filter = Builders<Product>.Filter.Eq(p => p.Id, id);`<br><br>**2. Hajtsd végre a törlést:**<br>`_collection.DeleteOne(filter);` |

### Összefoglalás – A legfontosabb határok

1.  **Végrehajtási Modell:**
    *   **EF Core:** **Unit of Work.** Gyűjti a módosításokat, majd a `SaveChanges()` hívásakor egyetlen tranzakcióban végrehajtja őket.
    *   **MongoDB:** **Közvetlen végrehajtás.** Minden `InsertOne`, `UpdateOne`, `DeleteOne` parancs azonnal lefut az adatbázison.

2.  **Módosítás és Törlés Filozófiája:**
    *   **EF Core:** Általában először le kell kérdezni az adatot a memóriába, hogy módosítani vagy törölni tudd.
    *   **MongoDB:** Hatékonysági okokból szinte soha nem kérdezed le az adatot, ha csak módosítani vagy törölni akarod. Közvetlenül a szervernek adsz parancsot egy szűrő alapján.

3.  **Adatkapcsolatok Kezelése:**
    *   **EF Core:** Navigációs property-k és `Include()` használata a JOIN-ok elvégzésére. Relációs gondolkodásmód.
    *   **MongoDB:** A **beágyazást (denormalizációt)** preferálja a teljesítmény miatt. A `JOIN`-szerű művelet (`$lookup`) egy bonyolultabb aggregációs lépés, nem egy alapvető lekérdezési funkció.

4.  **Séma Kezelése:**
    *   **EF Core:** **Schema-first vagy Code-first Migrations.** A séma szigorúan definiált és a migrációkkal van verziókövetve.
    *   **MongoDB:** **Séma nélküli (Schema-on-read).** A C# osztályod határozza meg a struktúrát, de az adatbázis nem kényszeríti ki. Ez nagyobb rugalmasságot, de nagyobb fegyelmet is igényel a fejlesztőtől.