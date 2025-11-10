Abszolút! Lássuk a negyedik, Entity Framework-központú diasort a már megszokott részletességgel, minden diára és kódrészletre kitérve.

---

### Bevezetés és Alapok (1-8. dia)

**1. dia: Címlap**
Az előadás címe: **"Adatvezérelt rendszerek: ADO.NET, Entity Framework"**. Ez a diasor a .NET keretrendszer konkrét adatelérési technológiáira fókuszál, az alapoktól (ADO.NET) a modern, magas szintű megoldásig (Entity Framework).

**2. dia: Adatelérési osztálykönyvtárak**
Ez az ábra bemutatja, hogyan épül fel a kommunikáció a programunk és az adatbázis-szerver között.
1.  **Kliens alkalmazás:** A mi programunk (pl. egy C# konzolalkalmazás vagy egy webalkalmazás üzleti logikája).
2.  **Adatelérési osztálykönyvtár:** Ez egy magas szintű keretrendszer, mint az **Entity Framework** vagy az **ADO.NET**. Ez ad egy programozóbarát felületet az adatbázis-műveletekhez.
3.  **Adatbázis meghajtó (Driver):** Ez egy alacsonyabb szintű komponens (pl. `System.Data.SqlClient`), ami az adott adatbázis-típus (pl. MS SQL Server) kommunikációs protokollját ismeri.
4.  **Kommunikációs protokoll:** A hálózati "nyelv", amin a kliens és a szerver beszélgetnek.

**4. dia: Alapprobléma**
Ez a dia ismétli az ORM (Objektum-Relációs Leképzés) szükségességét.
*   **Adat ≠ Objektum:** A relációs adatbázisok adatközpontú, táblás világát kell leképezni az objektumorientált programozás objektumközpontú világára.
*   **SQL nyelv hátrányai programozói szempontból:**
    *   **Nem típusos:** Az SQL-ben a lekérdezések szövegként ("string") léteznek. Egy elgépelést a tábla- vagy oszlopnévben csak futásidőben veszünk észre, nem fordításkor.
    *   **Nem objektum alapú:** Nem illeszkedik természetesen a C# osztályokhoz és objektumokhoz.
    *   **Nem épül be nyelvi elemként:** Különálló, "idegen" nyelv a C# kódon belül.

**5. dia: Adatelérés LINQ segítségével**
A **LINQ (Language-Integrated Query)** a Microsoft zseniális megoldása az előző probléma áthidalására. Lehetővé teszi, hogy SQL-szerű lekérdezéseket írjunk **közvetlenül C# kódban**, ami:
*   **Típusos:** A fordítóprogram ellenőrzi a lekérdezést. Ha elgépeled a `product.Name` propertyt, fordítási hibát kapsz.
*   **Objektumokra épül:** Közvetlenül a `Product` osztályunkkal és annak tulajdonságaival dolgozhatunk.
*   **Beépül a nyelvbe:** A `from`, `where`, `select` kulcsszavak a C# nyelv részei.

```csharp
// Egy LINQ lekérdezés, ami a 'db' adatbázis-kontextus
// 'Products' gyűjteményéből kiválasztja a "Lego" nevű terméket.
var results = from product in db.Products
              where product.Name == "Lego"
              select product;
```
Az Entity Framework ezt a LINQ lekérdezést a háttérben lefordítja a megfelelő SQL `SELECT` parancsra.

**6. dia: Entity Framework (EF)**
*   **Mi az?** Egy ORM rendszer a .NET-hez.
*   **Célja:** Szétválasztani a **logikai modellt** (ahogy az adatbázis látja a világot: táblák, oszlopok) a **fogalmi modelltől** (ahogy a programozó látja: osztályok, propertyk).
*   **Függetlenítés:** Lehetővé teszi, hogy az alkalmazásunk független legyen a konkrét adatbázis-motortól (pl. SQL Serverről Oracle-re váltani kevesebb munkával jár).
*   **EF vs. EF Core:**
    *   **Entity Framework (6.x):** A régebbi, csak Windowson futó .NET Frameworkhöz készült, stabil, de már nem fejlesztik aktívan.
    *   **Entity Framework Core (EF Core):** A modern, platformfüggetlen (.NET Core / .NET 5+), folyamatosan fejlesztett verzió. **Ma már ezt használjuk.**

**7-8. dia: Entitások és Adatbázis Providerek**
*   **Entitás:** Egy osztály, ami az adatmodellünk egy elemét (tipikusan egy adatbázis-táblát) reprezentálja. Csak tulajdonságai (adatok) és relációi (kapcsolatok) vannak, viselkedést (metódusokat) nem definiál.
*   **Provider:** Az EF egy "bővítmény" (provider) segítségével kommunikál a különböző adatbázis-motorokkal. A provider feladata a LINQ lekérdezések lefordítása az adott adatbázis SQL dialektusára. Támogatott adatbázisok pl.: MS SQL Server, SQLite, PostgreSQL, MySQL. Létezik `InMemory` provider is, ami teszteléshez hasznos.

---

### Fejlesztési Módszerek (Workflows) (9-12. dia)

Az EF három fő fejlesztési megközelítést támogat.

**9-10. dia: Database First**
*   **Mikor?** Ha már van egy meglévő, kész adatbázisunk.
*   **Folyamat:** Az EF egy eszköz segítségével "visszafejti" az adatbázis sémáját, és legenerálja a C# entitás osztályokat és a `DbContext`-et.
*   **Frissítés:** Ha az adatbázis sémája megváltozik (pl. egy új oszlop kerül bele), a generálási folyamatot újra le kell futtatni.

**11. dia: Model First (Elavult)**
*   **Mikor?** Ha nulláról indulunk, és vizuálisan szeretnénk tervezni.
*   **Folyamat:** A Visual Studio egy grafikus tervezőjében (designer) rajzoljuk meg az entitásokat és a kapcsolataikat. Ebből a modellből az EF legenerálja az adatbázist létrehozó SQL szkriptet és a C# osztályokat is.
*   **Megjegyzés:** Ez a módszer a régi EF 6-ban volt népszerű, az **EF Core már nem támogatja**.

**12. dia: Code First (A modern, javasolt módszer)**
*   **Mikor?** Ha nulláról indulunk, és a teljes logikát a C# kódban szeretnénk tartani.
*   **Folyamat:** Mi magunk írjuk meg a C# entitás osztályokat (POCO - Plain Old CLR Object). Az EF a kódunk alapján képes legenerálni és naprakészen tartani az adatbázis sémáját (a **Migrációk** segítségével, lásd később).
*   **Leképezés testreszabása:** A kód és az adatbázis közötti leképezést (pl. táblanevek, oszloptípusok) három módon befolyásolhatjuk:
    1.  **Konvenciók:** EF alapértelmezett szabályai (pl. az `Id` nevű property lesz az elsődleges kulcs).
    2.  **Attribútumok (Data Annotations):** Az osztályaink és propertyjeink fölé írt "címkék" (pl. `[Key]`, `[MaxLength(50)]`).
    3.  **Fluent API:** A `DbContext` `OnModelCreating` metódusában, C# kóddal, láncolt hívásokkal definiáljuk a leképezés minden részletét. Ez a legrugalmasabb módszer.

---

### Code First a Gyakorlatban (13-32. dia)

**13. dia: Termék entitás osztály**
Egy tipikus entitás osztály (POCO).

```csharp
public partial class Product
{
    // Egyszerű, skalár tulajdonságok, amik az adatbázis-tábla oszlopaira fognak leképeződni.
    public int Id { get; set; } // Konvenció szerint ez lesz az elsődleges kulcs.
    public string? Name { get; set; }
    public double? Price { get; set; }
    // A '?' jelzi, hogy a tulajdonság lehet NULL az adatbázisban (nullable).

    // --- Navigációs tulajdonságok (Navigation Properties) ---
    // Ezek definiálják a kapcsolatokat más entitásokkal.

    // Egy-a-többhöz kapcsolat 'egy' oldala: Egy terméknek PONTOSAN EGY kategóriája van.
    // A 'virtual' kulcsszó a Lazy Loadinghoz (lásd később) kellhet.
    public virtual Category? Category { get; set; }
    public int? CategoryId { get; set; } // Ez a külső kulcs (Foreign Key) property.

    // Egy-a-többhöz kapcsolat 'több' oldala: Egy termék TÖBB rendelési tételben is szerepelhet.
    public virtual ICollection<OrderItem> OrderItems { get; } = new List<OrderItem>();
}```

**14. dia: Adatbázis környezet (DbContext)**
A `DbContext` osztály a kapu az adatbázishoz. Ez egy munkamenetet (session) reprezentál.
*   **`DbSet<T>`:** Minden, a modellben szereplő táblát egy `DbSet<T>` propertyként kell felvenni. Ezen keresztül érjük el és kérdezzük le az adott tábla entitásait.
*   **`OnModelCreating(ModelBuilder)`:** Ebben a metódusban tudjuk a **Fluent API** segítségével felülbírálni a konvenciókat és finomhangolni a leképezést.

```csharp
public class MyDbContext : DbContext
{
    // A 'Products' táblát reprezentáló gyűjtemény.
    public DbSet<Product> Products { get; set; }
    // A 'Categories' táblát reprezentáló gyűjtemény.
    public DbSet<Category> Categories { get; set; }

    // Itt történik a modell testreszabása a Fluent API-val.
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Megmondjuk az EF-nek, hogy a Product entitás...
        modelBuilder.Entity<Product>()
            // ...Name propertyje kötelező (NOT NULL lesz az adatbázisban).
            .Property(b => b.Name).IsRequired();

        // Definiáljuk a kapcsolatot: egy Product-nak van EGY (HasOne) Category-ja,
        // aminek pedig SOK (WithMany) Product-ja van.
        modelBuilder.Entity<Product>()
            .HasOne(p => p.Category)
            .WithMany(c => c.Products);
    }
}
```

**16-32. dia: A leképezés részletei (Code First)**
Ezek a diák a Fluent API és az Attribútumok használatát mutatják be részletesen.

*   **Elsődleges kulcs (19. dia):**
    *   **Konvenció:** `Id` vagy `TypeNameId` (pl. `CarId`).
    *   **Attribútum:** `[Key]` a property fölé.
    *   **Fluent API:** `modelBuilder.Entity<Car>().HasKey(c => c.LicensePlate);`

*   **Azonosító generálás (20-22. dia):**
    *   Konvenció szerint az `int` és `Guid` típusú kulcsok értékét az adatbázis generálja (`IDENTITY` vagy `newid()`).
    *   A `[DatabaseGenerated(DatabaseGeneratedOption.None)]` attribútummal kikapcsolható a generálás.
    *   `[DatabaseGenerated(DatabaseGeneratedOption.Computed)]` esetén az értéket minden `UPDATE`-nél az adatbázis számolja (pl. egy `LastUpdated` oszlop).

*   **Relációk (24-26. dia):**
    *   Az EF a navigációs propertyk alapján automatikusan felismeri a kapcsolatokat és a külső kulcsokat (konvenció).
    *   Ha két entitás között több kapcsolat is van (pl. egy `Post`-nak van `Author`-a és `Contributor`-a is, mindkettő `User`), akkor az `[InverseProperty]` attribútummal kell egyértelműsíteni, hogy melyik navigációs property melyikkel van párban.
    *   **Több-többes kapcsolat:** Ha mindkét osztály tartalmaz egy-egy gyűjtemény típusú navigációs propertyt a másikra, az EF Core automatikusan létrehoz egy rejtett **kapcsoló táblát**.

*   **Shadow Properties (27-28. dia):**
    *   Olyan oszlopok az adatbázisban, amik nem szerepelnek a C# entitás osztályban.
    *   Fluent API-val hozhatók létre: `modelBuilder.Entity<Blog>().Property<DateTime>("LastUpdated");`
    *   Tipikusan technikai adatokhoz használják, mint pl. külső kulcsok vagy időbélyegek.

*   **Indexek (29. dia):**
    *   A lekérdezések gyorsítására szolgálnak. Alapból minden külső kulcsra létrejön egy.
    *   Fluent API-val hozhatunk létre továbbiakat, akár egyedi (`IsUnique()`) vagy összetett indexeket is.
      `modelBuilder.Entity<Person>().HasIndex(p => new { p.FirstName, p.LastName });`

*   **Nem leképezett propertyk (30. dia):**
    *   A `[NotMapped]` attribútummal megjelölt propertyket az EF figyelmen kívül hagyja, nem próbálja meg oszlopként létrehozni az adatbázisban. Hasznos pl. számított vagy ideiglenes értékekhez.

---

### Műveletek az Entity Frameworkkel (33-58. dia)

**33-34. dia: A DbContext szerepe és életciklusa**
*   **Központi osztály:** Minden adatbázis-művelet a `DbContext` egy példányán keresztül történik.
*   **Állapotkövetés (Change Tracking):** A `DbContext` "emlékszik" a betöltött entitásokra és figyeli a rajtuk végzett változtatásokat.
*   **Rövid életciklus:** A `DbContext`-et a lehető legrövidebb ideig szabad csak használni. Tipikus minta: hozz létre egyet egy művelethez (`using` blokkban), végezd el a munkát, majd hagyd, hogy felszabaduljon. Ez megakadályozza a memória-problémákat és a konkurencia-konfliktusokat.
*   **Unit of Work minta:** A `DbContext` a Unit of Work (Munkaegység) tervezési minta egy megvalósítása. Összegyűjti a változtatásokat, és a `SaveChanges()` hívásakor egyetlen tranzakcióban küldi el őket az adatbázisnak.

**35-38. dia: CRUD műveletek**
*   **Beszúrás (Create):**
    1.  `var newEntity = new Product { ... };` (objektum létrehozása)
    2.  `db.Products.Add(newEntity);` (hozzáadás a `DbContext` követett objektumaihoz, az állapota `Added` lesz)
    3.  `db.SaveChanges();` (az EF generál egy `INSERT` parancsot és elküldi az adatbázisnak)

*   **Módosítás (Update):**
    1.  `var product = db.Products.Find(1);` (entitás betöltése az adatbázisból)
    2.  `product.Name = "Új név";` (property módosítása; az EF észleli a változást, az állapot `Modified` lesz)
    3.  `db.SaveChanges();` (az EF generál egy `UPDATE` parancsot)

*   **Törlés (Delete):**
    1.  `var product = db.Products.Find(1);` (entitás betöltése)
    2.  `db.Products.Remove(product);` (törlésre jelölés, az állapot `Deleted` lesz)
    3.  `db.SaveChanges();` (az EF generál egy `DELETE` parancsot)

*   **Entitás állapotok:** `Added`, `Modified`, `Deleted`, `Unchanged`, `Detached`. A `SaveChanges()` csak az `Added`, `Modified`, `Deleted` állapotú entitásokkal foglalkozik.

**39-40. dia: Tömeges (Bulk) műveletek**
A hagyományos `SaveChanges()` nem hatékony, ha több ezer sort kell módosítani, mert minden sort külön betölt a memóriába. Az EF Core 7-től bevezették a `ExecuteUpdate` és `ExecuteDelete` metódusokat, amik közvetlenül SQL-t generálnak a memóriába töltés nélkül.

```csharp
// Töröl minden "Ro"-val kezdődő nevű személyt egyetlen DELETE paranccsal.
db.Persons.Where(p => p.Name.StartsWith("Ro")).ExecuteDelete();

// Minden "Rozsa"-val kezdődő nevű személy korát növeli eggyel.
db.Persons
    .Where(p => p.Name.StartsWith("Rozsa"))
    .ExecuteUpdate(s => s.SetProperty(p => p.Age, p => p.Age + 1));
```

**41-43. dia: Tranzakciók kezelése**
*   Alapértelmezetten a `SaveChanges()` egyetlen, implicit tranzakcióba foglalja az összes változtatást.
*   Ha több `SaveChanges()` hívást, vagy más, adatbázistól független logikát akarunk egyetlen tranzakcióba foglalni, használhatjuk a manuális tranzakciókezelést.

```csharp
// Explicit tranzakció indítása.
using (var transaction = db.Database.BeginTransaction())
{
    try
    {
        // ... művelet 1 ...
        db.SaveChanges();

        // ... művelet 2 ...
        db.SaveChanges();

        // Ha minden sikeres, véglegesítjük a tranzakciót.
        transaction.Commit();
    }
    catch (Exception)
    {
        // Bármilyen hiba esetén a transaction.Rollback() automatikusan lefut a 'using' blokk végén.
        // Itt naplózhatjuk a hibát.
    }
}
```

**44-48. dia: Lekérdezések (Querying)**
*   A `DbContext` állapotkövetése miatt, ha egy már memóriában lévő entitást kérdezünk le újra, az EF a memóriából fogja visszaadni a módosított állapotot, nem futtat új SQL lekérdezést.
*   Ha csak olvasni akarunk adatokat, és nem tervezzük módosítani őket, érdemes a `.AsNoTracking()` metódust használni. Ez kikapcsolja az állapotkövetést, ami jelentősen gyorsíthatja a lekérdezést.
    `var products = db.Products.AsNoTracking().Where(...).ToList();`

**49-57. dia: Navigáció és kapcsolódó adatok betöltése**
Amikor lekérdezünk egy `Product`-ot, a hozzá tartozó `Category` alapból nem töltődik be. Ez az N+1 lekérdezési probléma forrása lehet. Három fő stratégia van a kapcsolódó adatok betöltésére:

1.  **Előtöltés (Eager Loading):** **EZ A JAVASOLT MÓDSZER.** Az `.Include()` metódussal megmondjuk az EF-nek, hogy a fő lekérdezéssel együtt, egyetlen `JOIN`-nal töltse be a kapcsolódó adatokat is.

    ```csharp
    // Betölti a blogokat ÉS a hozzájuk tartozó posztokat egyetlen SQL lekérdezéssel.
    var blogs = context.Blogs.Include(b => b.Posts).ToList();
    ```

2.  **Explicit betöltés (Explicit Loading):** Akkor használjuk, ha egy már memóriában lévő entitáshoz utólag szeretnénk betölteni a kapcsolódó adatait.

    ```csharp
    var blog = context.Blogs.Find(1); // Blog betöltése
    // Posztok utólagos betöltése külön lekérdezéssel.
    context.Entry(blog).Collection(b => b.Posts).Load();
    ```

3.  **Késleltetett betöltés (Lazy Loading):** **NEM JAVASOLT, VESZÉLYES!** Ha be van kapcsolva, az EF automatikusan futtat egy új SQL lekérdezést a háttérben, amikor először hozzáérünk egy be nem töltött navigációs propertyhez. Ez könnyen az N+1 problémához vezet: ha lekérdezel 20 posztot, majd egy cikluson belül mindegyiknek lekérdezed a blogját (`post.Blog`), az 1 (posztok) + 20 (blogok) = 21 külön adatbázis-lekérdezést fog eredményezni!

**58. dia: Közvetlen SQL lekérdezések**
Ha egy lekérdezés túl bonyolult a LINQ számára, vagy egy tárolt eljárást szeretnénk meghívni, használhatjuk a `.FromSqlRaw()` vagy `.FromSqlInterpolated()` metódusokat. Fontos a paraméterezés az SQL Injection támadások elkerülése végett.

---

### Migrációk és Záró Témakörök (59-78. dia)

**59-64. dia: Állapotkövetés és Konkurencia többrétegű alkalmazásban**
Webes környezetben (ahol a `DbContext` csak egy kérés erejéig él) az állapotkövetés nem működik automatikusan. Amikor a kliens visszaküld egy módosított objektumot, egy új, üres `DbContext`-nek kell megmondani, hogy az objektum `Modified` állapotú. Az ütközés-felismerés (concurrency) itt különösen fontossá válik.

**64-69. dia: Ütközés felismerés EF-fel**
Az EF támogatja az optimista konkurenciakezelést.
*   **Működése:** A generált `UPDATE` vagy `DELETE` parancs `WHERE` feltételébe nem csak az `ID`-t teszi bele, hanem az eredeti értékeket vagy egy verziószámot is. Ha a rekord közben megváltozott, a `WHERE` feltétel nem fog illeszkedni, a parancs 0 sort fog módosítani, és az EF egy `DbUpdateConcurrencyException` kivételt dob.
*   **Konfigurálás:**
    *   `[ConcurrencyCheck]` attribútum: Az ezzel megjelölt property bekerül a `WHERE` feltételbe.
    *   `[Timestamp]` attribútum egy `byte[]` propertyn: Az EF ezt egy adatbázis-specifikus verziószám oszlopra (`rowversion` SQL Serverben) képezi le, amit az adatbázis minden módosításkor automatikusan frissít. **Ez a leghatékonyabb módszer.**

**70-75. dia: Migrációk (Migrations)**
A **Code First** megközelítés lelke. A migrációk segítségével tudja az EF az adatbázis sémáját szinkronban tartani a C# kódunkkal, az adatok elvesztése nélkül.
*   **Folyamat:**
    1.  Módosítjuk a C# entitás osztályainkat (pl. új propertyt adunk hozzá).
    2.  Kiadunk egy parancsot: `dotnet ef migrations add UjPropertyHozzaadasa`. Az EF összehasonlítja az aktuális modellt az előző migráció "pillanatképével", és legenerál egy C# kódot, ami leírja a változtatáshoz szükséges `Up()` (alkalmaz) és `Down()` (visszavon) lépéseket (pl. `migrationBuilder.AddColumn(...)`).
    3.  Kiadunk egy másik parancsot: `dotnet ef database update`. Az EF lefuttatja a függőben lévő migráció(ka)t, és módosítja az adatbázis sémáját (lefuttatja a megfelelő `ALTER TABLE` parancsot).

Ez a mechanizmus lehetővé teszi az adatbázis-séma verziókövetését a forráskóddal együtt.